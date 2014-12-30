using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示映射器值的事件参数。
    /// </summary>
    public class MapperValueEventArgs : EventArgs
    {
        private IDbEngine _Engine;
        /// <summary>
        /// 获取数据源查询与交互引擎的实例。
        /// </summary>
        public IDbEngine Engine { get { return this._Engine; } }

        private StringBuilder _Builder;
        /// <summary>
        /// 获取当前查询语句缓冲区。
        /// </summary>
        public StringBuilder Builder { get { return this._Builder; } }

        private DefaultParameterSettings _ParameterSettings;
        /// <summary>
        /// 获取参数的配置信息。
        /// </summary>
        public DefaultParameterSettings ParameterSettings { get { return this._ParameterSettings; } }

        private ExecuteParameterCollection _Parameters;
        /// <summary>
        /// 获取查询参数的集合。
        /// </summary>
        public ExecuteParameterCollection Parameters { get { return this._Parameters; } }

        private object _Entity;
        /// <summary>
        /// 获取当前的实体对象。
        /// </summary>
        public object Entity { get { return this._Entity; } }

        private MapperRuntime _Runtime;
        /// <summary>
        /// 获取映射器的运行时。
        /// </summary>
        public MapperRuntime Runtime { get { return this._Runtime; } }

        internal MapperValueEventArgs(IDbEngine engine
            , StringBuilder builder
            , DefaultParameterSettings parameterSettings
            , ExecuteParameterCollection parameters
            , object entity
            , MapperRuntime runtime)
        {
            this._Engine = engine;
            this._Builder = builder;
            this._Parameters = parameters;
            this._ParameterSettings = parameterSettings;
            this._Entity = entity;
            this._Runtime = runtime;
        }
    }

    /// <summary>
    /// 表示映射器值的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void MapperValueEventHandler(object sender, MapperValueEventArgs e);

}
