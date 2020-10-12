using System;
using System.Collections.Generic;

namespace Client.Commands
{
    class BeginRepeat : Command
    {
        private readonly int repeatTimes;
        private readonly List<Command> commands = new List<Command>();

        public BeginRepeat(int repeatTimes)
        {
            this.repeatTimes = repeatTimes;
        }

        public void AddCommand(Command command)
        {
            this.commands.Add(command);
        }

        public void Execute()
        {
            int i = 0;
            while (i++ != repeatTimes)
            {
                foreach (Command command in this.commands)
                {
                    command.Execute();
                }
            }
        }
    }
}
