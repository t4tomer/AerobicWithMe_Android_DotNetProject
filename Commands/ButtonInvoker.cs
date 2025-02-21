using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AerobicWithMe.Commands;
using AerobicWithMe.Interfaces;
using AerobicWithMe.Commands;



namespace AerobicWithMe.Commands
{
    public class ButtonInvoker
    {
        private I_Command _command;

        public void SetCommand(I_Command command)
        {
            _command = command;
        }

        public async Task PressButton()
        {
            if (_command != null)
                await _command.Execute();
        }
    }
}
