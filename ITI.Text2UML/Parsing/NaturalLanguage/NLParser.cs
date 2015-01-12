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
                    if(tokenizer.CurrentWordValue != "ROOT")
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
        /// Express the content on a List<Tuple<string, string>> in a single string without ends of line.
        /// </summary>
        /// <param name="tuples">Input List<Tuple<string, string>> to express.</param>
        /// <returns></returns>
        static string ExpressInLine(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string> t in tuples)
                builder.AppendFormat(" {0} {1}", t.Item1, t.Item2);
            if(builder.Length>0)
                builder.Remove(0, 1); // Remove first char wich is a blank space
            return builder.ToString();
        }

        /// <summary>
        /// Parse the sentence.
        /// </summary>
        /// <param name="input">Single sentence</param>
        /// <returns>Returns the pseudo code representing the sentence structure and data.</returns>
        public static string Parse(string input, UserStructureSet uss = null)
        {
            // Initialize
            List<Tuple<string, string>> tuples = GetLowLevelTokens(input);
            string type = ExpressInLine(tuples);
            // Parse

            //*********************PARSE WITH USER STRUCTURES*********************

            if (uss != null)
            {
                foreach (UserStructure s in uss.Structures)
                    switch(s.Type)
                    {
                        case UserStructureType.ByRegex:
                            if (Regex.Match(type, s.Input).Success)
                            return s.Output;
                            break;
                        case UserStructureType.ByTree:
                            break;
                        default :
                            break;
                    }   
            }

            //********************************************************************

            // Type definition (X is a type of Y || X and Y are types of Z)
            if (Regex.Match(type, "NN[A-Z]* [a-zA-Z]+ VB[A-Z]* is DT a NN[A-Z]* type IN of NN[A-Z]* [a-zA-Z]+").Success || Regex.Match(type, "(NN[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)*)+ VB[A-Z]* are NN[A-Z]* types IN of NN[A-Z]* [a-zA-Z]+").Success)
                return TypeDefinition(tuples);
            // Match (Match A, B, and C as D || Match A, B, C as D)
            if (Regex.Match(type, "[A-Z]+ (M|m)atch ([A-Z]+ [a-zA-Z]* (CC[A-Z]* [a-zA-Z]+ )*)+IN as [A-Z]+ [a-zA-Z]+").Success || Regex.Match(type, "NN[A-Z]* (M|m)atch NN[A-Z]* [A-Z]* IN as NN[A-Z]* [A-Z]+").Success)
            {
                Match(tuples);
                return "";
            }
            // Beeing (eg "A tiny cat")
            else if (Regex.Match(type, "(DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+").Success)
            {
                return ComplexDefinition(tuples);
            }
            // Reverse Definition
            else if (Regex.Match(type, "(DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ MD [a-zA-Z]+ VB[A-Z]* [a-zA-Z]+ (DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+").Success)
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
                    if (i < 0)
                        break;
                }// tuples now contains 3 elements (NN* VB* NN*), but due to the modal, we have to reverse them
                tuples.Reverse();
                return SimpleDefinition(tuples);
            }
            // Definition
            else if (Regex.Match(type, "((DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+ )+VB[A-Z]* [a-zA-Z]+ (DT [a-zA-Z]+ )*(JJ[A-Z]* [a-zA-Z]+( CC[A-Z]* [a-zA-Z]+)* )*NN[A-Z]* [a-zA-Z]+").Success)
            {
                return ComplexDefinition(tuples);
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
        /// Returns the pseudo code for a simple definition sentence structure (noun verb noun).
        /// </summary>
        /// <param name="tuples">List<Tuple<string, string>> representing the sentence. Only pass List<Tuple<string, string>> tuples if sentence type is SimpleDefinition</param>
        static string SimpleDefinition(List<Tuple<string, string>> tuples)
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
            }// tuples now contains 3 elements (NN* VB* NN*)

            if (String.IsNullOrEmpty(tuples[1].Item2))
                //builder.AppendFormat("Class {0} ", tuples[0].Item2);
                builder.Append("");
            else
                if (NLGrammar.Verb_Be.Contains(tuples[1].Item2))
                    builder.AppendFormat("Class {0} Class {1} {0} -> {1} ", tuples[0].Item2, tuples[2].Item2);
                else if (NLGrammar.Verb_Have.Contains(tuples[1].Item2))
                    builder.AppendFormat("Class {0} {1} {2} {0} --> {2} ", tuples[0].Item2, "Object", tuples[2].Item2);
                else
                    builder.AppendFormat("Class {0} void {1}({2}) ", tuples[0].Item2, tuples[1].Item2, tuples[2].Item2);
            return builder.ToString();
        }

        /// <summary>
        /// Returns the pseudo code for a complex definition sentence structure (adjectives noun verb adjectives noun).
        /// </summary>
        /// <param name="tuples">List<Tuple<string, string>> representing the sentence. Only pass List<Tuple<string, string>> tuples if sentence type is ComplexDefinition</param>
        static string ComplexDefinition(List<Tuple<string, string>> tuples)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();

            if (tuples[0].Item1 == "DT")
                tuples.RemoveAt(0);

            int i = tuples.Count - 1;
            while (i != 0)
            {
                if (i < 0)
                    break;
                if (tuples[i].Item1 == "DT" || tuples[i].Item1 == "CC")
                {
                    tuples.RemoveAt(i);
                    i--;
                }
                i--;
            }// tuples now contains elements (((JJ* )*NN*)+ VB* (JJ* )*NN*)

            bool isLastName = false;
            string lastName = "", verb = "";
            List<string> firstNames = new List<string>();
            int pos = 0; // position to insert names to
            bool isNewName = false;
            foreach (Tuple<string, string> t in tuples)
            {
                if (t.Item1.StartsWith("NN"))
                {
                    if (isLastName == false)
                    {
                        isNewName = true;
                        firstNames.Add(t.Item2);
                        builder.Insert(pos, "Class " + t.Item2 + " ");
                    }
                    else
                    {
                        lastName = t.Item2;
                        builder2.Insert(0, "Class " + t.Item2 + " ");
                    }
                }

                if (t.Item1.StartsWith("JJ"))
                {
                    if (isLastName == false)
                    {
                        if(isNewName == true)
                            pos = builder.Length;
                        builder.AppendFormat("thing{0} {1} ", j, t.Item2);
                    }
                    else
                        builder2.AppendFormat("thing{0} {1} ", j, t.Item2);
                    j++;
                }

                if (t.Item1.StartsWith("VB"))
                {
                    isLastName = true;
                    verb = t.Item2;
                }
                    
            }

            builder.Append(" " + builder2.ToString());
            foreach(string name in firstNames)
                builder.Append(SimpleDefinition(new List<Tuple<string, string>>() { new Tuple<string, string>("NN", name), new Tuple<string, string>("VB", verb), new Tuple<string, string>("NN", lastName) }));
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
                if (t.Item1.StartsWith("IN"))
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
    }
}
