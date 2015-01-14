using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITI.Text2UML.Parsing.PseudoCode;
using ITI.Text2UML.Model;
using MoreLinq;
using ITI.Text2UML.Parsing.NaturalLanguage;
using System.Text.RegularExpressions;
using System.Globalization;


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

            //Service to pluralize/singularize words
            var PLurService = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

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

            }

            // Remove classes which are personnal types
            List<string> types = new List<string>();
            List<string> adj = new List<string>();
            foreach (Tuple<string, string> t in NLGrammar.Types)
            {
                types.Add(t.Item1.ToLower());
                types.Add(t.Item2.ToLower());
            }
            foreach (Class c in tuple.Item1)
                foreach (ITI.Text2UML.Model.Attribute a in c.Attributes)
                {
                    adj.Add(a.Name.ToLower());
                    adj.Add(a.Type.ToLower());
                }

            List<Class> classes = new List<Class>();
            foreach (Class c in tuple.Item1)
                if (!(types.Contains(c.Name.ToLower()) && adj.Contains(c.Name.ToLower())))
                {
                    classes.Add(c);
                    output += c.ToString();
                }
            tuple = new Tuple<List<Class>, List<Link>>(classes, tuple.Item2);

            // Format links
            tuple = new Tuple<List<Class>, List<Link>>(tuple.Item1, tuple.Item2.Distinct().ToList());
            foreach (Link l in tuple.Item2)
            {
                if (l.From != l.To)
                    output += String.Format("{0}\n", l.ToString());
            }

            
            // Matches
            foreach (Tuple<List<string>, string> t in NLParser.Matches)
                foreach (string s in t.Item1)
                    output = Regex.Replace(output, @"\b" + s + @"\b", t.Item2);

            string[] split = output.Split(new Char[] { ' ', ',', '.', ':', '\t', '!', '?', '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);


            foreach (string s in split)
            {
                try
                {
                    output = Regex.Replace(output, @"\b" + s + @"\b", PLurService.Singularize(s));
                }
                catch
                {
                    continue;
                }
            }

                

            return output.ToLower();
        }
    }


}
