using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public abstract class Command {
        protected const int TRIES = 4; // 1 normal and 3 backoffs
        protected const int MIN_BACKOFF = 2000;
        protected const int MAX_BACKOFF = 5000;

        private delegate void Logger(string s);
        private readonly Logger logger;
        protected readonly PuppetMaster form;

        protected Command(PuppetMaster form) {
            this.logger = new Logger(form.Log);
            this.form = form;
        }

        public virtual async Task Execute() {
            await DoWork().ConfigureAwait(false);
        }
        protected abstract Task DoWork();

        protected void Log(string s) {
            this.form.Invoke(this.logger, s);
        }
    }
}
