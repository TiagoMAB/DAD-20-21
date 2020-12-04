using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Linq;
using Grpc.Core;
using Client.Exceptions;
using GStore;

namespace Client
{
    sealed class ServerInfo
    {
        static ServerInfo serverInfo = null;

        public readonly int backoffTime = 5000;

        public readonly int numOfRetries = 5;

        readonly Dictionary<string, string> serverURL = new Dictionary<string, string>();                                       // <serverId, URL>

        readonly Dictionary<string, List<string>> serverReplicas = new Dictionary<string, List<string>>();                      // <URL, partitionIds>

        Dictionary<string, GStore.GStore.GStoreClient> channelClient = new Dictionary<string, GStore.GStore.GStoreClient>();    // <URL, channelClient>

        Dictionary<string, GrpcChannel> channels = new Dictionary<string, GrpcChannel>();                                       // <URL, channel>

        Dictionary<string, int> partitionTimestamp = new Dictionary<string, int>();                                             // <partitionId, timestamp>

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

        public List<string> GetURLs()
        {
            return serverURL.Values.ToList();
        }
        public List<string> GetPartitionsByURL(string url)
        {
            List<string> value;
            if (url != null && serverReplicas.TryGetValue(url, out value))
                return value;
            else return null;
        }
        public string GetURLByServerId(string serverId)
        {
            string value;
            if (serverId != null && serverURL.TryGetValue(serverId, out value))
                return value;
            else return null;
        }
        public GStore.GStore.GStoreClient GetChannel(string url)
        {
            GStore.GStore.GStoreClient client;

            CurrentServerURL = url;

            if (url != null && channelClient.TryGetValue(url, out client))
                return client;

            GrpcChannel channel = GrpcChannel.ForAddress(CurrentServerURL);
            channels.Add(CurrentServerURL, channel);
            client = new GStore.GStore.GStoreClient(channel);
            channelClient.Add(CurrentServerURL, client);

            return client;
        }

        public void AddServerURL(string serverId, string url)
        {
            serverURL.Add(serverId, url);
        }
        public List<string> GetURLsWithPartitionId(string partitionId)
        {
            List<string> urls = new List<string>();

            foreach (KeyValuePair<string, List<string>> entry in serverReplicas)
                if (entry.Value.Contains(partitionId))
                    urls.Add(entry.Key);

            return urls;
        }

        public void RegisterPartition(string name, List<string> serverIds)
        {
            foreach (string serverId in serverIds) {
                string url = GetURLByServerId(serverId);
                if (url == null)
                    throw new NonExistentServerException(String.Format("Server with id \"{0}\" not found.", serverId));

                List<string> partitionIds = GetPartitionsByURL(url);
                if (partitionIds == null)
                    serverReplicas.Add(url, new List<string>() { name });
                else partitionIds.Add(name);
            }

            if (!partitionTimestamp.ContainsKey(name))
                partitionTimestamp.Add(name, 0);
        }

        private void ClearChannels()
        {
            channelClient.Clear();
            foreach (GrpcChannel channel in channels.Values.ToList())
                channel.ShutdownAsync().Wait();
            channels.Clear();
        }

        public void updatePartitionTimestamp(string partitionId, int serverTimestamp)
        {
            if (partitionTimestamp.TryGetValue(partitionId, out int partTimestamp))
                if (partTimestamp < serverTimestamp)
                    partitionTimestamp[partitionId] = serverTimestamp;
        }

        public bool IsNewerTimestamp(string partitionId, int serverTimestamp)
        {
            if (partitionTimestamp.TryGetValue(partitionId, out int partTimestamp))
                if (serverTimestamp >= partTimestamp)
                    return true;
            return false;
        }

        public void GetServerInfo()
        {
            Random random = new Random();
            List<string> urls = serverURL.Values.ToList();

            //Drop current network information
            serverURL.Clear();
            serverReplicas.Clear();
            ClearChannels();

            foreach (int retries in Enumerable.Range(1, numOfRetries).Reverse())
            {
                //Get random server from the list of known servers
                string url = urls[random.Next(urls.Count)];

                Console.WriteLine("Contacting server with URL \"{0}\" for info about the network...", url);

                try
                {
                    GStore.GStore.GStoreClient client = GetChannel(url);

                    var response = client.ServerInfo(new ServerInfoRequest { });

                    foreach (var value in response.Servers)
                        serverURL.Add(value.Id, value.Url);

                    foreach (var value in response.Partition)
                        RegisterPartition(value.Name, value.ServerIds.ToList());

                    Console.WriteLine("Information about the network obtained.\n");
                    return;
                }
                catch (RpcException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Server with URL \"{0}\" failed with status \"{1}\".\nRetrying {2} more time(s) in {3}ms...", url, e.StatusCode.ToString(), retries, backoffTime));
                    Console.WriteLine("Server with URL \"{0}\" failed with status \"{1}\". Retrying {2} more time(s) in {3}ms...\n", url, e.StatusCode.ToString(), retries, backoffTime);
                    System.Threading.Thread.Sleep(backoffTime);
                }
            }

            if (serverURL.Count == 0)
                throw new NonExistentServerException("No connection with server was reached.");
        }
    }
}
