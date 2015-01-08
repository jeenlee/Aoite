using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个执行查询的命令。
    /// </summary>
    [Serializable]
    public class ExecuteCommand : ObjectDisposableBase, ICloneable
    {
        /// <summary>
        /// 获取一个值，表示执行的查询是否已被用户终止。
        /// </summary>
        internal bool Aborted;
        [NonSerialized]
        private ExecutedEventArgs _EventArgs;

        internal ExecutedEventArgs GetEventArgs(ExecuteType type, IDbResult result)
        {
            var e = this._EventArgs ?? (this._EventArgs = new ExecutedEventArgs(this));
            e.ExecuteType = type;
            e.Result = result;
            return e;
        }

        #region Properties

        [NonSerialized]
        private object _UserToken;
        /// <summary>
        /// 获取或设置用户自定义数据。
        /// </summary>
        [Ignore]
        public object UserToken { get { return this._UserToken; } set { this._UserToken = value; } }

        [NonSerialized]
        private WeakReference _RuntimeObject;
        /// <summary>
        /// 获取命令运行时的对象。
        /// </summary>
        [Ignore]
        public object RuntimeObject { get { return this._RuntimeObject != null && this._RuntimeObject.IsAlive ? this._RuntimeObject.Target : null; } }

        /// <summary>
        /// 获取或设置一个值，表示在终止执行命令的尝试并生成错误之前的等待时间。
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// 获取或设置命令字符串的类型。如果设置此值，将会强制查询命令的类型。
        /// </summary>
        public CommandType? CommandType { get; set; }

        private string _CommandText;
        /// <summary>
        /// 获取或设置一个值，表示查询命令的 Transact-SQL 查询字符串。
        /// </summary>
        public string CommandText { get { return this._CommandText; } set { this._CommandText = value; } }

        private ExecuteParameterCollection _Parameters;
        /// <summary>
        /// 获取查询命令的参数的键值集合。
        /// </summary>
        public ExecuteParameterCollection Parameters { get { return this._Parameters ?? (this._Parameters = new ExecuteParameterCollection()); } }

        /// <summary>
        /// 获取查询命令的参数的键值集合的元素数。
        /// </summary>
        public int Count { get { return this._Parameters == null ? 0 : this._Parameters.Count; } }

        /// <summary>
        /// 获取指定参数索引的参数内容。
        /// </summary>
        /// <param name="index">参数索引。</param>
        /// <returns>获取一个 <see cref="Aoite.Data.ExecuteParameter"/> 的参数实例。</returns>
        public ExecuteParameter this[int index]
        {
            get
            {
                return this.Parameters[index];
            }
        }

        /// <summary>
        /// 获取或设置指定参数名称的参数内容。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>获取一个 <see cref="Aoite.Data.ExecuteParameter"/> 的参数实例。</returns>
        public ExecuteParameter this[string name]
        {
            get
            {
                return this.Parameters[name];
            }
            set
            {
                this.Parameters[name] = value;
            }
        }

        #endregion

        #region Cotrs

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        /// </summary>
        public ExecuteCommand() : this(null, (ExecuteParameterCollection)null) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        public ExecuteCommand(string commandText) : this(commandText, (ExecuteParameterCollection)null) { }

        /// <summary>
        /// 指定查询字符串和匿名参数集合实例，初始化一个 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public ExecuteCommand(string commandText, object objectInstance)
            : this(commandText, new ExecuteParameterCollection(objectInstance)) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        public ExecuteCommand(string commandText, params object[] parameters)
            : this(commandText, new ExecuteParameterCollection(parameters)) { }

        /// <summary>
        /// 指定查询字符串和查询参数，初始化一个 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        public ExecuteCommand(string commandText, params ExecuteParameter[] parameters)
            : this(commandText, new ExecuteParameterCollection(parameters)) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        public ExecuteCommand(string commandText, ExecuteParameterCollection parameters)
        {
            this._CommandText = commandText;
            this._Parameters = parameters;
        }

        #endregion

        /// <summary>
        /// 强制中断正在执行的命令。
        /// </summary>
        /// <returns>返回中断的结果。</returns>
        public Result AbortExecuting()
        {
            DbCommand dbCommand = this.RuntimeObject as DbCommand;
            if(dbCommand == null || dbCommand.Connection == null)
                return "这是一个尚未开始的命令。";
            try
            {
                using(dbCommand)
                {
                    if(dbCommand.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        this.Aborted = true;
                        dbCommand.Cancel();
                        //using(dbCommand.Connection) connection.TryClose();
                        //Console.WriteLine("connection.State : {0}", connection.State);
                    }
                }
            }
            catch(Exception e) { return e; }
            return Result.Successfully;
        }

        /// <summary>
        /// 设置命令运行时的对象。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="runtimeObject">命令运行时的对象。</param>
        public virtual void SetRuntimeObject(IDbEngine engine, object runtimeObject)
        {
            this._RuntimeObject = new WeakReference(runtimeObject);
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this._CommandText = null;
            if(this._Parameters != null) this._Parameters.Clear();
            this._Parameters = null;
        }

        /// <summary> 
        /// 返回当前查询命令的字符串形式（Transact-SQL 查询语句）。
        /// </summary>
        public override string ToString()
        {
            return this._CommandText;
        }

        ///// <summary>
        ///// 将字符串隐式转换为 <see cref="Aoite.Data.ExecuteCommand"/> 类的新实例。
        ///// </summary>
        ///// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        //public static implicit operator ExecuteCommand(string commandText)
        //{
        //    return new ExecuteCommand(commandText);
        //}

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        public object Clone()
        {
            ExecuteParameterCollection newParameters = null;
            if(this._Parameters != null)
            {
                newParameters = new ExecuteParameterCollection(this._Parameters.Count);
                foreach(var p in this._Parameters)
                {
                    newParameters.Add(p.Clone() as ExecuteParameter);
                }
            }
            return new ExecuteCommand(this._CommandText, newParameters);
        }
    }
}
