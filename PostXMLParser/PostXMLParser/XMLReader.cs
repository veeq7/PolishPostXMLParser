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
                dataa.opis = level1Element.Attribute("opis").Value.Remove(level1Element.Attribute("opis").Value.Length - 1);

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
            //bool date = Checkdate(data);
            bool date = true;


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

            if (data.opis.ToLower().Contains("24h"))
            {
                return true;
            }

            if(data.opis[13].ToString() == "P")
            {
                //Console.WriteLine("asd");
            }
            else
            {
                if (dzien == "poniedziałek" || dzien == "wtorek" || dzien == "środa" || dzien == "czwartek" || dzien == "piątek")
                {
                    if (data.opis[13] >= '0' && data.opis[13] <= '9')
                    {
                        startTime += Int32.Parse(data.opis[13].ToString()) * 600;
                        startTime += Int32.Parse(data.opis[14].ToString()) * 60;
                        startTime += Int32.Parse(data.opis[16].ToString()) * 10;
                        startTime += Int32.Parse(data.opis[17].ToString()) * 1;

                        //

                        int move = 0;

                        if (data.opis[19] == '-') move = 2;
                        if (data.opis[19] == ' ') move = 1;
                        if (data.opis[18] == '0') move = 1;

                        endTime += Int32.Parse(data.opis[19 + move].ToString()) * 600;
                        endTime += Int32.Parse(data.opis[20 + move].ToString()) * 60;
                        endTime += Int32.Parse(data.opis[22 + move].ToString()) * 10;
                        endTime += Int32.Parse(data.opis[23 + move].ToString()) * 1;
                    }
                }
                else
                {
                    if (dzien == "sobota")
                    {
                        //Console.WriteLine(data.opis);

                        for (int i = 0; i < data.opis.Length; i++)
                        {
                            if (data.opis[i] == '#')
                            {
                                try
                                {
                                    int move = 0;

                                    if (data.opis[i + 9] == ' ') move = 1;

                                    startTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 600;
                                    startTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 60;
                                    startTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 10;
                                    startTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 1;

                                    //

                                    

                                    //if (data.opis[19] == '-') move = 2;
                                    //if (data.opis[19] == ' ') move = 1;
                                    //if (data.opis[18] == '0') move = 1;

                                    endTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 600;
                                    endTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 60;
                                    endTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 10;
                                    endTime += Int32.Parse(data.opis[i + 9 + move].ToString()) * 1;
                                }
                                catch
                                {
                                    if(data.opis[i + 9] == 'p' || data.opis[i + 9] == 'N')
                                    {
                                        return false;
                                    }

                                    //Console.WriteLine(data.opis);
                                }

                                goto end;
                            }
                        }

                    end:;
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
    }
}
