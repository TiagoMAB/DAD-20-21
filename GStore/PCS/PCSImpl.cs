using Grpc.Core;
using GStore;
using System;
using System.Threading.Tasks;

namespace PCS
{
    public class PCSImpl : GStore.PCS.PCSBase
    {
        public override Task<ClientResponse> Client(ClientRequest request, ServerCallContext context)
        {
            Console.WriteLine(String.Format("Request to launch client with script file '{0}'", request.Script));

            return Task.FromResult(new ClientResponse());
        }

        public override Task<ServerResponse> Server(ServerRequest request, ServerCallContext context)
        {
            Console.WriteLine(String.Format("Request to launch server with id '{0}', min delay of {1}ms and max delay of {2}ms", request.Id, request.MinDelay, request.MaxDelay));

            return Task.FromResult(new ServerResponse());
        }
    }
}
