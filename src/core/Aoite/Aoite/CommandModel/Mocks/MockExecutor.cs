using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    class MockExecutor<TCommand> : IExecutor<TCommand> where TCommand : ICommand
    {
        private CommandExecutedHandler<TCommand> _handler;
        public MockExecutor(CommandExecutedHandler<TCommand> handler)
        {
            this._handler = handler;
        }
        public void Execute(IContext context, TCommand command)
        {
            this._handler(context, command);
        }
    }
}
