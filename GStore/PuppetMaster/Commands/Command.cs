using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public abstract class Command {
        private delegate void Logger(string s);
        private readonly Logger logger;
        private readonly PuppetMaster form;

        protected Command(PuppetMaster form) {
            this.logger = new Logger(form.Log);
            this.form = form;
        }

        public async virtual Task Execute() {
            await Task.Run(DoWork).ConfigureAwait(false);
        }
        protected abstract void DoWork();

        protected void Log(string s) {
            this.form.Invoke(this.logger, s);
        }
    }
}
