using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ITI.Text2UML.Parsing.PseudoCode
{
    // Defines token types for pseudo code
    public enum PCTokenType
    {
        None,
        Word,
        Keyword,
        Link,
        OpenCurly,
        CloseCurly,
        OpenPar,
        ClosePar,
        EndOfInput,
        Error
    }



    public class PCTokenizer
    {
        // 1. Fields and properties
        string input;
        int currentPos;
        int maxPos;
        public PCTokenType CurrentToken { get; set; }
        public string CurrentWordValue { get; private set; }
        bool IsEnd { get { return currentPos >= maxPos; } }

        // 2. Constructors
        public PCTokenizer( string s )
            : this( s, 0, s.Length )
        {
        }

        public PCTokenizer( string s, int startIndex )
            : this( s, startIndex, s.Length )
        {
        }

        public PCTokenizer( string s, int startIndex, int count )
        {
            CurrentToken = PCTokenType.None;
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
        public bool Match(PCTokenType type)
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
        public PCTokenType GetNextToken()
        {
            if (IsEnd)
                return CurrentToken = PCTokenType.EndOfInput;

            char c = GetNextChar();

            while (Char.IsWhiteSpace(c))
            {
                if (IsEnd)
                    return CurrentToken = PCTokenType.EndOfInput;
                c = GetNextChar();
            }

            switch (c)
            {
                case '{':
                    CurrentToken = PCTokenType.OpenCurly;
                    break;
                case '}':
                    CurrentToken = PCTokenType.CloseCurly;
                    break;
                case '(':
                    CurrentToken = PCTokenType.OpenPar;
                    break;
                case ')':
                    CurrentToken = PCTokenType.ClosePar;
                    break;
                default:
                    {
                        if (Char.IsLetter(c) || Char.IsDigit(c))
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append(c);
                            while (!IsEnd && (Char.IsLetter(c = GetCurrentChar()) || Char.IsDigit(c)))
                            {
                                builder.Append(c);
                                MoveNext();
                            }
                            CurrentWordValue = builder.ToString();
                            if (PCGrammar.Keywords.Contains(CurrentWordValue) && CurrentToken != PCTokenType.Keyword) // Avoid "Class Class" errors
                                CurrentToken = PCTokenType.Keyword;
                            else
                                CurrentToken = PCTokenType.Word;
                        }
                        else if (c == '-')
                        {
                            CurrentToken = PCTokenType.Link;
                            StringBuilder builder = new StringBuilder();
                            builder.Append(c);
                            while (!IsEnd && !Char.IsWhiteSpace(c = GetCurrentChar()))
                            {
                                builder.Append(c);
                                MoveNext();
                            }
                            CurrentWordValue = builder.ToString();
                            if (!PCGrammar.Links.Contains(CurrentWordValue))
                                CurrentToken = PCTokenType.Error;
                        }
                        else CurrentToken = PCTokenType.Error;
                        break;
                    }
            }
            return CurrentToken;
        }

        

    }

}
