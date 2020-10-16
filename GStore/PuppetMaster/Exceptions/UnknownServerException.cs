using System;

namespace PuppetMaster.Exceptions {
    class UnknownServerException : Exception {
        private readonly string id;
        private readonly string command;

        public UnknownServerException(string command, string id) {
            this.id      = id;
            this.command = command;
        }

        public string Id => this.id;

        public string Command => this.command;
    }
}
