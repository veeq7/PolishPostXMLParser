using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.Archives;

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

            DownloadXMLFile();
            //Console.WriteLine("a");

            /*parameters.dzien = "wtorek";
            parameters.godzina = "07:00";
            parameters.powiat = "dzierżoniowski";
            XMLReader.Find(parameters);*/
            if (!error)
            {
                XMLReader.Find(parameters);
                ShowData();
            }

            return 0;
        }

        public static void LoadArgsToParameters(string[] args, XMLData parameters)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Za mało argumentów");
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
                Console.WriteLine("Musisz podać przynajmniej jeden z tych argumentów: województwo, powiat, gmiana, miejscowość");
                error = true;
                return;
            }
        }

        public static void ShowData()
        {
            foreach(XMLData data in XMLReader.dataList)
            {
                Console.WriteLine("Nazwa: " + data.nazwa);
                Console.WriteLine("Typ: " + data.typ);
                Console.WriteLine("Ulica: " + data.ulica);
                Console.WriteLine("Miejscowość: " + data.miejscowosc);
                Console.WriteLine("Kod: " + data.kod);
                Console.WriteLine("Współrzędne: " + data.x + ", " + data.y);
                Console.WriteLine("----------------------------------------------------");
            }
            
        }

        public static void DownloadXMLFile()
        {
            if(CheckIfDownloadIsNecessary())
            {
                Console.WriteLine("Download");

                using (var client = new WebClient())
                {
                    client.DownloadFile("https://odbiorwpunkcie.poczta-polska.pl/pliki.php?t=xmlK48S", "Odbior.zip");
                }

                UnRar("Odbior.zip", "Odbior.xml");
            }
        }

        private static void UnRar(string workingDirectory, string filepath)
        {
            if(!File.Exists(filepath))
            {
                File.Create(filepath);
            }

            var opts = new SharpCompress.Readers.ReaderOptions();
            var encoding = Encoding.GetEncoding(932);
            opts.ArchiveEncoding = new SharpCompress.Common.ArchiveEncoding();
            opts.ArchiveEncoding.CustomDecoder = (data, x, y) =>
            {
                return encoding.GetString(data);
            };
            var tr = SharpCompress.Archives.Zip.ZipArchive.Open(workingDirectory, opts);
            foreach (var entry in tr.Entries)
            {
                entry.WriteTo(File.Open(filepath, FileMode.Open));
            }
        }

        public static bool CheckIfDownloadIsNecessary()
        {
            if(!File.Exists("Odbior.xml"))
            {
                CreateTimeFile();
                return true;
            }

            if (!File.Exists("Time.txt"))
            {
                CreateTimeFile();
                return true;
            }

            try
            {
                string[] data = new string[1];
                data = File.ReadAllLines("Time.txt");
                if (!data[0].ToString().Equals(DateTime.Today.DayOfWeek.ToString()))
                {
                    CreateTimeFile();
                    return true;
                }
                else
                {
                    CreateTimeFile();
                    return false;
                }
            }
            catch
            {
                CreateTimeFile();
                return true;
            }
        }

        public static void CreateTimeFile()
        {
            File.WriteAllText("Time.txt", DateTime.Today.DayOfWeek.ToString());
        }
    }
}
