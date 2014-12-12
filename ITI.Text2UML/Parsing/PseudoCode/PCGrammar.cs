using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML
{
    public static class Grammar
    {
        public static List<string> Keywords = new List<string>() { "Class", "Interface", "Abstract" };
        public static List<string> Links = new List<string>() { "->", "-->", "-.>" };
    }
}
