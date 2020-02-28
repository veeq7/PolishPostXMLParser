using System;

namespace PostXMLParser
{
    class Program
    {
        public static XMLData parameters = new XMLData();
        public static bool findNearest = false;
        public static bool error = false;

        static int Main(string[] args)
        {
            LoadParameters(args);

            /*parameters.dzien = "Sobota";
            parameters.godzina = "17:00";
            parameters.powiat = "dzierżoniowski";*/
            if(!error) XMLReader.Find();

            return 0;
        }

        public static void LoadParameters(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Za mało parametrów");
                error = true;
                return;
            }

            for(int i = 0; i< args.Length - 1; i += 2)
            {
                switch (args[i].ToString())
                {
                    case "-x": parameters.x = args[i + 1].ToString(); break;
                    case "-y": parameters.y = args[i + 1].ToString(); break;
                    case "-w": parameters.wojewodztwo = args[i + 1].ToString(); break;
                    case "-p": parameters.powiat = args[i + 1].ToString(); break;
                    case "-g": parameters.gmina = args[i + 1].ToString(); break;
                    case "-m": parameters.miejscowosc = args[i + 1].ToString(); break;
                    case "-d": parameters.dzien = args[i + 1].ToString(); break;
                    case "-godz": parameters.godzina = args[i + 1].ToString(); break;

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
