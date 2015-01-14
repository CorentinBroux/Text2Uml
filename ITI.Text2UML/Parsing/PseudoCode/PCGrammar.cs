using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.PseudoCode
{
    public static class PCGrammar
    {
        /// <summary>
        /// Lists all pseudo code keywords
        /// </summary>
        public static List<string> Keywords = new List<string>() { 
            "class"
        };

        /// <summary>
        /// Lists all pseudo code link symbols
        /// </summary>
        public static List<string> Links = new List<string>() { 
            "->",
            "-->",
        };
    }
}
