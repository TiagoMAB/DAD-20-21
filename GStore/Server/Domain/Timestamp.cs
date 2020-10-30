using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Domain
{
    public class Timestamp
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

        public int[] getValue()
        {
            return this.ts;
        }

        public void setEntry(int n, int val)
        {
            this.ts[n] = val;
        }

        public void merge(Timestamp other, int replica)
        {
            this.ts[replica] = Math.Max(this.ts[replica], other.ts[replica]);
        }

        public Boolean happensBefore(Timestamp other, int replica)
        {
            bool lower = false, higher = false, conflict = false;

            for(int i = 0; i < this.ts.Length; i++)
            {
                if(this.ts[i] > other.ts[i])
                {
                    if (lower)
                    {
                        conflict = true;
                        break;
                    }

                    higher = true;
                }
                else if(this.ts[i] < other.ts[i])
                {
                    if (higher)
                    {
                        conflict = true;
                        break;
                    }

                    lower = true;
                }
            }

            if(conflict)
            {
                return this.ts[replica] < other.ts[replica];
            }

            return lower;
        }

        public static bool operator <(Timestamp a, Timestamp b)
        {
            for(int i = 0; i < a.ts.Length; i++)
            {
                if (a.ts[i] > b.ts[i])
                {
                    return false;
                }
            }

            return true;
        }   

        public static bool operator >(Timestamp a, Timestamp b)
        {
            return !(a < b);
        }   
        public static bool operator ==(Timestamp a, Timestamp b)
        {
            for(int i = 0; i < a.ts.Length; i++)
            {
                if (a.ts[i] != b.ts[i])
                {
                    return false;
                }
            }

            return true;
        }   

        public static bool operator !=(Timestamp a, Timestamp b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Timestamp && this == ((Timestamp) obj);
        }

        public override int GetHashCode()
        {
            return this.ts.GetHashCode();
        }
    }
}
