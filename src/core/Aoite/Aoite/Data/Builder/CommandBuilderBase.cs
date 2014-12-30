using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    internal abstract class CommandBuilderBase
    {
        internal IDbEngine _engine;
        internal CommandBuilderBase(IDbEngine engine)
        {
            this._engine = engine;
        }
        public abstract ExecuteParameterCollection Parameters { get; }
        public abstract string CommandText { get; }
        public IDbExecutor Execute()
        {
            return this._engine.Execute(this.End());
        }
        public abstract ExecuteCommand End();
    }

}
