using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Script {
        private readonly List<Command> commands = new List<Command>();
        private readonly List<Task> tasks = new List<Task>();

        public void AddTask(Task task) {
            this.tasks.Insert(0, task);
        }

        private void Await() {
            while (this.tasks.Count > 0) {
                if (this.tasks.Count == 1) {
                    this.tasks[0].Wait();
                    this.tasks.Clear();
                    break;
                }

                for (int i = this.tasks.Count - 1; i >= 0; i--) {
                    if (this.tasks[i].IsCompleted) {
                        System.Diagnostics.Debug.WriteLine("Completed");
                        this.tasks[i].Wait();
                        this.tasks.RemoveAt(i);
                    }
                }
            }
        }

        public void AddCommand(Command command) {
            this.commands.Add(command);
        }

        public void Execute() {
            foreach (Command command in this.commands) {
                Task task = command.Execute();

                if(task != null) {
                    this.AddTask(task);
                }
            }

            this.Await();
        }
    }
}
