using System;

namespace DDKTCKE
{
    class FormatovacTextu
    {
        public static string Uprav(string t)
        {
            string Text = t.Trim();
            Text = Text.Replace("\n      ", "\n");

            int poz = 0;
            while (Text.IndexOf("\n", poz) != -1)
            {
                int nPoz = Text.IndexOf("\n", poz);
                if (Char.IsLower(Text[nPoz + 1])) //Pokud je další písmeno malé můžu vymazat \n
                {
                    Text = Text.Remove(nPoz, 1);
                    Text = Text.Insert(nPoz, " ");
                }
                poz = nPoz + 1;
            }

            Text = Text.Replace("(!b)", "<b>");
            Text = Text.Replace("(?b)", "</b>");
            Text = Text.Replace("(!u)", "<u>");
            Text = Text.Replace("(?u)", "</u>");
            Text = Text.Replace("\n", "<br>");
            if (Text.LastIndexOf(")") == Text.Length - 1)
            {
                Text = Text.Insert(Text.LastIndexOf("(") - 1, "<i>");
                Text = Text.Insert(Text.LastIndexOf(")") + 1, "</i>");
            }

            return Text;
        }
    }
}
