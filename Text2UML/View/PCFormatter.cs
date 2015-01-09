using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITI.Text2UML.Parsing.PseudoCode;
using ITI.Text2UML.Model;
using MoreLinq;

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
                c.Attributes = c.Attributes.DistinctBy(x => x.Name).ToList();
                //c.Methods = c.Methods.Distinct().ToList();
                c.Methods = c.Methods.DistinctBy(x => x.Name).ToList();

                // Specify specials types (defined by user input)
                foreach (ITI.Text2UML.Model.Attribute a in c.Attributes)
                    foreach (Tuple<string, string> t in ITI.Text2UML.Parsing.NaturalLanguage.NLGrammar.Types)
                        if (a.Name.Equals(t.Item1, StringComparison.InvariantCultureIgnoreCase))
                            a.Type = t.Item2;

                //c.Attributes = c.Attributes.Distinct().ToList();
                //c.Methods = c.Methods.Distinct().ToList();
                //c.Attributes = RemoveDuplicates(c.Attributes);


                output += c.ToString();
            }

            // Format links
            foreach (Link l in tuple.Item2)
            {
                if (l.From != l.To)
                    output += String.Format("{0}\n", l.ToString());
            }

            return output;
        }


        private static List<T> RemoveDuplicates<T>(List<T> inputList)
        {
            List<T> outputList = new List<T>();
            foreach (T t in inputList)
                if (!outputList.Contains(t))
                    outputList.Add(t);
            return outputList;
        }
    }


}
