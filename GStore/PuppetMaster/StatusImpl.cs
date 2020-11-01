using GStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppetMaster {
    public class StatusImpl : Status.StatusBase {
        private delegate void Logger(string s);
        private readonly PuppetMaster form;
        private readonly Logger logger;

        public static string PrettyStatus(StatusInfo status) {
            if (status.StatusCase == StatusInfo.StatusOneofCase.Client) {
                string response = String.Format("Status of client '{0}':", status.Id);

                response += status.Client.IsProcessComplete ? "\nFinished script execution" : "\nScript execution not finished yet";

                return response;
            } else if (status.StatusCase == StatusInfo.StatusOneofCase.Server) {
                string response = String.Format("Status of client '{0}':", status.Id);

                response += "\nKnown servers:";

                foreach(KeyValuePair<string, string> server in status.Server.Servers) {
                    response += String.Format("\nId: {0} URL: {1}", server.Key, server.Value);
                }

                response += "\nPartitions:";

                foreach(string partition in status.Server.Partitions) {
                    response += partition;
                }

                return response;
            } else {
                // SHOULD NEVER HAPPEN
                // BUT WHO KNOWS?
                throw new NotImplementedException();
            }
        }

        public StatusImpl(PuppetMaster form) {
            this.form = form;
            this.logger = new Logger(form.Log);
        }

        public override Task<StatusResponse> Status(StatusInfo request, Grpc.Core.ServerCallContext context) {
            LogStatus(request);

            // TODO: Handle error

            return Task.FromResult(new StatusResponse());
        }

        public void LogStatus(StatusInfo status) {
            this.form.Invoke(this.logger, PrettyStatus(status));
        }
    }
}
