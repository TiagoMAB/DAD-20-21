using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    class Timestamp
    {
        private readonly int[] ts;

        public Timestamp(int n)
        {
            this.ts = new int[n];
        }

        public Timestamp(int[] ts)
        {
            this.ts = ts;
        }

        public int getReplica(int n)
        {
            return this.ts[n];
        }

        public void inc(int n)
        {
            this.ts[n]++;
        }

        // TODO: Implement
        /*
        public static bool operator >(Timestamp a, Timestamp b)
        {
            return a.ts > b.ts;
        }   
        public static bool operator <(Timestamp a, Timestamp b)
        {
            return a.ts < b.ts;
        }   

        public static bool operator >=(Timestamp a, Timestamp b)
        {
            return a.ts >= b.ts;
        }   

        public static bool operator <=(Timestamp a, Timestamp b)
        {
            return a.ts <= b.ts;
        }   

        public static bool operator ==(Timestamp a, Timestamp b)
        {
            return a.ts == b.ts;
        }   

        public static bool operator !=(Timestamp a, Timestamp b)
        {
            return a.ts != b.ts;
        }   
        */
    }
}
