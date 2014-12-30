using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示数据源查询操作的返回结果。
    /// </summary>
    /// <typeparam name="TValue">返回值的类型。</typeparam>
    public class DbResult<TValue> : Result<TValue>, IDbResult<TValue>
    {
        #region Properties

        private DbCommand _Command;
        /// <summary>
        /// 获取一个值，表示查询操作的 <see cref="System.Data.Common.DbCommand"/> 对象。
        /// </summary>
        public DbCommand Command { get { return this._Command; } }

        private bool _IsDisposed;
        /// <summary>
        /// 获取一个值，表示当前查询结果是否已释放资源。
        /// </summary>
        bool IDbResult.IsDisposed { get { return this._IsDisposed; } }

        private IDbEngine _Engine;
        /// <summary>
        /// 获取一个值，表示数据源的操作引擎。
        /// </summary>
        public IDbEngine Engine { get { return this._Engine; } }

        object IDbResult.Value { get { return this._Value; } }

        /// <summary>
        /// 指定参数的名称，获取一个参数的值。
        /// </summary>
        /// <param name="parameterName">参数的名称。可以不指定参数名称的前缀（如“@”、“:”之类）。</param>
        /// <returns>返回具有指定名称的参数值。</returns>
        public object this[string parameterName]
        {
            get
            {
                var name = parameterName;
                var prefix = this._Engine.Owner.Injector.ParameterSettings.PrefixWithCollection;
                if(name != null && _Engine != null && !name.StartsWith(prefix))
                {
                    name = prefix + name;
                }
                return this._Command.Parameters[name].Value;
            }
        }

        /// <summary>
        /// 指定参数的索引，获取一个参数的值。
        /// </summary>
        /// <param name="parameterIndex">参数的从零开始的索引。</param>
        /// <returns>返回索引处的参数值。</returns>
        public object this[int parameterIndex]
        {
            get { return this._Command.Parameters[parameterIndex].Value; }
        }

        #endregion

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.DbResult&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        public DbResult() { }

        /// <summary>
        /// 初始化查询操作的返回结果。
        /// </summary>
        /// <param name="dbEngine">数据源的操作引擎。</param>
        /// <param name="command">查询操作的 <see cref="System.Data.Common.DbCommand"/> 对象。</param>
        /// <param name="value">查询操作的返回值。</param>
        /// <param name="exception">查询操作的引发的异常。</param>
        internal virtual void Initialization(IDbEngine dbEngine, DbCommand command, TValue value, Exception exception)
        {
            this._Engine = dbEngine;
            this._Command = command;
            if(exception == null)
                this._Value = value;
            this.ToFailded(exception);
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(this._IsDisposed) return;
            this._IsDisposed = true;
            if(disposing) this.OnDispose();
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void OnDispose()
        {
            if(this._Value is IDisposable) (this._Value as IDisposable).Dispose();
            this.ToSuccessed();
            if(this._Command != null) this._Command = null;
            if(this._Value != null) this._Value = default(TValue);
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~DbResult()
        {
            this.Dispose(false);
        }
    }

    /// <summary>
    /// 表示数据源查询操作的返回结果。
    /// </summary>
    public class DbResult : DbResult<VoidValue>
    {
        /// <summary>
        /// 该值没有任何意义。
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("该值没有任何意义。", true)]
        public new VoidValue Value { set { } }
    }
}
