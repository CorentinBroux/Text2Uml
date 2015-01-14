using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ITI.Text2UML.Model;
using ITI.Text2UML.Parsing.NaturalLanguage.Tools;
using ITI.Text2UML.Parsing.NaturalLanguage.UserInput;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    public static class NLParser
    {
        /// <summary>
        /// Lists all matches
        /// </summary>
        public static List<Tuple<List<string>, string>> Matches = new List<Tuple<List<string>, string>>();

        /// <summary>
        /// Counter for unknown types (named 'thing1', 'thing2'...)
        /// </summary>
        public static int j = 1;


        /// <summary>
        /// Gets lower tokens in the Stanford parser tree
        /// </summary>
        /// <param name="input">Input string from Standford parser tree</param>
        /// <returns>List of Tuple<string,string> where the Item1 is the Penn Treebank P.O.S. Tag <see cref="NLGrammar.Keywords"/> and the Item2 is the word value.</returns>
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

        /// <summary>
        /// Returns the Tree data structure from the Stanford parser
        /// </summary>
        /// <param name="input">Sentence</param>
        /// <returns>Tree</returns>
        public static Tree GetTree(string input)
        {
            // Initialize
            NLTokenizer tokenizer = new NLTokenizer(input);
            Node root = new Node("root");
            Node currentNode = root;

            tokenizer.GetNextToken();

            // Parse
            while (tokenizer.CurrentToken != NLTokenType.EndOfInput)
            {
                if (tokenizer.PreviousToken == NLTokenType.OpenPar)
                {
                    if (tokenizer.CurrentWordValue != "ROOT")
                    {
                        Node node = new Node(tokenizer.CurrentWordValue);
                        currentNode.AddChild(node);
                        currentNode = node;
                    }

                }
                if (tokenizer.CurrentToken == NLTokenType.ClosePar)
                    currentNode = currentNode.Parent;

                tokenizer.GetNextToken();
            }
            // {(ROOT (S (NP (DT This)) (VP (VBZ is) (NP (DT a) (NN test))) (. .)))}
            return new Tree(root);
        }

        /// <summary>
        /// Express the content on a List&lt;Tuple&lt;string, string&gt;&gt; in a single string without ends of line.
        /// </summary>
        /// <param name="tuples">Input List&lt;Tuple&lt;string, string&gt;&gt; to express.</param>
        /// <returns></returns>
        static string ExpressInLine(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string> t in tuples)
                builder.AppendFormat(" {0} {1}", t.Item1, t.Item2);
            if (builder.Length > 0)
                builder.Remove(0, 1); // Remove first char wich is a blank space
            return builder.ToString();
        }

        /// <summary>
        /// Parse the sentence.
        /// </summary>
        /// <param name="input">Single sentence</param>
        /// <returns>Returns the pseudo code representing the sentence structure and data.</returns>
        public static string Parse(string input, List<UserStructureSet> usss = null)
        {
            // Initialize
            List<Tuple<string, string>> tuples = GetLowLevelTokens(input);
            string type = ExpressInLine(tuples);
            // Parse

            //*********************PARSE WITH USER STRUCTURES*********************

            if (usss != null)
            {
                usss.RemoveAll(item => item == null);
                foreach (UserStructureSet uss in usss)
                    foreach (UserStructure us in uss.Structures)
                        switch (us.Type)
                        {
                            case UserStructureType.ByRegex:
                                if (Regex.Match(type, us.Input).Success)
                                    return us.Output;
                                break;
                            case UserStructureType.ByTree:
                                break;
                            default:
                                break;
                        }
            }

            //********************************************************************

            // Type definition (X is a type of Y || X and Y are types of Z)
            if (Regex.Match(type, "NN[A-Z]* [a-zA-Z]+ VB[A-Z]* is DT a NN[A-Z]* type IN of NN[A-Z]* [a-zA-Z]+").Success || Regex.Match(type, "(NN[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)*)+ VB[A-Z]* are NN[A-Z]* types IN of NN[A-Z]* [a-zA-Z]+").Success)
                return TypeDefinition(tuples);
            // Match (Match A, B, and C as D || Match A, B, C as D)
            if (Regex.Match(type, "[A-Z]+ (M|m)atch ([A-Z]+ [a-zA-Z]* (CC[A-Z]* [a-zA-Z]+ )*)+[A-Z]+ as [A-Z]+ [a-zA-Z]+").Success || Regex.Match(type, "NN[A-Z]* (M|m)atch NN[A-Z]* [A-Z]* [A-Z]+ as NN[A-Z]* [A-Z]+").Success)
            {
                Match(tuples);
                return "";
            }
            // Reverse Definition
            //else if (Regex.Match(type, "(DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ MD [a-zA-Z]+ VB[A-Z]* [a-zA-Z]+ (DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+").Success)
            else if (Regex.Match(type, "(DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ MD [a-zA-Z]+ VB[A-Z]*( [a-zA-Z]+)+").Success)
            {
                return ComplexDefinition(tuples, false, false, true);
            }
            // TEST : JJR / JJS
            else if (Regex.Match(type, "([a-zA-Z]+ )+JJ(R|S)( [a-zA-Z]+)+").Success)
            {
                return ComplexDefinition(tuples, false, false, false, true);
            }
            // Definition
            else if (Regex.Match(type, "((DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ )+VB[A-Z]* [a-zA-Z]+ (IN [a-zA-Z]+ (DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*(NN[A-Z]* [a-zA-Z]+ )*)+").Success)
            {
                return ComplexDefinition(tuples, false, true);
            }
            else if (Regex.Match(type, "((DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ )+VB[A-Z]* [a-zA-Z]+ ((DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*(NN[A-Z]* [a-zA-Z]+ )*)+").Success)
            {
                return ComplexDefinition(tuples);
            }
            // Beeing (eg "A tiny cat")
            else if (Regex.Match(type, "((DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+( )*)+").Success)
            {
                return ComplexDefinition(tuples, true);
            }
            // Action without complement
            else if (Regex.Match(type, "(DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ VB[A-Z]* [a-zA-Z]+").Success)
            {
                return SimpleAction(tuples);
            }

            // If not recognized
            return "Unknown";
        }


        // Specialization


        /// <summary>
        /// Returns the pseudo code for a complex definition sentence structure (adjectives noun verb adjectives noun).
        /// </summary>
        /// <param name="tuples">List<Tuple<string, string>> representing the sentence. Only pass List<Tuple<string, string>> tuples if sentence type is ComplexDefinition</param>
        static string ComplexDefinition(List<Tuple<string, string>> tuples, bool be = false, bool withIN = false, bool isModal = false, bool isJJRJJS = false)
        {
            if (withIN == true) // if "in", "on", "at"... 
            {
                tuples.Reverse();
            }

            bool isLastName = false;
            string verb = "";
            List<Tuple<int, string>> adjectives = new List<Tuple<int, string>>();
            List<Tuple<int, string>> lastAdjectives = new List<Tuple<int, string>>();

            List<Class> firstClasses = new List<Class>();
            List<Class> lastClasses = new List<Class>();
            List<Method> methods = new List<Method>();
            Method m = null;

            string min = "0", max = "n";

            string jrss = "";

            foreach (Tuple<string, string> t in tuples)
            {
                if (t.Item1.StartsWith("NN") || (t.Item1.StartsWith("CD") && verb == ""))
                {

                    if (isLastName == false)
                    {
                        firstClasses.Add(new Class(t.Item2));
                        foreach (Tuple<int, string> adj in adjectives)
                            firstClasses.Last().Attributes.Add(new Model.Attribute(String.Format("thing{0}", adj.Item1), adj.Item2));
                        adjectives.Clear();

                    }
                    else
                    {
                        lastClasses.Add(new Class(t.Item2));
                        foreach (Tuple<int, string> adj in lastAdjectives)
                            lastClasses.Last().Attributes.Add(new Model.Attribute(String.Format("thing{0}", adj.Item1), adj.Item2));
                        lastAdjectives.Clear();
                        if (m != null)
                        {
                            m.ParamTypes.Add(t.Item2);
                            m = null;
                        }
                    }
                }

                else if (t.Item1.StartsWith("CD"))
                {
                    //int i;
                    //Int32.TryParse(t.Item2, out i);
                    //jjrss.Add(new Tuple<jjrsType, int>(jjrs, i));
                    switch (jrss)
                    {
                        case "more":
                            min = StringToNumber(t.Item2).ToString();
                            break;
                        case "less":
                            max = StringToNumber(t.Item2).ToString();
                            break;
                        case "least":
                            min = StringToNumber(t.Item2).ToString();
                            break;
                        default:
                            min = StringToNumber(t.Item2).ToString();
                            max = StringToNumber(t.Item2).ToString();
                            break;
                    }
                }

                else if (t.Item1 == "JJ")
                {
                    if (isLastName == false)
                        adjectives.Add(new Tuple<int, string>(j, t.Item2));
                    else
                        lastAdjectives.Add(new Tuple<int, string>(j, t.Item2));

                    j++;
                }

                else if (t.Item1 == "JJS" || t.Item1 == "JJR")
                {
                    jrss = t.Item2;
                }
                else if (t.Item1.StartsWith("VB"))
                {
                    isLastName = true;
                    verb = t.Item2;
                    if (!NLGrammar.Verb_Be.Contains(verb) && !NLGrammar.Verb_Have.Contains(verb))
                    {
                        m = new Method("void", verb, new List<string>());
                        methods.Add(m);
                    }

                }

            }

            foreach (Method method in methods)
                foreach (Class c in firstClasses)
                    c.Methods.Add(method);

            if (isModal == true)
            {
                List<Class> temp = firstClasses;
                firstClasses = lastClasses;
                lastClasses = temp;
            }

            StringBuilder builder = new StringBuilder();

            if (lastClasses.Count == 0)
            {
                foreach (Class c in firstClasses)
                {
                    foreach (Tuple<int, string> adj in lastAdjectives)
                        c.Attributes.Add(new Model.Attribute(String.Format("thing{0}", adj.Item1), adj.Item2));
                    builder.AppendFormat("{0} ", c.ToString());
                }
                return builder.ToString();
            }


            if (NLGrammar.Verb_Be.Contains(verb) && withIN == false)
            {
                if (isModal && firstClasses.Count == 0)
                {
                    foreach (Class c2 in lastClasses)
                        foreach (Tuple<int, string> adj in lastAdjectives)
                            NLGrammar.Types.Add(new Tuple<string, string>(adj.Item2, c2.Name));

                }
                foreach (Class c in firstClasses)
                {
                    builder.AppendFormat("{0} ", c.ToString());
                    foreach (Class c2 in lastClasses)
                    {
                        builder.AppendFormat("{0} ", c2.ToString());
                        builder.AppendFormat("{0} -> {1} ", c.Name, c2.Name);
                        if (isModal == false)
                            NLGrammar.Types.Add(new Tuple<string, string>(c.Name, c2.Name));
                        else
                            NLGrammar.Types.Add(new Tuple<string, string>(c2.Name, c.Name));
                    }

                }

            }
            else if (NLGrammar.Verb_Have.Contains(verb) || withIN == true)
            {
                string linkLabel = "";
                if (isJJRJJS == true)
                {
                    linkLabel = String.Format("({0} {1})", min, max);
                }

                foreach (Class c in firstClasses)
                {
                    builder.AppendFormat("{0} ", c.ToString());
                    foreach (Class c2 in lastClasses)
                    {
                        builder.AppendFormat("{0} ", c2.ToString());
                        builder.AppendFormat("{0} --> {1} {2}", c.Name, c2.Name, linkLabel);
                    }

                }
            }
            else
            {
                foreach (Class c in firstClasses)
                    builder.AppendFormat("{0} ", c.ToString());
                foreach (Class c in lastClasses)
                    builder.AppendFormat("{0} ", c.ToString());
            }

            return builder.ToString();

        }

        /// <summary>
        /// Fill the 'Types' list with specific types defined by the user input ("X is a type of Y" defines a type 'Y' for each attribute named 'X', "X and Y are types of Z" defines a type 'Z' for each attribute named 'X' or 'Y')
        /// </summary>
        /// <param name="tuples">List<Tuple<string, string>> representing the sentence. Only pass List<Tuple<string, string>> tuples if sentence type is TypeDefinition</param>
        static string TypeDefinition(List<Tuple<string, string>> tuples)
        {
            List<string> names = new List<string>();
            foreach (Tuple<string, string> t in tuples)
                if (t.Item1.StartsWith("NN"))
                    names.Add(t.Item2);
            string type = names.Last();
            names.Remove(names.Last());
            StringBuilder builder = new StringBuilder();
            foreach (string name in names)
            {
                NLGrammar.Types.Add(new Tuple<string, string>(name, type));
                builder.AppendFormat(" {0} -> {1}", name.ToLower(), type);
            }
            return builder.ToString();
        }


        // Action

        /// <summary>
        /// Returns the pseudo code for a simple action sentence structure (adjectives noun verb).
        /// </summary>
        /// <param name="tuples">List<Tuple<string, string>> representing the sentence. Only pass List<Tuple<string, string>> tuples if sentence type is SimpleAction</param>
        static string SimpleAction(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();

            if (tuples[0].Item1 == "DT")
                tuples.RemoveAt(0);

            int i = tuples.Count - 1;
            while (i != 0)
            {
                if (i < 0)
                    break;
                if (tuples[i].Item1 == "DT")
                {
                    tuples.RemoveAt(i);
                    i--;
                }
                i--;
            }// tuples now contains ((JJ* )*NN* VB*)

            foreach (Tuple<string, string> t in tuples)
            {
                if (t.Item1.StartsWith("NN"))
                    builder.Insert(0, "Class " + t.Item2 + " ");

                if (t.Item1.StartsWith("JJ"))
                {
                    builder.AppendFormat("thing{0} {1} ", j, t.Item2);
                    j++;
                }


                if (t.Item1.StartsWith("VB") && !NLGrammar.Verb_Be.Contains(t.Item2))
                    builder.AppendFormat("void {0}()", t.Item2);
            }

            return builder.ToString();
        }

        // Match

        private static void Match(List<Tuple<string, string>> tuples)
        {
            Tuple<List<string>, string> matchTuple;
            List<string> list = new List<string>();
            string last = "";
            bool isAsReached = false;
            tuples.RemoveAt(0);// Remove the first noun which is "Match"
            foreach (Tuple<string, string> t in tuples)
            {
                if (t.Item1.StartsWith("IN") || t.Item1.StartsWith("RB"))
                {
                    isAsReached = true;
                }
                else if (!t.Item1.StartsWith("CC"))
                {
                    if (isAsReached == true)
                        last = t.Item2;
                    else
                        list.Add(t.Item2);
                }



            }

            matchTuple = new Tuple<List<string>, string>(list, last);
            Matches.Add(matchTuple);
        }


        private static int StringToNumber(string number)
        {
            int ii = 0;
            if (Int32.TryParse(number, out ii) == true)
                return ii;
            int n = 0;

            if (String.IsNullOrEmpty(number)) return 0;

            var dict = new Dictionary<string, int>();
            dict.Add("zero", 0);
            dict.Add("nought", 0);
            dict.Add("one", 1);
            dict.Add("two", 2);
            dict.Add("three", 3);
            dict.Add("four", 4);
            dict.Add("five", 5);
            dict.Add("six", 6);
            dict.Add("seven", 7);
            dict.Add("eight", 8);
            dict.Add("nine", 9);
            dict.Add("ten", 10);
            dict.Add("eleven", 11);
            dict.Add("twelve", 12);
            dict.Add("thirteen", 13);
            dict.Add("fourteen", 14);
            dict.Add("fifteen", 15);
            dict.Add("sixteen", 16);
            dict.Add("seventeen", 17);
            dict.Add("eighteen", 18);
            dict.Add("nineteen", 19);
            dict.Add("twenty", 20);
            dict.Add("thirty", 30);
            dict.Add("forty", 40);
            dict.Add("fifty", 50);
            dict.Add("sixty", 60);
            dict.Add("seventy", 70);
            dict.Add("eighty", 80);
            dict.Add("ninety", 90);
            dict.Add("hundred", 100);
            dict.Add("thousand", 1000);
            dict.Add("million", 1000000);

            // rough check whether it's a valid number
            string temp = number.ToLower().Trim().Replace(" and ", " ");
            string[] words = temp.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length > 1)
            {
                foreach (string word in words)
                {
                    if (!dict.ContainsKey(word)) return n;
                }
            }

            int i;
            int j;

            for (i = words.Length - 1; i > (-1); i--)
            {
                if (words[i] == "hundred")
                {
                    n = (dict[words[i - 1]] * dict[words[i]]) + n;
                    i--;
                }
                else if (words[i] == "thousand")
                {
                    string[] tmpwords = new string[i];
                    for (j = i - 1; j > -1; j--)
                        tmpwords[j] = words[j];

                    n = (StringToNumber(string.Join(" ", tmpwords)) * dict[words[i]]) + n;
                    i = i - tmpwords.Length;
                }
                else if (words[i] == "million")
                {
                    string[] tmpwords2 = new string[i];
                    for (j = i - 1; j > -1; j--)
                        tmpwords2[j] = words[j];

                    n = (StringToNumber(string.Join(" ", tmpwords2)) * dict[words[i]]) + n;
                    i = i - tmpwords2.Length;
                }
                else
                {
                    n = n + dict[words[i]];
                }
            }
            return n;
        }
    }
}
