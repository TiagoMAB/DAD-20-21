using System;
using System.IO;
using System.Collections.Generic;
using Client.Commands;
using Client.Exceptions;
using System.Linq;

namespace Client
{
    class Parser
    {
        public static Script ParseScript(string path)
        {
            Script script = new Script();
            string line;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Given file doesn't exist.");
            }

            StreamReader file = new StreamReader(path);

            while ((line = file.ReadLine()) != null)
            {
                if (String.IsNullOrWhiteSpace(line)) {continue;}

                string[] command = line.Trim().Split(" ");

                switch (command[0])
                {
                    case "begin-repeat":
                        int repeatTimes;
                        BeginRepeat beginRepeat;
                        List<string> lines = new List<string>();

                        if (command.Length != 2)
                        {
                            throw new InvalidExpressionException("Invalid input:\n" + line);
                        }
                        else if (!int.TryParse(command[1], out repeatTimes))
                        {
                            throw new InvalidExpressionException("Invalid number for repeats:\n" + line);
                        }

                        beginRepeat = new BeginRepeat();

                        while ((line = file.ReadLine()) != null && string.Compare(line, "end-repeat") != 0)
                            lines.Add(line);

                        if (line == null)
                            throw new NoEndOfCycleException();

                        foreach (int i in Enumerable.Range(1, repeatTimes))
                            foreach (string cycleLine in lines)
                                beginRepeat.AddCommand(ParseLine(new string(cycleLine).Replace("$i", i.ToString())));

                        script.AddCommand(beginRepeat);
                        break;

                    default:
                        script.AddCommand(ParseLine(line));
                        break;
                }
            }
            return script;
        }

        public static Command ParseLine(string line)
        {
                string[] command = line.Trim().Split(" ");

                switch (command[0])
                {
                    case "read":
                        if (command.Length != 4)
                        {
                            throw new InvalidExpressionException("Invalid input:\n" + line);
                        }
                        return new Read(command[1], command[2], command[3]);

                    case "write":
                        if (command.Length != 4)
                        {
                            throw new InvalidExpressionException("Invalid input:\n" + line);
                        }
                       return new Write(command[1], command[2], command[3]);
                    case "listServer":
                        if (command.Length != 2)
                        {
                            throw new InvalidExpressionException("Invalid input:\n" + line);
                        }
                        return new ListServer(command[1]);
                    case "listGlobal":
                        if (command.Length != 1)
                        {
                            throw new InvalidExpressionException("Invalid input:\n" + line);
                        }
                        return new ListGlobal();
                    case "wait":
                        int time;
                        if (command.Length != 2)
                        {
                            throw new InvalidExpressionException("Invalid input:\n" + line);
                        }
                        else if (!int.TryParse(command[1], out time))
                        {
                            throw new InvalidExpressionException("Invalid time given:\n" + line);
                        }
                        return new Wait(time);
                    case "begin-repeat":
                        throw new CycleInceptionException();
                    default:
                        throw new InvalidExpressionException("Invalid input:\n" + line);
                }
        }
    }
}
