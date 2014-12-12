using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ITI.Text2UML.Model;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    public static class NLParser
    {
        public static List<Tuple<string, string>> GetLowLevelTokens(string input)
        {
            // Initialize
            NLTokenizer tokenizer = new NLTokenizer(input);
            List<Tuple<string, string>> tuples = new List<Tuple<string, string>>();

            // Parse
            while (tokenizer.CurrentToken != NLTokenType.EndOfInput)
            {
                if (tokenizer.CurrentToken == NLTokenType.ClosePar && tokenizer.PreviousToken == NLTokenType.Word && tokenizer.PrePreviousToken == NLTokenType.Keyword)
                {
                    tuples.Add(new Tuple<string, string>(tokenizer.PrePreviousWordValue, tokenizer.PreviousWordValue));
                }
                tokenizer.GetNextToken();
            }
            return tuples;
        }

        static string ExpressInLine(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string> t in tuples)
                builder.AppendFormat(" {0} {1}", t.Item1, t.Item2);
            return builder.ToString();
        }

        public static string Parse(string input)
        {
            // Initialize
            List<Tuple<string, string>> tuples = GetLowLevelTokens(input);
            string type = ExpressInLine(tuples);

            // Parse
                // Reverse Definition
                if (Regex.Match(type, "(DT [a-zA-Z]+)*( JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)*)* NN[A-Z]* [a-zA-Z]+ MD [a-zA-Z]+ VB[A-Z]* [a-zA-Z]+( DT [a-zA-Z]+)*( JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)*)* NN[A-Z]* [a-zA-Z]+").Success)
                {
                    int i = tuples.Count - 1;
                    while (i != 0)
                    {
                        if (tuples[i].Item1 == "DT" || tuples[i].Item1 == "MD")
                        {
                            tuples.RemoveAt(i);
                            i--;
                        }
                        i--;
                    }// tuples now contains 3 elements (NN* VB* NN*), but due to the modal, we have to reverse them
                    tuples.Reverse();
                    return SimpleDefinition(tuples);
                }
                // Definition
                else if (Regex.Match(type, "(DT [a-zA-Z]+)*( JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)*)* NN[A-Z]* [a-zA-Z]+ VB[A-Z]* [a-zA-Z]+( DT [a-zA-Z]+)*( JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)*)* NN[A-Z]* [a-zA-Z]+").Success)
                {
                    return ComplexDefinition(tuples);
                }
                // Action without complement
                else if (Regex.Match(type, "(DT [a-zA-Z]+)* NN[A-Z]* [a-zA-Z]+ VB[A-Z]* [a-zA-Z]+").Success)
                {
                    return SimpleAction(tuples);
                }
                
            return "Unknown";
        }


        // Specialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples">Only pass List<Tuple<string, string>> tuples if sentence type is SimpleDefinition</param>
        static string SimpleDefinition(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();

            if (tuples[0].Item1 == "DT")
                tuples.RemoveAt(0);

            int i = tuples.Count - 1;
            while(i !=0 )
            {
                if(tuples[i].Item1 == "DT")
                {
                    tuples.RemoveAt(i);
                    i--;
                }
                i--;
            }// tuples now contains 3 elements (NN* VB* NN*)

            if (NLGrammar.Verb_Be.Contains(tuples[1].Item2))
                builder.AppendFormat("Class {0} Class {1} {0} -> {1}", tuples[0].Item2, tuples[2].Item2);
            else if (NLGrammar.Verb_Have.Contains(tuples[1].Item2))
                builder.AppendFormat("Class {0} {1} {2} ", tuples[0].Item2, "Object", tuples[2].Item2);
            else
                builder.AppendFormat("Class {0} void {1}({2})", tuples[0].Item2, tuples[1].Item2, tuples[2].Item2);
            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples">Only pass List<Tuple<string, string>> tuples if sentence type is SimpleDefinition</param>
        static string ComplexDefinition(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();

            if (tuples[0].Item1 == "DT")
                tuples.RemoveAt(0);

            int i = tuples.Count - 1;
            while (i != 0)
            {
                if (tuples[i].Item1 == "DT")
                {
                    tuples.RemoveAt(i);
                    i--;
                }
                i--;
            }// tuples now contains elements ((JJ* )*NN* VB* (JJ* )*NN*)

            bool isFirstName = true;
            string name1 = "", name2 = "", verb = "";
            int j = 1;
            foreach (Tuple<string, string> t in tuples)
            {
                if (t.Item1.StartsWith("NN"))
                {
                    if(isFirstName == true)
                    {
                        name1 = t.Item2;
                        isFirstName = false;
                        j = 1;
                        builder.Insert(0, "Class " + t.Item2 + " ");
                    }
                    else
                    {
                        name2 = t.Item2;
                        builder2.Insert(0, "Class " + t.Item2 + " ");
                    }
                }

                if(t.Item1.StartsWith("JJ"))
                {
                    if (isFirstName == true)
                        builder.AppendFormat("thing{0} {1} ", j, t.Item2);
                    else
                        builder2.AppendFormat("thing{0} {1} ", j, t.Item2);
                    j++;
                }

                if (t.Item1.StartsWith("VB"))
                    verb = t.Item2;
            }

            builder.Append(" " + builder2.ToString());
            builder.Append(SimpleDefinition(new List<Tuple<string, string>>() { new Tuple<string, string>("NN", name1), new Tuple<string, string>("VB", verb), new Tuple<string, string>("NN", name2) }));
            return builder.ToString();
        }


        // Action
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples">Only pass List<Tuple<string, string>> tuples if sentence type is SimpleAction</param>
        static string SimpleAction(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();

            if (tuples[0].Item1 == "DT")
                tuples.RemoveAt(0);

            int i = tuples.Count - 1;
            while (i != 0)
            {
                if (tuples[i].Item1 == "DT")
                {
                    tuples.RemoveAt(i);
                    i--;
                }
                i--;
            }// tuples now contains 2 elements (NN* VB*)

            builder.AppendFormat("Class {0} void {1}()", tuples[0].Item2, tuples[1].Item2);
            return builder.ToString();
        }
    }
}
