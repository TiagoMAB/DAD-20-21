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
        public static Script parseScript(PuppetMaster form, string path)
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

                    script.AddCommand(new Wait(form, Int32.Parse(command[1])));
                    continue;
                }

                script.AddCommand(Parse(form, command));
            }

            return script;
        }

        public static Command Parse(PuppetMaster form, string[] command)
        {
            switch (command[0])
            {
                case "Client":
                    if (command.Length != 4)
                    {
                        throw new WrongArgumentNumberException(command[0], 4, command.Length);
                    }

                    return new Client(form, command[1], command[2], command[3]);

                case "Crash":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new Crash(form, command[1]);

                case "Freeze":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new Freeze(form, command[1]);

                case "Partition":
                    if (command.Length < 4)
                    {
                        throw new WrongArgumentNumberException(command[0], 4, command.Length);
                    }

                    return new Partition(form, Int32.Parse(command[1]), command[2], command.Skip(3));

                case "ReplicationFactor":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new ReplicationFactor(form, Int32.Parse(command[1]));

                case "Server":
                    if (command.Length != 5)
                    {
                        throw new WrongArgumentNumberException(command[0], 5, command.Length);
                    }

                    return new Server(form, command[1], command[2], Int32.Parse(command[3]), Int32.Parse(command[4]));

                case "Status":
                    if (command.Length != 1)
                    {
                        throw new WrongArgumentNumberException(command[0], 1, command.Length);
                    }

                    return new Status(form);

                case "Unfreeze":
                    if (command.Length != 2)
                    {
                        throw new WrongArgumentNumberException(command[0], 2, command.Length);
                    }

                    return new Unfreeze(form, command[1]);

                default:
                    throw new UnknownCommandException(command[0]);
            }

        }
    }
}
