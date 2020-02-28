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
        static bool error = false;
        static int startTime = 0;
        static int endTime = 0;

        public static void Find(XMLData parameters)
        {

            foreach (XElement level1Element in XElement.Load(@"Odbior.xml").Elements("r"))
            {
                XMLData dataa = new XMLData();
                dataa.x = level1Element.Attribute("x").Value;
                dataa.y = level1Element.Attribute("y").Value;
                dataa.wojewodztwo = level1Element.Attribute("wojewodztwo").Value.ToLower();
                dataa.powiat = level1Element.Attribute("powiat").Value.ToLower();
                dataa.gmina = level1Element.Attribute("gmina").Value.ToLower();
                dataa.miejscowosc = level1Element.Attribute("miejscowosc").Value.ToLower();
                dataa.opis = level1Element.Attribute("opis").Value.Remove(level1Element.Attribute("opis").Value.Length - 1);

                if (!error && CheckIfDataMatch(dataa, parameters))
                {
                    //Console.WriteLine("w: " + dataa.wojewodztwo + " p: " + dataa.powiat + " g: " + dataa.gmina + " m: " + dataa.miejscowosc);
                    Console.WriteLine(dataa.opis);
                    data.Add(dataa);
                }
            }
        }

        public static bool CheckIfDataMatch(XMLData data, XMLData parameters)
        {
            bool location = CheckLocation(data, parameters);
            //bool location = true;
            bool date = Checkdate(data, parameters);
            //bool date = true;


            if (location == true && date == true) return true;
            else return false;
        }

        public static bool CheckLocation(XMLData data, XMLData parameters)
        {
            if(parameters.wojewodztwo != null)
            {
                if (!parameters.wojewodztwo.Equals(data.wojewodztwo)) return false;
            }

            if (parameters.powiat != null)
            {
                if (!parameters.powiat.Equals(data.powiat)) return false;
            }

            if (parameters.gmina != null)
            {
                if (!parameters.gmina.Equals(data.gmina)) return false;
            }

            if (parameters.miejscowosc != null)
            {
                if (!parameters.miejscowosc.Equals(data.miejscowosc)) return false;
            }

            return true;
        }

        public static bool Checkdate(XMLData data, XMLData parameters)
        {
            string dzien = parameters.dzien.ToLower();
            int godzina = 0;
            startTime = 0;
            endTime = 0;

            if (CheckTimeFormat(parameters))
            {
                godzina += Int32.Parse(parameters.godzina[0].ToString()) * 600;
                godzina += Int32.Parse(parameters.godzina[1].ToString()) * 60;
                godzina += Int32.Parse(parameters.godzina[3].ToString()) * 10;
                godzina += Int32.Parse(parameters.godzina[4].ToString()) * 1;
            }
            else
            {
                Console.WriteLine("Niepoprawny format godziny! Poprawny format: 06:30");
                error = true;
                return false;
            }

            if (data.opis.ToLower().Contains("24h"))
            {
                return true;
            }

            if(DataFormatIsExtended(data))
            {

            }
            else
            {
                if (dzien == "poniedziałek" || dzien == "wtorek" || dzien == "środa" || dzien == "czwartek" || dzien == "piątek")
                {
                    SetStartTimeAndEndTimeForPnPt(data);
                }
                else
                {
                    if (dzien == "sobota")
                    {
                        SetStartTimeAndEndTimeForSb(data);
                    }
                    else
                    {

                    }
                }
            }


            if(godzina >= startTime && godzina <= endTime)
            {
                return true;
            }

            return false;
        }

        public static bool CheckTimeFormat(XMLData parameters)
        {
            int godzina = 0;

            if (parameters.godzina[0] < '0' || parameters.godzina[0] > '9') return false;
            if (parameters.godzina[0] < '0' || parameters.godzina[1] > '9') return false;
            if (parameters.godzina[0] < '0' || parameters.godzina[3] > '9') return false;
            if (parameters.godzina[0] < '0' || parameters.godzina[4] > '9') return false;

            return true;
        }

        public static bool DataFormatIsExtended(XMLData data)
        {
            if (data.opis[13].ToString() == "P") return true;
            else return false;
        }

        public static void SetStartTimeAndEndTimeForPnPt(XMLData data)
        {
            if ((data.opis[13] >= '0' && data.opis[13] <= '9') || data.opis[13] == ' ')
            {
                int move = 0;
                if (data.opis[13] == ' ') move = 1;

                startTime += Int32.Parse(data.opis[13 + move].ToString()) * 600;
                startTime += Int32.Parse(data.opis[14 + move].ToString()) * 60;
                startTime += Int32.Parse(data.opis[16 + move].ToString()) * 10;
                startTime += Int32.Parse(data.opis[17 + move].ToString()) * 1;

                if (data.opis[19] == '-') move = 2;
                if (data.opis[19] == ' ') move = 1;
                if (data.opis[18] == '0') move = 1;

                endTime += Int32.Parse(data.opis[19 + move].ToString()) * 600;
                endTime += Int32.Parse(data.opis[20 + move].ToString()) * 60;
                endTime += Int32.Parse(data.opis[22 + move].ToString()) * 10;
                endTime += Int32.Parse(data.opis[23 + move].ToString()) * 1;
            }
        }

        public static void SetStartTimeAndEndTimeForSb(XMLData data)
        {
            for (int i = 0; i < data.opis.Length; i++)
            {
                if (data.opis[i] == '#')
                {
                    try
                    {
                        int move = 0;

                        if (data.opis[i + 9] == ' ') move = 1;

                        startTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 600;
                        startTime += Int32.Parse(data.opis[i + 10 + move].ToString()) * 60;
                        startTime += Int32.Parse(data.opis[i + 12 + move].ToString()) * 10;
                        startTime += Int32.Parse(data.opis[i + 13 + move].ToString()) * 1;

                        int move2 = 0;

                        if (data.opis[i + 14 + move ] == ' ') move2 += 1;
                        if (data.opis[i + 14 + move ] == '0') move2 += 1;
                        if (data.opis[i + 16 + move ] == ' ') move2 += 1;

                        move += move2;
                        
                        endTime += Int32.Parse(data.opis[i + 15 + move].ToString()) * 600;
                        endTime += Int32.Parse(data.opis[i + 16 + move].ToString()) * 60;
                        endTime += Int32.Parse(data.opis[i + 18 + move].ToString()) * 10;
                        endTime += Int32.Parse(data.opis[i + 19 + move].ToString()) * 1;
                    }
                    catch
                    {
                        startTime = 100000;
                        endTime = 100000;
                    }

                    return;
                }
            }
        }
    }
}
