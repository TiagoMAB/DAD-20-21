using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Freeze : Command {
        private readonly string id;

        public Freeze(string id) {
            this.id = id;
        }
        protected override void DoWork() {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Freeze {0}", id));
        }
    }
}
