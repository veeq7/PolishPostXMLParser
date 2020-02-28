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

        public static void LoadXML()
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
                //data.dzien = level1Element.Attribute("dzien").Value;
                //data.godz = level1Element.Attribute("godz").Value;
                data.Add(dataa);
            }

        }
    }
}
