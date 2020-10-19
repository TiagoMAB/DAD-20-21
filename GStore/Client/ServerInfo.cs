using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Linq;
using Grpc.Core;
using Client.Exceptions;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;

        readonly Dictionary<string, string> serverURL = new Dictionary<string, string>();                   // <serverId, URL>

        readonly Dictionary<string, string> masterURL = new Dictionary<string, string>();                   // <partitionId, URL>

        //TODO: maybe not needed
        readonly Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>();  // <URL, partitionIds>

        public string CurrentServerURL { get; set; }
        public bool ExecFinish { get; set; }
        public string UserName { get; set; }

        public static ServerInfo Instance()
        {
            if (serverInfo == null)
                return (serverInfo = new ServerInfo());
            else 
                return serverInfo;
        }

        public string GetMasterURLByPartitionId(string partitionId)
        {
            string value;
            if (masterURL.TryGetValue(partitionId, out value))
                return value;
            else return null;
        }
        public List<string> GetURLs()
        {
            return serverURL.Values.ToList();
        }
        public List<string> GetPartitionIds()
        {
            return masterURL.Keys.ToList();
        }
        public List<string> GetPartitionsByURL(string url)
        {
            List<string> value;
            if (serverReplicas.TryGetValue(url, out value))
                return value;
            else return null;
        }
        public string GetURLByServerId(string serverId)
        {
            string value;
            if (serverURL.TryGetValue(serverId, out value))
                return value;
            else return null;
        }

        public void AddServerURL(string serverId, string url)
        {
            serverURL.Add(serverId, url);
        }

        public void RegisterPartition(string name, string masterId, List<string> serverIds) {
            string url = GetURLByServerId(masterId);
            if (url == null)
                throw new NonExistentServerException(String.Format("Server with id {0} not found.", masterId));

            masterURL.Add(name, url);

            foreach (string serverId in serverIds) {
                url = GetURLByServerId(serverId);
                if (url == null)
                    throw new NonExistentServerException(String.Format("Server with id {0} not found.", serverId));

                List<string> partitionIds = GetPartitionsByURL(url);
                if (partitionIds == null)
                    serverReplicas.Add(url, new List<string>() { name } );
                else partitionIds.Add(name);
            }
        }

        public void GetServerInfo()
        {
            Random random = new Random();
            List<string> urls = serverURL.Values.ToList();

            //Drop current network information
            serverURL.Clear();
            masterURL.Clear();
            serverReplicas.Clear();

            while (true)
            {
                //Get random server from the list of known servers
                string url = urls[random.Next(urls.Count)];
                var channel = GrpcChannel.ForAddress(url);
                var client = new GStore.GStore.GStoreClient(channel);

                try
                {
                    //TODO: maybe add a variable for waiting time
                    var response = client.ServerInfo(
                        new GStore.ServerInfoRequest { },
                        //TODO: maybe use deadline: context.Deadline in server
                        deadline: DateTime.UtcNow.AddMilliseconds(5000));

                    foreach (var value in response.Servers)
                        serverURL.Add(value.Id, value.Url);

                    foreach (var value in response.Partition)
                        RegisterPartition(value.Name, value.Master, value.ServerIds.ToList());

                    CurrentServerURL = url;
                    return;
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server {0} exceeded response time.", url));
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.Unavailable)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server {0} not available.", url));
                }
            }
        }
    }
}
