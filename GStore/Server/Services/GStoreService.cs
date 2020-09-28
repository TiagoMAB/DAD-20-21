using System.Threading.Tasks;
using Gstore;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class GStoreService : GStore.GStoreBase
    {
        private readonly ILogger _logger;

        public GStoreService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GStoreService>();
        }

        public override Task<WriteReply> write(WriteRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Write");
            return Task.FromResult(new WriteReply());
        }

        public override Task<ReadReply> read(ReadRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Read");
            return Task.FromResult(new ReadReply());
        }
    }
}