using Server.Domain;
using System.Collections.Generic;

namespace Domain
{
    public class Partition
    {
        public string name { get; }
        public bool own { get; }
        public bool locked { get; set; }

        public Dictionary<string, string> replicas { get; set; }    //  Dictionary<replica_id, replica_url>
        public Dictionary<string, string> objects { get; set; }     //  Dictionary<object_id, value>

        private List<Record> updateLog { get; }    // Gossip updates

        private Timestamp ts { get; }    // Vectorial TS

        private int num { get; }   // Server index in TS

        public Partition(string name, int index, int servers)
        {
            this.name = name;
            this.own = index != -1;
            this.locked = false;
            this.replicas = new Dictionary<string, string>();
            this.objects = new Dictionary<string, string>();
            this.updateLog = new List<Record>();
            this.ts = new Timestamp(servers);
            this.num = index;
        }

        public void addId(string id, string url)
        {
            replicas.Add(id, url);
        }

        public void addObject(string id, string value)
        {
            if (objects.ContainsKey(id))
            {
                objects[id] = value;
            }
            else
            {
                objects.Add(id, value);
            }

            lock (this.ts) {
                this.ts.inc(this.num);
                this.updateLog.Add(new Record(this.ts, id, value));
            }
        }

        public string getObject(string id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            else
            {
                return "N/A";
            }
        }

        public override string ToString()
        {
            string ret = "Name: " + name + " | Own: " + own + "\nIds: ";
            
            foreach (string id in replicas.Keys)
            {
                ret = ret + id + "; "; 
            }
            
            if (own)
            {
                ret = ret + "\nObjects:";

                foreach (KeyValuePair<string, string> info in this.objects)
                {
                    ret = ret + "\nObject id: " + info.Key + " - Value: " + info.Value + "; ";
                }
            }

            return ret;
        }
    }
}