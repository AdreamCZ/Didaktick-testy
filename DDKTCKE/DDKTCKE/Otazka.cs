using System.Collections.Generic;
using System.Xml.Serialization;


namespace DDKTCKE
{
    [XmlRoot(ElementName = "Moznosti")]
    public class Moznosti
    {
        [XmlElement(ElementName = "Moznost")]
        public List<string> Moznost { get; set; }

        public Moznosti(List<string> arr) => Moznost = arr;
    }

    [XmlRoot(ElementName = "Otazka")]
    public class Otazka
    {
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }
        [XmlElement(ElementName = "Typ")]
        public string Typ { get; set; }
        [XmlElement(ElementName = "Bodu")]
        public int Bodu { get; set; }
        [XmlElement(ElementName = "Ukol")]
        public string Ukol { get; set; }

        [XmlElement(ElementName = "Moznosti")]
        public Moznosti Moznosti { get; set; }
        [XmlElement(ElementName = "Spravna")]
        public string Spravna { get; set; }
        [XmlElement(ElementName = "Zdroj")]
        public string Zdroj { get; set; }
    }

    [XmlRoot(ElementName = "Otazky")]
    public class Otazky
    {
        [XmlElement(ElementName = "Otazka")]
        public List<Otazka> Otazka { get; set; }
    }

}



