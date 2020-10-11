using System;
using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public class Freeze : Command {
        private readonly string id;

        public Freeze(PuppetMaster form, string id) : base(form) {
            this.id = id;
        }
        protected override void DoWork() {
            // TODO: Implement
            Log(String.Format("Freezed server '{0}'", this.id));
        }
    }
}
