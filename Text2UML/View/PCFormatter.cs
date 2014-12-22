using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITI.Text2UML.Parsing.PseudoCode;
using ITI.Text2UML.Model;


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
                c.Attributes = c.Attributes.Distinct().ToList();
                c.Methods = c.Methods.Distinct().ToList();

                // Specify specials types (defined by user input)
                foreach (ITI.Text2UML.Model.Attribute a in c.Attributes)
                    foreach (Tuple<string, string> t in ITI.Text2UML.Parsing.NaturalLanguage.NLGrammar.Types)
                        if (a.Name.Equals(t.Item1,StringComparison.InvariantCultureIgnoreCase))
                            a.Type = t.Item2;

                output += c.ToString();
            }

            // Format links
            foreach (Link l in tuple.Item2)
            {
                output += String.Format("{0}\n", l.ToString());
            }

            return output;
        }
    }

    
}
