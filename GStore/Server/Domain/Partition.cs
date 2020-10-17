

using System;
using System.Collections.Generic;

namespace Domain
{
    public class Partition
    {
        public string name { get; }
        public string masterID { get; set; }
        public string masterURL { get; set; }
        public bool own { get; }

        public Dictionary<string, string> replicas { get; set; }    //  Dictionary<replica_id, replica_url>
        public Dictionary<string, string> objects { get; set; }     //  Dictionary<object_id, value>

        public Partition(string name, string id, string url, bool own)
        {
            this.name = name;
            this.masterID = id;
            this.masterURL = url;
            this.own = own;
            this.replicas = new Dictionary<string, string>();
            this.objects = new Dictionary<string, string>();
        }

        public void addId(string id, string url)
        {
            replicas.Add(id, url);
        }

        public override string ToString()
        {
            string ret = "Name: " + name + " | Master: " + masterID + " | Own: " + own + "\n";
            
            foreach (string id in replicas.Keys)
            {
                ret = ret + "Id: " + id + " | "; 
            }

            return ret;
        }
    }
}
/*
Console.WriteLine("Known partitions:");
foreach (KeyValuePair<string, List<string>> partition in this.partitions)
{
    Console.Write("Partition: " + partition.Key + " Servers: ");
    foreach (string server in partition.Value)
    {
        Console.Write(server + " ; ");
    }
    Console.WriteLine();
}
Console.WriteLine("Known masters:");
foreach (KeyValuePair<string, string> server in this.masters)
{
    Console.WriteLine("Partition: " + server.Key + " Master: " + server.Value);
}
*/