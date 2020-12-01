using Server.Domain;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Domain
{
    public class Partition
    {
        public string masterID { get; set; }

        public string masterURL { get; set; }
        public string name { get; }
        public bool own { get; }
        public bool locked { get; set; }

        public int uniqueId { get; set; }

        public Dictionary<string, string> replicas { get; set; }    //  Dictionary<replica_id, replica_url>
        public ConcurrentDictionary<string, string> objects { get; set; }     //  Dictionary<object_id, value>

        private List<Record> updateLog;    // Updates to send while in Gossip

        private List<string> order;     // List to get index of each server in the timestamp

        private int[] ts;     // TS

        private int num;      // Index in timestamp

        private Object objectsLock;     // Lock objects

        public Object updateLock { get; }     // Lock updateLog

        public Partition(string name, int index, List<string> servers)
        {
            this.name = name;
            this.own = index != -1;
            this.locked = false;
            this.replicas = new Dictionary<string, string>();
            this.objects = new ConcurrentDictionary<string, string>();
            this.updateLog = new List<Record>();
            this.ts = new int[servers.Count];
            this.num = index;
            this.objectsLock = new Object();
            this.updateLock = new Object();
            this.order = servers;
            this.uniqueId = 0;
        }

        public void addId(string id, string url)
        {
            if (replicas.Count == 0)
            {
                masterID = id;
                masterURL = url;
            }
            replicas.Add(id, url);
        }

        public int getUniqueId()
        {
            return ++uniqueId;
        }
        public void addObject(string key, string value, int id)
        {
            update(new Record(id, key, value));
        }

        // Add update to log and check if can be applied to current server
        private void update(Record r)
        {
            lock (this.objectsLock)
            {
                this.updateLog.Add(r);

                // Apply update if its the next one
                if (r.getTimestamp() == getTimestamp() + 1)
                {
                    objects[r.getObject()] = r.getValue();
                    setCurTimestamp(r.getTimestamp());
                }
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

        public int getTimestamp() => this.ts[this.num];

        public void setTimestamp(int n, int ts) => this.ts[n] = ts;

        public void setCurTimestamp(int val) => setTimestamp(this.num, val);

        public int getReplicaNumber() => this.num;

        public void update(List<Record> updates)
        {
            updates.ForEach(r => { 
                if(r.getTimestamp() > getTimestamp())
                {
                    update(r);
                }
            });
        }

        public void cleanLog()
        {
            this.updateLog.RemoveAll(r =>
            {
                foreach(int ts in this.ts)
                {
                    if (ts > getTimestamp())
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        public int getServerIndex(string id) => this.order.FindIndex(s => id == s);

        public override string ToString()
        {
            string ret = "Name: " + name + " | Master: " + masterID + " | Own: " + own + "\nIds: ";

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