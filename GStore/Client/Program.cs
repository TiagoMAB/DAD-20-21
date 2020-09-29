using System;
using System.IO;
using Client.Commands;
using Client.Exceptions;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //should it exit after parsing script?
            //make a try/catch for script
            Script script;
            try
            {
                script = Parser.ParseScript();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (InvalidExpressionException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (CycleInceptionException)
            {
                Console.WriteLine("It is not possible to have a cycle inside another cycle.");
                return;
            }
            catch (NoEndOfCycleException)
            {
                Console.WriteLine("Script ended with no cycle closure.");
                return;
            }

            script.Execute();
        }
    }
}
