using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PostXMLParser
{
    class XMLReader
    {
        public static List<XMLData> dataList = new List<XMLData>();
        static bool error = false;
        static int startTime = 0;
        static int endTime = 0;
        static int startTime2 = 0;
        static int endTime2 = 0;

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
                dataa.nazwa = level1Element.Attribute("nazwa").Value;
                dataa.typ = level1Element.Attribute("typ").Value;
                dataa.ulica = level1Element.Attribute("ulica").Value;
                dataa.kod = level1Element.Attribute("kod").Value;

                if (Program.findNearest)
                {
                    float x1 = float.Parse(dataa.x, CultureInfo.InvariantCulture);
                    float y1 = float.Parse(dataa.y, CultureInfo.InvariantCulture);
                    float x2 = float.Parse(parameters.x, CultureInfo.InvariantCulture);
                    float y2 = float.Parse(parameters.y, CultureInfo.InvariantCulture);

                    dataa.dystans = Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));
                    dataList.Add(dataa);
                }
                else
                {
                    if (!error && CheckIfDataMatch(dataa, parameters))
                    {
                        //Console.WriteLine("w: " + dataa.wojewodztwo + " p: " + dataa.powiat + " g: " + dataa.gmina + " m: " + dataa.miejscowosc);
                        //Console.WriteLine(dataa.opis);
                        dataList.Add(dataa);
                    }
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
            if (parameters.wojewodztwo != null)
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

            if (DataFormatIsExtended(data) && dzien != "sobota" && dzien != "niedziela")
            {
                SetStartTimeAndEndTimeForExtendedFormat(data, dzien);
            }
            else
            {
                switch (dzien)
                {
                    case "poniedziałek":
                    case "wtorek":
                    case "środa":
                    case "czwartek":
                    case "piątek": SetStartTimeAndEndTimeForPnPt(data); break;
                    case "sobota": SetStartTimeAndEndTimeForSb(data); break;
                    case "niedziela": SetStartTimeAndEndTimeForNdz(data); break;
                }
            }

            if (endTime == startTime) return false;
            if (endTime == 0)
            {
                endTime = 24 * 60;
            }


            if (godzina >= startTime && godzina <= endTime)
            {
                return true;
            }

            if (godzina >= startTime2 && godzina <= endTime2 && startTime2 != endTime2)
            {
                return true;
            }

            return false;
        }

        public static bool CheckTimeFormat(XMLData parameters)
        {
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

                        if (data.opis[i + 14 + move] == ' ') move2 += 1;
                        if (data.opis[i + 14 + move] == '0') move2 += 1;
                        if (data.opis[i + 16 + move] == ' ') move2 += 1;

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

        public static void SetStartTimeAndEndTimeForNdz(XMLData data)
        {
            for (int i = 0; i < data.opis.Length; i++)
            {
                if (data.opis[i] == '#' && data.opis[i + 1] == 'n')
                {
                    try
                    {
                        if (data.opis[i + 21] == 'N' || data.opis[i + 21] == 'p' || data.opis[i + 21] == 'S' || data.opis[i + 21] == 'n')
                        {
                            return;
                        }

                        int move = 0;
                        if (data.opis[i + 9] == ' ') move = 1;

                        startTime += Int32.Parse(data.opis[i + 21 + move].ToString()) * 600;
                        startTime += Int32.Parse(data.opis[i + 22 + move].ToString()) * 60;
                        startTime += Int32.Parse(data.opis[i + 24 + move].ToString()) * 10;
                        startTime += Int32.Parse(data.opis[i + 25 + move].ToString()) * 1;

                        int move2 = 0;

                        if (data.opis[i + 26 + move] == ' ') move2 += 1;
                        if (data.opis[i + 26 + move] == '0') move2 += 1;
                        if (data.opis[i + 28 + move] == ' ') move2 += 1;

                        move += move2;

                        endTime += Int32.Parse(data.opis[i + 27 + move].ToString()) * 600;
                        endTime += Int32.Parse(data.opis[i + 28 + move].ToString()) * 60;
                        endTime += Int32.Parse(data.opis[i + 30 + move].ToString()) * 10;
                        endTime += Int32.Parse(data.opis[i + 31 + move].ToString()) * 1;
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

        public static void SetStartTimeAndEndTimeForExtendedFormat(XMLData data, string dzien)
        {
            char char1 = '+';
            char char2 = '+';

            switch (dzien)
            {
                case "poniedziałek": char1 = 'P'; char2 = 'o'; break;
                case "wtorek": char1 = 'W'; char2 = 't'; break;
                case "środa": char1 = 'Ś'; char2 = 'r'; break;
                case "czwartek": char1 = 'C'; char2 = 'z'; break;
                case "piątek": char1 = 'P'; char2 = 't'; break;
            }

            for (int i = 0; i < data.opis.Length; i++)
            {
                if (data.opis[i] == char1 && data.opis[i + 1] == char2)
                {
                    try
                    {
                        int move = 0;
                        if (dzien == "poniedziałek" || dzien == "czwartek") move = 1;

                        startTime += Int32.Parse(data.opis[i + 5 + move].ToString()) * 600;
                        startTime += Int32.Parse(data.opis[i + 6 + move].ToString()) * 60;
                        startTime += Int32.Parse(data.opis[i + 8 + move].ToString()) * 10;
                        startTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 1;

                        endTime += Int32.Parse(data.opis[i + 11 + move].ToString()) * 600;
                        endTime += Int32.Parse(data.opis[i + 12 + move].ToString()) * 60;
                        endTime += Int32.Parse(data.opis[i + 14 + move].ToString()) * 10;
                        endTime += Int32.Parse(data.opis[i + 15 + move].ToString()) * 1;

                        if (data.opis[i + 16 + move] == ',') SetStartTime2AndEndTime2ForExtendedFormat(data, i + 16 + move);
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

        public static void SetStartTime2AndEndTime2ForExtendedFormat(XMLData data, int commaPosition)
        {
            try
            {
                int move = 0;

                startTime2 += Int32.Parse(data.opis[commaPosition + 2].ToString()) * 600;
                startTime2 += Int32.Parse(data.opis[commaPosition + 3].ToString()) * 60;
                startTime2 += Int32.Parse(data.opis[commaPosition + 5].ToString()) * 10;
                startTime2 += Int32.Parse(data.opis[commaPosition + 6].ToString()) * 1;

                //int move2 = 0;

                //if (data.opis[i + 10 + move] == ' ') move2 += 1;
                //if (data.opis[i + 10 + move] == '0') move2 += 1;
                //if (data.opis[i + 12 + move] == ' ') move2 += 1;

                //move += move2;

                endTime2 += Int32.Parse(data.opis[commaPosition + 8].ToString()) * 600;
                endTime2 += Int32.Parse(data.opis[commaPosition + 9].ToString()) * 60;
                endTime2 += Int32.Parse(data.opis[commaPosition + 11].ToString()) * 10;
                endTime2 += Int32.Parse(data.opis[commaPosition + 12].ToString()) * 1;

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
