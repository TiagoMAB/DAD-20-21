using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    public class Record
    {
        private readonly int ts;
        private readonly string obj;
        private readonly string val;

        public Record(int ts, string obj, string val)
        {
            this.ts = ts;
            this.obj = obj;
            this.val = val;
        }

        public int getTimestamp()
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

        public override string ToString()
        {
            return String.Format("<TS: {0}, OBJ: {1}, VAL: {2}>", ts, obj, val);
        }
    }
}
