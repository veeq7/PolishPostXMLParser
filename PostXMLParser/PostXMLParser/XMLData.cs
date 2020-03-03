using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace PostXMLParser
{
    class XMLData
    {
        public XMLData()
        {

        }

        public XMLData(XElement content)
        {
            x = content.Attribute("x").Value;
            y = content.Attribute("y").Value;
            wojewodztwo = content.Attribute("wojewodztwo").Value.ToLower();
            powiat = content.Attribute("powiat").Value.ToLower();
            gmina = content.Attribute("gmina").Value.ToLower();
            miejscowosc = content.Attribute("miejscowosc").Value.ToLower();
            opis = content.Attribute("opis").Value.Remove(content.Attribute("opis").Value.Length - 1);
            nazwa = content.Attribute("nazwa").Value;
            typ = content.Attribute("typ").Value;
            ulica = content.Attribute("ulica").Value;
            kod = content.Attribute("kod").Value;
        }

        public string x {get; set;}
        public string y {get; set;}
        public string wojewodztwo { get; set; }
        public string powiat {get; set;}
        public string gmina { get; set;}
        public string miejscowosc {get; set;}
        public string dzien {get; set;}
        public string godzina {get; set;}
        public string opis { get; set; }
        public string nazwa { get; set; }
        public string typ { get; set; }
        public string ulica { get; set; }
        public string kod { get; set; }
        public double dystans { get; set; }
    }
}
