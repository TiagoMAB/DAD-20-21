using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

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
