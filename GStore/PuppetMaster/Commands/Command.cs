using System.Threading.Tasks;

namespace PuppetMaster.Commands {
    public abstract class Command {
        public virtual Task Execute() {
            return Task.Run(DoWork);
        }
        protected abstract void DoWork();
    }
}
