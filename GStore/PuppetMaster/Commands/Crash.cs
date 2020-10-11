using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Crash : Command {
        private readonly string id;

        public Crash(string id) {
            this.id = id;
        }

        protected override void DoWork() {
            // TODO: implement
            System.Diagnostics.Debug.WriteLine(String.Format("Crash {0}", this.id));
        }
    }
}
