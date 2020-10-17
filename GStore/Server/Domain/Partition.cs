

using System.Collections.Generic;

namespace Domain
{
    public class Partition
    {
        public string name { get; }
        public string masterID { get; set; }
        public string masterURL { get; set; }
        public bool own { get; }
        public bool locked { get; set; }

        public Dictionary<string, string> replicas { get; set; }    //  Dictionary<replica_id, replica_url>
        public Dictionary<string, string> objects { get; set; }     //  Dictionary<object_id, value>

        public Partition(string name, string id, string url, bool own)
        {
            this.name = name;
            this.masterID = id;
            this.masterURL = url;
            this.own = own;
            this.locked = false;
            this.replicas = new Dictionary<string, string>();
            this.objects = new Dictionary<string, string>();
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