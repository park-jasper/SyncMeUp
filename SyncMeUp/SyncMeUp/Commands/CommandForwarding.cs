using System;

namespace SyncMeUp.Commands
{
    public class CommandForwarding : CommandBase
    {
        private readonly Action<object> _executeAction;
        private readonly Func<object, bool> _canExecuteFunc;
        public override bool CanExecute(object parameter)
        {
            return _canExecuteFunc(parameter);
        }

        public override void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        public CommandForwarding(Action<object> executeAction, Func<object, bool> canExecuteFunc = null)
        {
            _executeAction = executeAction;
            _canExecuteFunc = canExecuteFunc ?? ((obj) => true);
        }

        public void OnCanExecuteChanged() => RaiseCanExecuteChangedEvent();
    }
}