using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.Archives;
using System.Collections.Generic;

namespace PostXMLParser
{
    class Program
    {
        public static bool findNearest = false;

        static int Main(string[] args)
        {
            bool error = false;
            DownloadXMLFile();
            XMLData parameters = new XMLData();
            error = LoadArgsToParametersReturnError(args, parameters);
            

            /*findNearest = true;
            parameters.x = "55.55";
            parameters.y = "44.44";
            parameters.dzien = "wtorek";
            parameters.godzina = "07:00";
            parameters.powiat = "dzierżoniowski";*/
            //XMLReader.Find(parameters);


            if (error)
                return 0;

            List<XMLData> dataList = XMLReader.Find(parameters);
            if (dataList == null) return 0;
            ShowData(dataList);
            return 0;
        }

        /// <summary>
        /// Return error whenever args are incorrect
        /// </summary>
        public static bool LoadArgsToParametersReturnError(string[] args, XMLData parameters)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Za mało argumentów");
                return true;
            }

            for (int i = 0; i < args.Length - 1; i += 2)
            {
                String data = args[i + 1].ToString().ToLower();

                int incrementI = 0;

                while (i + 2 + incrementI < args.Length && args[i + 2 + incrementI][0] != '-')
                {
                    data += " ";
                    data += args[i + 2 + incrementI].ToString().ToLower();
                    incrementI++;
                }

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

                    default: Console.WriteLine("Niepoprawny argument: " + args[i].ToString()); return true;
                }

                i += incrementI;
            }

            if (parameters.x != null && parameters.y != null)
            {
                findNearest = true;
                if (parameters.dzien == null) parameters.dzien = "";
                if (parameters.godzina == null) parameters.godzina = "";
            }
            else
            {
                if (parameters.dzien == null)
                {
                    Console.WriteLine("Musisz podać dzień!");
                    return true;
                }

                if (parameters.godzina == null)
                {
                    Console.WriteLine("Musisz podać godzinę!");
                    return true;
                }

                if (parameters.wojewodztwo == null && parameters.powiat == null && parameters.gmina == null && parameters.miejscowosc == null)
                {
                    Console.WriteLine("Musisz podać przynajmniej jeden z tych argumentów: województwo, powiat, gmiana, miejscowość");
                    return true;
                }
            }

            return false;
        }

        public static void ShowData(List<XMLData> dataList)
        {
            if (findNearest)
            {
                List<XMLData> data = dataList.OrderBy(o => o.dystans).ToList();
                Console.WriteLine("Nazwa: " + data[0].nazwa);
                Console.WriteLine("Typ: " + data[0].typ);
                Console.WriteLine("Ulica: " + data[0].ulica);
                Console.WriteLine("Miejscowość: " + data[0].miejscowosc);
                Console.WriteLine("Kod: " + data[0].kod);
                Console.WriteLine("Współrzędne: " + data[0].x + ", " + data[0].y);
                Console.WriteLine("Odległość: " + data[0].dystans);
                Console.WriteLine("----------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"Znaleziono: {dataList.Count} punktów");

                foreach (XMLData data in dataList)
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
        }
         
        public static void DownloadXMLFile()
        {
            if (CheckIfDownloadIsNecessary())
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
                entry.WriteToFile(filepath);
            }
        }

        public static bool CheckIfDownloadIsNecessary()
        {
            if (!File.Exists("Odbior.xml") || new FileInfo("Odbior.xml").Length == 0)
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
                if (!data[0].ToString().Equals(DateTime.Today.ToString()))
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
            File.WriteAllText("Time.txt", DateTime.Today.ToString());
        }
    }
}
