using Server.Domain;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Domain
{
    public class Partition
    {
        public string name { get; }
        public bool own { get; }
        public bool locked { get; set; }

        public List<string> order;

        public Dictionary<string, string> replicas { get; set; }    //  Dictionary<replica_id, replica_url>
        public ConcurrentDictionary<string, string> objects { get; set; }     //  Dictionary<object_id, value>

        private List<Record> updateLog;

        private Timestamp[] ts;     // Vectorial TS

        private int num;   // Server index in TS

        public Partition(string name, int index, List<string> servers)
        {
            this.name = name;
            this.own = index != -1;
            this.locked = false;
            this.replicas = new Dictionary<string, string>();
            this.objects = new ConcurrentDictionary<string, string>();
            this.updateLog = new List<Record>();
            this.ts = new Timestamp[servers.Count];
            this.order = servers;

            for(int i = 0; i < servers.Count; i++)
            {
                this.ts[i] = new Timestamp(servers.Count);
            }

            this.num = index;
        }

        public void addId(string id, string url)
        {
            replicas.Add(id, url);
        }

        public void addObject(string id, string value)
        {
            lock (getTimestamp())
            {
                objects[id] = value;

                this.ts[this.num].inc(this.num);
                this.updateLog.Add(new Record(this.ts[this.num], id, value));
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

        public List<Record> getUpdateLog() => this.updateLog;

        public Timestamp getTimestamp() => this.ts[this.num];

        public int getReplicaNumber() => this.num;

        public void setTimestamp(int n, Timestamp ts) => this.ts[n] = ts;

        public void setCurTimestamp(int n, int val) => this.ts[this.num].setEntry(n, val);

        public void update(List<Record> updates, int replica)
        {
            updates.ForEach(r => { 
                if(this.ts[this.num].happensBefore(r.getTimestamp(), replica))
                {
                    this.ts[this.num].merge(r.getTimestamp(), replica);
                    this.objects[r.getObject()] = r.getValue();
                }
            });
        }

        public void cleanLog()
        {
            this.updateLog.RemoveAll(r =>
            {
                foreach(Timestamp ts in this.ts)
                {
                    if (ts > this.ts[this.num])
                    {
                        return false;
                    }
                }

                return true;
            });
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