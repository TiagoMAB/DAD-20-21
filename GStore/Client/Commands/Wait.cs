﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Commands
{
    class Wait : Command
    {
        private readonly int time;

        public Wait(int time)
        {
            this.time = time;
        }

        public void Execute()
        {
            Console.WriteLine("Client sleeping for {0}ms...", this.time);

            System.Diagnostics.Debug.WriteLine(String.Format("Waiting for {0} ms", this.time.ToString()));

            System.Threading.Thread.Sleep(this.time);
        }
    }
}
