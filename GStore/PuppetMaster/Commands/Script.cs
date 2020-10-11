using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Script {
        private readonly List<Command> commands = new List<Command>();
        private readonly List<Task> tasks = new List<Task>();

        public void AddTask(Task task) {
            this.tasks.Add(task);
        }

        private async Task Await() {
            await Task.WhenAll(this.tasks);
        }

        public void AddCommand(Command command) {
            this.commands.Add(command);
        }

        public async Task Execute() {
            foreach (Command command in this.commands) {
                Task task = command.Execute();

                if(task != null) {
                    this.AddTask(task);
                }
            }

            await this.Await().ConfigureAwait(false);
        }
    }
}
