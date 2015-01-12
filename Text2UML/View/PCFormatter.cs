﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITI.Text2UML.Parsing.PseudoCode;
using ITI.Text2UML.Model;
using MoreLinq;
using ITI.Text2UML.Parsing.NaturalLanguage;
using System.Text.RegularExpressions;


namespace Text2UML.View
{
    public static class PCFormatter
    {
        /// <summary>
        /// Formats the pseudo code to avoid duplications and for user-friendly display.
        /// </summary>
        /// <param name="input">Pseudo code</param>
        /// <returns>Formatted pseudo code</returns>
        public static string Format(string input)
        {
            // Initialize
            Tuple<List<Class>, List<Link>> tuple = PCParser.Parse(input);
            string output = "";


            // Format classes
            foreach (Class c in tuple.Item1)
            {
                // Specify specials types (defined by user input)
                foreach (ITI.Text2UML.Model.Attribute a in c.Attributes)
                    foreach (Tuple<string, string> t in ITI.Text2UML.Parsing.NaturalLanguage.NLGrammar.Types)
                        if (a.Name.Equals(t.Item1, StringComparison.InvariantCultureIgnoreCase))
                            a.Type = t.Item2;


                c.Attributes = c.Attributes.DistinctBy(x => x.Name).ToList();
                c.Methods = c.Methods.DistinctBy(x => x.Name).ToList();


                output += c.ToString();
            }

            // Format links
            tuple = new Tuple<List<Class>,List<Link>>(tuple.Item1,tuple.Item2.Distinct().ToList());
            foreach (Link l in tuple.Item2)
            {
                if (l.From != l.To)
                    output += String.Format("{0}\n", l.ToString());
            }

            // Matches
            foreach (Tuple<List<string>, string> t in NLParser.Matches)
                foreach (string s in t.Item1)
                    output = Regex.Replace(output, @"\b" + s + @"\b", t.Item2);

            return output;
        }
    }


}
