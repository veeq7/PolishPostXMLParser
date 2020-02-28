using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PostXMLParser
{
    class XMLReader
    {
        public static List<XMLData> data = new List<XMLData>();

        public static void Find()
        {

            foreach (XElement level1Element in XElement.Load(@"Odbior.xml").Elements("r"))
            {
                XMLData dataa = new XMLData();
                dataa.x = level1Element.Attribute("x").Value;
                dataa.y = level1Element.Attribute("y").Value;
                dataa.wojewodztwo = level1Element.Attribute("wojewodztwo").Value;
                dataa.powiat = level1Element.Attribute("powiat").Value;
                dataa.gmina = level1Element.Attribute("gmina").Value;
                dataa.miejscowosc = level1Element.Attribute("miejscowosc").Value;
                dataa.opis = level1Element.Attribute("opis").Value;

                if (CheckIfDataMatch(dataa))
                {
                    //Console.WriteLine("w: " + dataa.wojewodztwo + " p: " + dataa.powiat + " g: " + dataa.gmina + " m: " + dataa.miejscowosc);
                    Console.WriteLine(dataa.opis);
                    
                    data.Add(dataa);
                }
            }

        }

        public static bool CheckIfDataMatch(XMLData data)
        {
            bool location = CheckLocation(data);
            bool date = Checkdate(data);


            if (location == true && date == true) return true;
            else return false;
        }

        public static bool CheckLocation(XMLData data)
        {
            if(Program.parameters.wojewodztwo != null)
            {
                if (!Program.parameters.wojewodztwo.Equals(data.wojewodztwo)) return false;
            }

            if (Program.parameters.powiat != null)
            {
                if (!Program.parameters.powiat.Equals(data.powiat)) return false;
            }

            if (Program.parameters.gmina != null)
            {
                if (!Program.parameters.gmina.Equals(data.gmina)) return false;
            }

            if (Program.parameters.miejscowosc != null)
            {
                if (!Program.parameters.miejscowosc.Equals(data.miejscowosc)) return false;
            }

            return true;
        }

        public static bool Checkdate(XMLData data)
        {
            string dzien = Program.parameters.dzien.ToLower();
            int godzina = 0;

            try
            {
                godzina += Int32.Parse(Program.parameters.godzina[0].ToString()) * 600;
                godzina += Int32.Parse(Program.parameters.godzina[1].ToString()) * 60;
                godzina += Int32.Parse(Program.parameters.godzina[3].ToString()) * 10;
                godzina += Int32.Parse(Program.parameters.godzina[4].ToString()) * 1;
            }
            catch
            {
                Console.WriteLine("Niepoprawny format godziny! Poprawny format: 06:30");
                return false;
            }     

            int startTime = 0;
            int endTime = 0;

            if (data.opis.Contains("24h"))
            {
                return true;
            }
            else
            {
                if (dzien == "poniedziałek" || dzien == "wtorek" || dzien == "środa" || dzien == "czwartek" || dzien == "piątek")
                {
                    try
                    {
                        if (data.opis[13] >= '0' && data.opis[13] <= '9')
                        {
                            startTime += Int32.Parse(data.opis[13].ToString()) * 600;
                            startTime += Int32.Parse(data.opis[14].ToString()) * 60;
                            startTime += Int32.Parse(data.opis[16].ToString()) * 10;
                            startTime += Int32.Parse(data.opis[17].ToString()) * 1;

                            endTime += Int32.Parse(data.opis[19].ToString()) * 600;
                            endTime += Int32.Parse(data.opis[20].ToString()) * 60;
                            endTime += Int32.Parse(data.opis[22].ToString()) * 10;
                            endTime += Int32.Parse(data.opis[23].ToString()) * 1;

                            //Console.WriteLine(startTime + "-" + endTime);
                        }
                    }
                    catch
                    {
                        startTime = 100000;
                    }
                }
            }


            if(godzina >= startTime && godzina <= endTime)
            {
                return true;
            }



            return false;
        }
    }
}
