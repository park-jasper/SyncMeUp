using System;
using System.Linq;
using System.Windows.Input;

namespace SyncMeUp.Domain.Commands
{
    public class CommandComposition : CommandBase
    {
        private readonly ICommand[] _commands;

        public CommandComposition(params ICommand[] commands)
        {
            _commands = new ICommand[commands.Length];
            Array.Copy(commands, _commands, commands.Length);
        }

        public override bool CanExecute(object parameter)
        {
            return _commands.All(comm => comm.CanExecute(parameter));
        }

        public override void Execute(object parameter)
        {
            foreach (var command in _commands)
            {
                command.Execute(parameter);
            }
        }
    }
}