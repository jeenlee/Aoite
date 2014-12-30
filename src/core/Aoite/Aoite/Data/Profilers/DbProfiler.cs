using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data.Profilers
{
    /// <summary>
    /// 一个查询监控器。
    /// </summary>
    public static class DbProfiler
    {
        private static List<DbEngine> _Engines = new List<DbEngine>();
        /// <summary>
        /// 获取已添加到监控集合的引擎列表。
        /// </summary>
        public static IEnumerable<DbEngine> Engines
        {
            get
            {
                return _Engines;
            }
        }

        private static Stack<ProfilerItem> _Items = new Stack<ProfilerItem>(100);
        /// <summary>
        /// 获取监控项列表。
        /// </summary>
        public static IEnumerable<ProfilerItem> Items
        {
            get { return _Items; }
        }

        /// <summary>
        /// 在引擎执行命令后发生。
        /// </summary>
        public static event ExecutedEventHandler Executed;
        /// <summary>
        /// 在引擎执行命令时发生。
        /// </summary>
        public static event ExecutingEventHandler Executing;

        private static int _MaxItemCount = 100;
        /// <summary>
        /// 获取或设置一个值，该值指示最大监控项的数量。默认为 100。
        /// </summary>
        public static int MaxItemCount
        {
            get
            {
                return _MaxItemCount;
            }
            set
            {
                _MaxItemCount = value;
            }
        }

        /// <summary>
        /// 添加一个引擎到监控器。
        /// </summary>
        /// <param name="engine">一个数据源查询与交互引擎的实例。</param>
        public static void Add(DbEngine engine)
        {
            lock(_Engines)
            {
                if(_Engines.Contains(engine)) return;
                _Engines.Add(engine);
            }
            engine.Executing += new ExecutingEventHandler(engine_Executing);
            engine.Executed += new ExecutedEventHandler(engine_Executed);
        }

        private static void RemoveEngine(DbEngine engine)
        {
            if(_Engines.Contains(engine))
            {
                engine.Executing -= engine_Executing;
                engine.Executed -= new ExecutedEventHandler(engine_Executed);
                _Engines.Remove(engine);

            }
        }

        /// <summary>
        /// 移除一个引擎到监控器。
        /// </summary>
        /// <param name="engine">一个数据源查询与交互引擎的实例。</param>
        public static void Remove(DbEngine engine)
        {
            lock(_Engines)
            {
                RemoveEngine(engine);
            }
        }

        /// <summary>
        /// 清空监控器。
        /// </summary>
        public static void Clear()
        {
            lock(_Engines)
            {
                while(_Engines.Count > 0)
                {
                    RemoveEngine(_Engines[0]);
                }
            }
        }

        static void engine_Executing(object sender, ExecutingEventArgs e)
        {
            e.ProfilerItem = new ProfilerItem();
            if(Executing != null) Executing.BeginInvoke(sender, e, null, null);
        }

        static void engine_Executed(object sender, ExecutedEventArgs e)
        {
            var item = e.ProfilerItem;
            if(item == null) return;
            item.Completed(e.ExecuteType, e.Result);
            lock(_Items)
            {
                while(_Items.Count >= _MaxItemCount) _Items.Pop();
                _Items.Push(item);
            }
            if(Executed != null) Executed.BeginInvoke(item, e, null, null);
        }
    }
}
