using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个事务。
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 提交事务。
        /// </summary>
        void Commit();
    }

    /// <summary>
    /// 表示一个实现基础服务的功能。
    /// </summary>
    public abstract class DefaultServiceBase : Aoite.ServiceModel.ServiceBase, IDbEngineExtendService
    {
        private DbEngineManager _Manager = Db.Manager;
        /// <summary>
        /// 获取或设置当前运行环境的数据库操作引擎集合管理器。
        /// </summary>
        public DbEngineManager Manager
        {
            get { return this._Manager; }
            set
            {
                this._Manager = value;
                this._Engine = value["Default"];
                this._Readonly = value["Readonly"];
            }
        }

        private DbEngine _Engine = Db.Engine;
        /// <summary>
        /// 获取当前运行环境的数据库操作引擎的实例。
        /// </summary>
        public DbEngine Engine { get { return this._Engine; } }

        private DbEngine _Readonly = Db.Readonly;
        /// <summary>
        /// 获取当前运行环境的数据库操作引擎的只读实例。
        /// </summary>
        public DbEngine Readonly { get { return this._Readonly; } }

        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public bool IsThreadContext { get { return this._Engine != null && this._Engine.IsThreadContext; } }

        /// <summary>
        /// 初始化一个 <see cref="System.DefaultServiceBase"/> 类的新实例。
        /// </summary>
        public DefaultServiceBase() { }

        class DefaultTransaction : ITransaction
        {
            private System.Transactions.TransactionScope _t = new System.Transactions.TransactionScope();

            void ITransaction.Commit()
            {
                _t.Complete();
            }

            void IDisposable.Dispose()
            {
                _t.Dispose();
            }
        }
        /// <summary>
        /// 开始事务模式。
        /// </summary>
        /// <returns>返回一个事务。</returns>
        protected ITransaction BeginTransaction()
        {
            return new DefaultTransaction();
        }

        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected override void OnCalled(Aoite.ServiceModel.ContractContext context)
        {
            base.OnCalled(context);
            if(this.IsThreadContext) this.Engine.Context.Dispose();
        }

    }

    /// <summary>
    /// 表示一个实现基础服务的功能。
    /// </summary>
    /// <typeparam name="TUserModel">用户模型的数据类型。</typeparam>
    public abstract class DefaultServiceBase<TUserModel> : DefaultServiceBase
        , IIdentityExtendService<TUserModel>
    {
        /// <summary>
        /// 获取当前已授权的登录用户。
        /// </summary>
        new public TUserModel User { get { return (TUserModel)base.User; } }
    }
}
