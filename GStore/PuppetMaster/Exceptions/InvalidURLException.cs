using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Exceptions {
    class InvalidURLException : Exception {
        private readonly string URL;
        private readonly string command;

        public InvalidURLException(string command, string URL) {
            this.URL = URL;
            this.command = command;
        }

        public string Url => this.URL;

        public string Command => this.command;
    }
}
