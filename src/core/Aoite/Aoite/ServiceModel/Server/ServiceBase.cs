using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个服务的控制。
    /// </summary>
    public interface IServiceHandler
    {
        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        void Calling(ContractContext context);
        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        void Called(ContractContext context);
    }

    /// <summary>
    /// 表示一个预定义的服务基类。一般情况下，契约服务并非需要继承此基类。
    /// </summary>
    public abstract class ServiceBase : IIdentityExtendService, IServiceHandler
    {
        private static readonly Lazy<IIocContainer> LazyContainer = new Lazy<IIocContainer>(CreateCommandModelContainer);
        private readonly static NotSupportedException ServerNotSupportedException = new NotSupportedException("没有启动契约服务器，无法获取给定的服务。");

        static IIocContainer CreateCommandModelContainer()
        {
            var container = new IocContainer();
            container.AddService<Aoite.CommandModel.IUserFactory>(ContractUserFactory.Instance);
            container.AddService<ContractContext>(lmps => ContractContext.Current);
            container.AddService<Aoite.Cache.ICacheProvider>(lmps =>
            {
                var context = container.GetService<ContractContext>();
                if(context == null) throw ServerNotSupportedException;
                return context.CacheProvider;
            });
            container.AddService<IContractServer>(lmps =>
            {
                var context = container.GetService<ContractContext>();
                if(context == null) throw ServerNotSupportedException;
                return context.Source.Server;
            });
            return container;
        }

        /// <summary>
        /// 在单元测试的运行环境创建一个新的契约上下文，并创建指定的服务。非单元测试运行环境，此方法将会抛出异常。
        /// </summary>
        /// <typeparam name="TService">服务的契约类型。</typeparam>
        /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
        /// <returns>返回服务的契约实例。</returns>
        public static TService CreateService<TService>(Action<CommandModel.MockExecutorFactory> mockFactoryCallback = null)
            where TService : ServiceBase, new()
        {
            return CreateService<TService>(null, mockFactoryCallback);
        }

        /// <summary>
        /// 在单元测试的运行环境创建一个新的契约上下文，并创建指定的服务。非单元测试运行环境，此方法将会抛出异常。
        /// </summary>
        /// <typeparam name="TService">服务的契约类型。</typeparam>
        /// <param name="user">当前已授权的登录用户。</param>
        /// <param name="mockFactoryCallback">模拟的执行器工厂回调函数。</param>
        /// <returns>返回服务的契约实例。</returns>
        public static TService CreateService<TService>(object user = null
            , Action<CommandModel.MockExecutorFactory> mockFactoryCallback = null)
            where TService : ServiceBase, new()
        {
            Aoite.ServiceModel.ContractContext.NewUnitTestContext(user);
            var container = CreateCommandModelContainer();
            if(mockFactoryCallback != null)
            {
                var executorFactory = new Aoite.CommandModel.MockExecutorFactory(container);
                mockFactoryCallback(executorFactory);
                container.AddService<CommandModel.IExecutorFactory>(executorFactory);
            }
            var service = new TService();
            service._commandBus = new Lazy<CommandModel.ICommandBus>(() => container.GetService<CommandModel.ICommandBus>());
            return service;
        }

        /// <summary>
        /// 获取用于服务契约架构的全局命令模型服务容器。这个命令模型服务容器，不等于契约服务器的服务模型。
        /// </summary>
        public static IIocContainer GlobalContainer { get { return LazyContainer.Value; } }

        /// <summary>
        /// 获取当前线程中的上下文契约。
        /// </summary>
        public ContractContext Context { get { return ContractContext.Current; } }


        private Lazy<Aoite.CommandModel.ICommandBus> _commandBus = new Lazy<CommandModel.ICommandBus>(() => GlobalContainer.GetService<Aoite.CommandModel.ICommandBus>());
        /// <summary>
        /// 获取用于服务契约架构的命令模型服务容器。
        /// </summary>
        public IIocContainer Container { get { return this._commandBus.Value.Container; } }
        /// <summary>
        /// 获取当前已授权的登录用户。
        /// </summary>
        public dynamic User { get { return Context.User; } }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="success">执行完成后发生的回调方法。</param>
        /// <param name="error">执行抛出异常后发生的回调方法。</param>
        /// <returns>返回命令模型。</returns>
        protected virtual TCommand Execute<TCommand>(TCommand command
            , CommandModel.CommandExecutingHandler<TCommand> success = null
            , CommandModel.CommandExecuteExceptionHandler<TCommand> error = null) where TCommand : CommandModel.ICommand
        {
            return this._commandBus.Value.Execute(command, success, error);
        }

        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="success">执行完成后发生的回调方法。</param>
        /// <param name="error">执行抛出异常后发生的回调方法。</param>
        protected virtual void ExecuteAsync<TCommand>(TCommand command
            , CommandModel.CommandExecutingHandler<TCommand> success = null
            , CommandModel.CommandExecuteExceptionHandler<TCommand> error = null) where TCommand : CommandModel.ICommand
        {
            this._commandBus.Value.ExecuteAsync(command, success, error);
        }

        /// <summary>
        /// 获取当前服务的缓存提供程序。
        /// </summary>
        public Aoite.Cache.ICacheProvider CacheProvider { get { return this.Container.GetService<Aoite.Cache.ICacheProvider>(); } }

        /// <summary>
        /// 获取一个全局锁的功能，如果获取锁超时将会抛出异常。
        /// </summary>
        /// <param name="key">锁的键。</param>
        /// <param name="timeout">获取锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        protected virtual IDisposable AcquireLock(string key, TimeSpan? timeout = null)
        {
            return this.CacheProvider.Lock(key, timeout);
        }
        /// <summary>
        /// 提供批量锁的功能。
        /// </summary>
        /// <param name="keys">锁的键名列表。</param>
        /// <returns>返回一个批量锁。</returns>
        protected virtual IDisposable MultipleLock(params string[] keys)
        {
            return this.CacheProvider.MultipleLock(keys);
        }
        /// <summary>
        /// 提供批量锁的功能。
        /// </summary>
        /// <param name="timeout">锁的超时设定。</param>
        /// <param name="keys">锁的键名列表。</param>
        /// <returns>返回一个批量锁。</returns>
        protected virtual IDisposable MultipleLock(TimeSpan timeout, params string[] keys)
        {
            return this.CacheProvider.MultipleLock(timeout, keys);
        }

        /// <summary>
        /// 获取指定数据类型键的原子递增序列。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        protected virtual long Increment<T>(long? increment = null)
        {
            return this.Increment(typeof(T).Name, increment);
        }

        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        protected virtual long Increment(string key, long? increment = null)
        {
            return this.CacheProvider.Increment(key, increment);
        }

        /// <summary>
        /// 设置当前已授权的登录用户模型，当 <paramref name="user"/> 为 null 值时，表示注销用户权限。
        /// </summary>
        /// <param name="user">已授权的登录用户模型。</param>
        protected virtual void SetUser(object user)
        {
            this.Context.User = user;
        }

        void IServiceHandler.Calling(ContractContext context)
        {
            var source = context.Source;
            if(source != null)
            {
                object user = this.User;
                context.IsAuthenticated = user != null;
            }

            this.OnCalling(context);
        }
        void IServiceHandler.Called(ContractContext context) { this.OnCalled(context); }

        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected virtual void OnCalling(ContractContext context) { }
        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected virtual void OnCalled(ContractContext context) { }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public virtual void Dispose() { }
    }
}
