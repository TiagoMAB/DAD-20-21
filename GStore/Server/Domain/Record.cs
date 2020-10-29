using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    class Record
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

        /*
        public static bool operator >(Record a, Record b)
        {
            return a.ts > b.ts;
        }   
        public static bool operator <(Record a, Record b)
        {
            return a.ts < b.ts;
        }   

        public static bool operator >=(Record a, Record b)
        {
            return a.ts >= b.ts;
        }   

        public static bool operator <=(Record a, Record b)
        {
            return a.ts <= b.ts;
        }   

        public static bool operator ==(Record a, Record b)
        {
            return a.ts == b.ts;
        }   

        public static bool operator !=(Record a, Record b)
        {
            return a.ts != b.ts;
        }   
        */
    }
}
