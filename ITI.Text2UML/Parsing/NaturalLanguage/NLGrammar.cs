using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    public static class NLGrammar
    {
        public static List<string> Keywords = new List<string>() { "VB", "NN" };
        public static List<string> Verb_Be = new List<string>() { "be", "am", "are", "is", "was", "were" };
        public static List<string> Verb_Have = new List<string>() { "have", "has", "had" };
        
    }
}
