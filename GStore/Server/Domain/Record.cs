using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    public class Record
    {
        private readonly Timestamp ts;
        private readonly string obj;
        private readonly string val;

        public Record(Timestamp ts, string obj, string val)
        {
            this.ts = ts;
            this.obj = obj;
            this.val = val;
        }

        public Timestamp getTimestamp()
        {
            return this.ts;
        }

        public string getObject()
        {
            return this.obj;
        }

        public string getValue()
        {
            return this.val;
        }
    }
}
