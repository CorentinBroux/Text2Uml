using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    public enum NLTokenType
    {
        None,
        Word,
        Keyword,
        OpenPar,
        ClosePar,
        EndOfInput,
        Error
    }


    public interface ITokenizer
    {
        NLTokenType CurrentToken { get; set; }

        NLTokenType GetNextToken();

        bool Match(NLTokenType t);
    }


    public class NLTokenizer : ITokenizer
    {
        // 1. Fields and properties
        string input;
        int currentPos;
        int maxPos;
        public NLTokenType CurrentToken { get; set; }
        public NLTokenType PreviousToken { get; set; }
        public NLTokenType PrePreviousToken { get; set; }
        public string WordValue { get; set; }
        public string PreviousWordValue { get; set; }
        public string PrePreviousWordValue { get; set; }
        bool IsEnd { get { return currentPos >= maxPos; } }

        // 2. Constructors
        public NLTokenizer(string s)
            : this(s, 0, s.Length)
        {
        }

        public NLTokenizer(string s, int startIndex)
            : this(s, startIndex, s.Length)
        {
        }

        public NLTokenizer(string s, int startIndex, int count)
        {
            CurrentToken = NLTokenType.None;
            input = s;
            currentPos = startIndex;
            maxPos = startIndex + count;
        }

        // 3. Methods
        // 3.1 Char methods
        char GetCurrentChar()
        {
            Debug.Assert(!IsEnd);
            return input[currentPos];
        }

        char GetNextChar()
        {
            Debug.Assert(!IsEnd);
            return input[currentPos++];
        }

        void MoveNext()
        {
            Debug.Assert(!IsEnd);
            ++currentPos;
        }

        // 3.2 Token methods

        public bool Match(NLTokenType t)
        {
            if (CurrentToken == t)
            {
                GetNextToken();
                return true;
            }
            return false;
        }


        public NLTokenType GetNextToken()
        {
            if (IsEnd)
                return CurrentToken = NLTokenType.EndOfInput;

            char c = GetNextChar();

            while (Char.IsWhiteSpace(c))
            {
                if (IsEnd)
                    return CurrentToken = NLTokenType.EndOfInput;
                c = GetNextChar();
            }

            switch (c)
            {
                case '(':
                    CurrentToken = NLTokenType.OpenPar;
                    break;
                case ')':
                    CurrentToken = NLTokenType.ClosePar;
                    break;
                default:
                    {
                        if (Char.IsLetter(c) || Char.IsDigit(c))
                        {
                            CurrentToken = NLTokenType.Word;
                            StringBuilder builder = new StringBuilder();
                            builder.Append(c);
                            while (!IsEnd && (Char.IsLetter(c = GetCurrentChar()) || Char.IsDigit(c)))
                            {
                                builder.Append(c);
                                MoveNext();
                            }
                            WordValue = builder.ToString();
                            if (PCGrammar.Keywords.Contains(WordValue))
                                CurrentToken = NLTokenType.Keyword;
                        }
                        else CurrentToken = NLTokenType.Error;
                        break;
                    }
            }
            return CurrentToken;
        }

        // 3.3 Utils
        static public string DumpTokens(string s)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Tokens in '{0}': ", s);
            NLTokenizer p = new NLTokenizer(s);
            while (p.GetNextToken() != NLTokenType.EndOfInput)
            {
                builder.Append(p.CurrentToken);
                builder.Append(", ");
            }
            builder.Append("<EOI>");
            return builder.ToString();
        }



    }
}
