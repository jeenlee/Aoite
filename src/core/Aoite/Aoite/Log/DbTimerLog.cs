using System;
using System.Collections.Generic;
using System.Text;
using Aoite.Data;

namespace Aoite.Log
{
    /// <summary>
    /// 基于数据库提供程序的定时器的日志提供程序。
    /// </summary>
    public class DbTimerLog : TimerLog
    {
        private string _TableName;
        /// <summary>
        /// 获取类型 <see cref="System.LogEntry"/> 在日志的数据库中的表名称。默认为 null 值，表示为默认的表名。
        /// </summary>
        public string TableName { get { return this._TableName; } set { this._TableName = value; } }

        private DbEngine _LogEngine;
        /// <summary>
        /// 获取日志的数据库操作引擎的实例。
        /// </summary>
        public DbEngine LogEngine
        {
            get
            {
                return this._LogEngine;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Log.DbTimerLog"/> 类的新实例。
        /// </summary>
        /// <param name="engine">日志引擎。</param>
        /// <param name="tableName">日志的数据库中的表名称。</param>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）。</param>
        public DbTimerLog(DbEngine engine, string tableName, double interval)
            : base(interval)
        {
            this._LogEngine = engine;
            this._TableName = tableName;
        }

        /// <summary>
        /// 将指定对象集合批量写入日志。
        /// </summary>
        /// <param name="descs">日志对象的集合。</param>
        protected override void Write(ILogEntry[] descs)
        {
            using(var context = _LogEngine.ContextTransaction)
            {
                foreach(var item in descs)
                {
                    if(item is LogEntry) _LogEngine.Add(item, this._TableName).ThrowIfFailded();
                    else _LogEngine.Add(item).ThrowIfFailded();
                }
                context.Commit().ThrowIfFailded();
            }
        }
    }
}
