using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    // Defines token types for natural language
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


    public class NLTokenizer
    {
        // 1. Fields and properties
        string input;
        int currentPos;
        int maxPos;
        public NLTokenType CurrentToken { get; set; }
        public NLTokenType PreviousToken { get; set; }
        public NLTokenType PrePreviousToken { get; set; }
        public string CurrentWordValue { get; set; }
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
        
        /// <summary>
        /// Gets the current read craracter in the input stream.
        /// </summary>
        /// <returns></returns>
        char GetCurrentChar()
        {
            Debug.Assert(!IsEnd);
            return input[currentPos];
        }

        /// <summary>
        /// Moves to and gets the next character in the input stream.
        /// </summary>
        /// <returns></returns>
        char GetNextChar()
        {
            Debug.Assert(!IsEnd);
            return input[currentPos++];
        }

        /// <summary>
        /// Moves to the next character in the input stream.
        /// </summary>
        void MoveNext()
        {
            Debug.Assert(!IsEnd);
            ++currentPos;
        }

        // 3.2 Token methods

        /// <summary>
        /// Checks if the current toen mathes a specific type.
        /// </summary>
        /// <param name="type">Token type to compare to.</param>
        /// <returns></returns>
        public bool Match(NLTokenType type)
        {
            if (CurrentToken == type)
            {
                GetNextToken();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the next token in the input stream.
        /// </summary>
        /// <returns></returns>
        public NLTokenType GetNextToken()
        {
            PrePreviousToken = PreviousToken;
            PreviousToken = CurrentToken;
            PrePreviousWordValue = PreviousWordValue;
            PreviousWordValue = CurrentWordValue;
            
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
                        if (Char.IsLetter(c) || Char.IsDigit(c) || Char.IsPunctuation(c))
                        {
                            CurrentToken = NLTokenType.Word;
                            StringBuilder builder = new StringBuilder();
                            builder.Append(c);
                            while (!IsEnd && (Char.IsLetter(c = GetCurrentChar()) || Char.IsDigit(c)))
                            {
                                builder.Append(c);
                                MoveNext();
                            }
                            CurrentWordValue = builder.ToString();
                            if (NLGrammar.Keywords.Contains(CurrentWordValue))
                                CurrentToken = NLTokenType.Keyword;
                        }
                        else CurrentToken = NLTokenType.Error;
                        break;
                    }
            }
            return CurrentToken;
        }


    }
}
