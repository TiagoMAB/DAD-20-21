using System;

namespace PuppetMaster.Commands
{
    public class Server : Command
    {
        private readonly string id;
        private readonly string URL;
        private readonly int minDelay;
        private readonly int maxDelay;

        public Server(string id, string URL, int minDelay, int maxDelay)
        {
            this.id       = id;
            this.URL      = URL;
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
        }

        public void Execute()
        {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Server {0} at {1} delaying between {2} and {3} ms", this.id, this.URL, this.minDelay, this.maxDelay));
        }
    }
}
