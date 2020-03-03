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
    static class XMLReader
    {
        static bool error = false;
        static int startTime = 0;
        static int endTime = 0;
        static int startTime2 = 0;
        static int endTime2 = 0;

        public static List<XMLData> Find(XMLData parameters)
        {
            List<XMLData> dataList = new List<XMLData>();

            foreach (XElement level1Element in XElement.Load(@"Odbior.xml").Elements("r"))
            {
                XMLData data = new XMLData(level1Element);

                if (Program.findNearest)
                {
                    try
                    {
                        float x1 = float.Parse(data.x, CultureInfo.InvariantCulture);
                        float y1 = float.Parse(data.y, CultureInfo.InvariantCulture);
                        float x2 = float.Parse(parameters.x, CultureInfo.InvariantCulture);
                        float y2 = float.Parse(parameters.y, CultureInfo.InvariantCulture);

                        data.dystans = Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));
                        dataList.Add(data);
                    }
                    catch
                    {
                        Console.WriteLine("Wprowadziłeś nieprawidłową liczbę!");
                        dataList = null;
                        return dataList;
                    }
                }
                else
                {
                    if (!error && CheckIfDataMatch(data, parameters))
                    {
                        //Console.WriteLine("w: " + data.wojewodztwo + " p: " + data.powiat + " g: " + data.gmina + " m: " + data.miejscowosc);
                        //Console.WriteLine(data.opis);
                        dataList.Add(data);
                    }
                }
            }

            return dataList;
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
            if (!CheckLocation(data.wojewodztwo, parameters.wojewodztwo)) return false;
            if (!CheckLocation(data.powiat, parameters.powiat)) return false;
            if (!CheckLocation(data.gmina, parameters.gmina)) return false;
            if (!CheckLocation(data.miejscowosc, parameters.miejscowosc)) return false;

            return true;
        }

        public static bool CheckLocation(string dataLocation, string parameterLocation)
        {
            if (string.IsNullOrEmpty(parameterLocation))
                return true;

            if (!parameterLocation.Equals(dataLocation))
                return false;

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
                godzina = ReadTime(parameters.godzina.Substring(0, 5));
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

                startTime = ReadTime(data.opis.Substring(13 + move, 5));

                if (data.opis[19] == '-') move = 2;
                if (data.opis[19] == ' ') move = 1;
                if (data.opis[18] == '0') move = 1;

                endTime = ReadTime(data.opis.Substring(19 + move, 5));
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

                        startTime = ReadTime(data.opis.Substring(i + 9 + move, 5));

                        int move2 = 0;
                        if (data.opis[i + 14 + move] == ' ') move2 += 1;
                        if (data.opis[i + 14 + move] == '0') move2 += 1;
                        if (data.opis[i + 16 + move] == ' ') move2 += 1;
                        move += move2;

                        endTime = ReadTime(data.opis.Substring(i + 15 + move, 5));
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
                        //Jeżeli napis to: Nieczynne/nieczynny/Sprawdź w sklepie//placówka nieczynna
                        if (data.opis[i + 21] == 'N' || data.opis[i + 21] == 'n' || data.opis[i + 21] == 'S' || data.opis[i + 21] == 'p')
                        {
                            return;
                        }

                        int move = 0;
                        if (data.opis[i + 9] == ' ') move = 1;

                        startTime = ReadTime(data.opis.Substring(i + 21 + move, 5));

                        int move2 = 0;
                        if (data.opis[i + 26 + move] == ' ') move2 += 1;
                        if (data.opis[i + 26 + move] == '0') move2 += 1;
                        if (data.opis[i + 28 + move] == ' ') move2 += 1;
                        move += move2;

                        endTime = ReadTime(data.opis.Substring(i + 27 + move, 5));
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

                        startTime = ReadTime(data.opis.Substring(i + 5 + move, 5));
                        endTime = ReadTime(data.opis.Substring(i + 11 + move, 5));

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
                startTime2 = ReadTime(data.opis.Substring(commaPosition + 2, 5));
                endTime2 = ReadTime(data.opis.Substring(commaPosition + 8, 5));
            }
            catch
            {
                startTime = 100000;
                endTime = 100000;
            }

            return;
        }

        private static int ReadTime(string time)
        {
            int returnTime = 0;

            returnTime += Int32.Parse(time[0].ToString()) * 600;
            returnTime += Int32.Parse(time[1].ToString()) * 60;
            returnTime += Int32.Parse(time[3].ToString()) * 10;
            returnTime += Int32.Parse(time[4].ToString()) * 1;

            return returnTime;
        }

    }
}
