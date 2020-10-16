using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System;

namespace PuppetMaster {
    static class ConnectionInfo {
        private static readonly ConcurrentDictionary<string, string> clients = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> servers = new ConcurrentDictionary<string, string>();

        public static void AddClient(string name, string url) {
            clients.TryAdd(name, url);
        }

        public static void AddServer(string name, string url) {
            servers.TryAdd(name, url);
        }

        public static string GetRandomServer() {
            Random r = new Random();
            int i = r.Next(0, servers.Count);

            foreach(var server in servers) {
                if(i == 0) {
                    return server.Value;
                }

                i--;
            }

            return null;
        }

        public static string GetServer(string name) {
            string value = servers.GetOrAdd(name, "");

            if (value.Trim().Equals("")) {
                value = null;
            }

            return value;
        }

        public static List<KeyValuePair<string, string>> GetAllServers() {
            return servers.ToList();
        }

        public static List<KeyValuePair<string, string>> GetAll() {
            List<KeyValuePair<string, string>> connections = servers.ToList();

            connections.AddRange(clients.ToList());

            return connections;
        }
    }
}
