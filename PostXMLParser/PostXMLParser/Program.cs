using System;

namespace PostXMLParser
{
    class Program
    {
        
        public static bool findNearest = false;
        public static bool error = false;

        static int Main(string[] args)
        {
            XMLData parameters = new XMLData();
            LoadArgsToParameters(args, parameters);

            /*parameters.dzien = "sobota";
            parameters.godzina = "17:00";
            parameters.powiat = "dzierżoniowski";
            XMLReader.Find(parameters);*/
            if (!error) XMLReader.Find(parameters);

            return 0;
        }

        public static void LoadArgsToParameters(string[] args, XMLData parameters)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Za mało parametrów");
                error = true;
                return;
            }

            for(int i = 0; i< args.Length - 1; i += 2)
            {
                String data = args[i + 1].ToString().ToLower();

                switch (args[i].ToString())
                {
                    case "-x": parameters.x = data; break;
                    case "-y": parameters.y = data; break;
                    case "-w": parameters.wojewodztwo = data; break;
                    case "-p": parameters.powiat = data; break;
                    case "-g": parameters.gmina = data; break;
                    case "-m": parameters.miejscowosc = data; break;
                    case "-d": parameters.dzien = data; break;
                    case "-godz": parameters.godzina = data; break;

                    default: Console.WriteLine("Niepoprawny argument: " + args[i].ToString()); error = true; return; 
                }
            }

            if(parameters.x != null && parameters.y != null)
            {
                findNearest = true;
            }
            else
            {
                if (parameters.dzien == null)
                {
                    Console.WriteLine("Musisz podać dzień!");
                    error = true;
                    return;
                }

                if (parameters.godzina == null)
                {
                    Console.WriteLine("Musisz podać godzinę!");
                    error = true;
                    return;
                }
            }

            if(parameters.wojewodztwo == null && parameters.powiat == null && parameters.gmina == null && parameters.miejscowosc == null)
            {
                Console.WriteLine("Musisz podać pdzynajmniej jeden z tych argumentów: województwo, powiat, gmiana, miejscowość");
                error = true;
                return;
            }
        }
    }
}
