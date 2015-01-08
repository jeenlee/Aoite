using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示数据源查询与交互引擎的管理器。
    /// </summary>
    public class DbEngineManager : System.Collections.Specialized.NameObjectCollectionBase
        , IEnumerable<DbEngine>
    {
        #region Executing & Executed

        /// <summary>
        /// 在引擎执行命令后发生。
        /// </summary>
        public event ExecutedEventHandler Executed;
        /// <summary>
        /// 在引擎执行命令时发生。
        /// </summary>
        public event ExecutingEventHandler Executing;
        internal void InternalOnExecuting(IDbEngine engine, ExecuteType type, ExecuteCommand command)
        {
            this.OnExecuting(engine, type, command);
        }

        /// <summary>
        /// 表示 <see cref="Aoite.Data.DbEngineManager.Executing"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="command">执行的命令。</param>
        protected virtual void OnExecuting(IDbEngine engine, ExecuteType type, ExecuteCommand command)
        {
            var handler = this.Executing;
            if(handler != null) handler(engine, command.GetEventArgs(type, null));
        }

        internal void InternalOnExecuted(IDbEngine engine, ExecuteType type, ExecuteCommand command, IDbResult result)
        {
            this.OnExecuted(engine, type, command, result);
        }
        /// <summary>
        /// 表示 <see cref="Aoite.Data.DbEngineManager.Executed"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="result">操作的返回值。</param>
        /// <param name="command">执行的命令。</param>
        protected virtual void OnExecuted(IDbEngine engine, ExecuteType type, ExecuteCommand command, IDbResult result)
        {
            var handler = this.Executed;
            if(handler != null) handler(engine, command.GetEventArgs(type, result));
        }

        #endregion

        /// <summary>
        /// 获取指定标识的引擎。
        /// </summary>
        /// <param name="name">引擎的标识名称。</param>
        public DbEngine this[string name] { get { return (DbEngine)base.BaseGet(name); } }

        /// <summary>
        /// 给定唯一标识符和对象，添加到集合中。
        /// </summary>
        /// <param name="name">唯一标识符。</param>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>如果集合已存在该唯一标识符，将会返回 false，否则返回 true。</returns>
        public void Add(string name, DbEngine engine)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if(engine == null) throw new ArgumentNullException("engine");
            engine._Name = name;
            engine._Manager = this;
            base.BaseSet(name, engine);
        }

        /// <summary>
        /// 删除指定标识的引擎。
        /// </summary>
        /// <param name="name">引擎的标识。</param>
        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        /// <summary>
        /// 清空所有引擎。
        /// </summary>
        public void Clear()
        {
            base.BaseClear();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>可用于循环访问集合的枚举器。</returns>
        public new IEnumerator<DbEngine> GetEnumerator()
        {
            var e = base.GetEnumerator();
            while(e.MoveNext()) yield return (DbEngine)e.Current;
        }
    }
}
