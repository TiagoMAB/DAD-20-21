﻿using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public abstract class Command {
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
