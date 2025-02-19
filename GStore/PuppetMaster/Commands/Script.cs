﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Script {
        private readonly List<Command> commands = new List<Command>();
        private readonly List<Task> tasks = new List<Task>();

        public void AddTask(Task task) {
            this.tasks.Add(task);
        }

        private async Task Await() {
            await Task.WhenAll(this.tasks).ConfigureAwait(false);
        }

        public void AddCommand(Command command) {
            this.commands.Add(command);
        }

        public async Task Execute() {
            
            List<Command> toExecute = this.commands.FindAll(command => command.GetType().Name == "Server");
            foreach (Command command in toExecute)
            {
                Task task = command.Execute();
                Thread.Sleep(1500);

                if (task != null)
                {
                    this.AddTask(task);
                }
            }

            toExecute = this.commands.FindAll(command => command.GetType().Name == "Partition");
            foreach (Command command in toExecute)
            {
                Task task = command.Execute();

                if (task != null)
                {
                    this.AddTask(task);
                }
            }

            toExecute = this.commands.FindAll(command => (command.GetType().Name != "Partition" && command.GetType().Name != "Server"));
            foreach (Command command in toExecute) {
                Task task = command.Execute();

                if(task != null) {
                    this.AddTask(task);
                }
            }

            await this.Await().ConfigureAwait(false);
        }
    }
}
