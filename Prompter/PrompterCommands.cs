using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prompter
{
    public class NextCommand : ICommand
    {
        public NextCommand(Prompter prompter)
        {
            Prompter = prompter;
        }

        public Prompter Prompter { get; set; }
        
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Prompter.NextPrompt();
        }
    }

    public class BackCommand : ICommand
    {
        public BackCommand(Prompter prompter)
        {
            Prompter = prompter;
        }

        public Prompter Prompter { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Prompter.PreviousPrompt();
        }
    }
}
