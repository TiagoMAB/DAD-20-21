using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class Script : Command
    {
        private readonly List<Command> commands = new List<Command>();

        public void AddCommand(Command command)
        {
            this.commands.Add(command);
        }

        public void Execute()
        {
            foreach(Command command in this.commands)
            {
                command.Execute();
            }
        }
    }
}
