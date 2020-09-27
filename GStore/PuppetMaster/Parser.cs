using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using PuppetMaster.Commands;
using PuppetMaster.Exceptions;

namespace PuppetMaster
{
    public static class Parser
    {
        public static Script parseScript(string path)
        {
            IEnumerable<string> file = File.ReadLines(path);
            Script script = new Script();

            foreach (string line in file)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] command = line.Trim().Split(" ");

                if (command[0].Equals("Wait"))
                {
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    script.AddCommand(new Wait(Int32.Parse(command[1])));
                    continue;
                }

                script.AddCommand(Parse(command));
            }

            return script;
        }

        public static Command Parse(string[] command)
        {
            switch (command[0])
            {
                case "Client":
                    if (command.Length != 4)
                    {
                        throw new WrongArgumentNumberException(command[0], 4, command.Length);
                    }

                    return new Client(command[1], command[2], command[3]);

                case "Crash":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new Crash(command[1]);

                case "Freeze":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new Freeze(command[1]);

                case "Partition":
                    if (command.Length < 4)
                    {
                        throw new WrongArgumentNumberException(command[0], 4, command.Length);
                    }

                    return new Partition(Int32.Parse(command[1]), command[2], command.Skip(3));

                case "ReplicationFactor":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new ReplicationFactor(Int32.Parse(command[1]));

                case "Server":
                    if (command.Length != 5)
                    {
                        throw new WrongArgumentNumberException(command[0], 5, command.Length);
                    }

                    return new Server(command[1], command[2], Int32.Parse(command[3]), Int32.Parse(command[4]));

                case "Status":
                    if (command.Length != 1)
                    {
                        throw new WrongArgumentNumberException(command[0], 1, command.Length);
                    }

                    return new Status();

                case "Unfreeze":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new Unfreeze(command[1]);

                default:
                    throw new UnknownCommandException(command[0]);
            }

        }
    }
}
