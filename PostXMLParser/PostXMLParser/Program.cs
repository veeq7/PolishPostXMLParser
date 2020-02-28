using System;

namespace PostXMLParser
{
    class Program
    {
        static int Main(string[] args)
        {
            /*if (args.Length < 2) return 0;

            string argument = "";

            switch(args[0].ToString())
            {
                case "-w": argument = "w"; break;
                case "-p": argument = "p"; break;
                case "-g": argument = "g"; break;
                case "-m": argument = "m"; break;
                case "-d": argument = "d"; break;
                case "-godz": argument = "godz"; break;
                default: Console.WriteLine("Niepoprawny argument"); return 0;
            }*/
            XMLReader.LoadXML();
            
            return 0;
        }
    }
}
