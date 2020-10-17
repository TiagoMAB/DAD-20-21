using GStore;
using System.Threading.Tasks;

namespace PuppetMaster {
    public class StatusImpl : Status.StatusBase {
        private delegate void Logger(string s);
        private readonly PuppetMaster form;
        private readonly Logger logger;

        public static string PrettyStatus(StatusInfo status) {
            // TODO: Pretty print

            return status.ToString();
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
