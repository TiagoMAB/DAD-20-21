using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    public class Client : Command
    {
        private readonly string username;
        private readonly string URL;
        private readonly string file;

        public Client(string username, string URL, string file)
        {
            this.username = username;
            this.URL      = URL;
            this.file     = file;
        }

        public void Execute()
        {
            // TODO: Implement
            System.Diagnostics.Debug.WriteLine(String.Format("Client {0} at {1} using file {2}", this.username, this.URL, this.file));
        }
    }
}
