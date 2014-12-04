using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ITI.Text2UML
{
    public enum TokenType
    {
        None,
        Word,
        Link,
        OpenCurly,
        CloseCurly,
        OpenPar,
        ClosePar,
        EndOfInput,
        Error
    }


    public interface ITokenizer
    {
        TokenType CurrentToken { get; }

        TokenType GetNextToken();

        bool Match(TokenType t);
    }


    public class Tokenizer : ITokenizer
    {
        // 1. Fields and properties
        string input;
        int currentPos;
        int maxPos;
        TokenType currentToken;
        string wordValue;
        bool IsEnd { get { return currentPos >= maxPos; } }

        // 2. Constructors
        public Tokenizer( string s )
            : this( s, 0, s.Length )
        {
        }

        public Tokenizer( string s, int startIndex )
            : this( s, startIndex, s.Length )
        {
        }

        public Tokenizer( string s, int startIndex, int count )
        {
            currentToken = TokenType.None;
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
            public TokenType CurrentToken
            {
                get { return currentToken; }
            }

            public bool Match(TokenType t)
            {
                if (currentToken == t)
                {
                    GetNextToken();
                    return true;
                }
                return false;
            }


            public TokenType GetNextToken()
            {
                if (IsEnd)
                    return currentToken = TokenType.EndOfInput;

                char c = GetNextChar();

                while (Char.IsWhiteSpace(c))
                {
                    if (IsEnd)
                        return currentToken = TokenType.EndOfInput;
                    c = GetNextChar();
                }

                switch (c)
                {
                    case '{':
                        currentToken = TokenType.OpenCurly;
                        break;
                    case '}':
                        currentToken = TokenType.CloseCurly;
                        break;
                    case '(':
                        currentToken = TokenType.OpenPar;
                        break;
                    case ')':
                        currentToken = TokenType.ClosePar;
                        break;
                    default:
                        {
                            if (Char.IsLetter(c))
                            {
                                currentToken = TokenType.Word;
                                StringBuilder builder = new StringBuilder();
                                while (!IsEnd && Char.IsLetter(c = GetCurrentChar()))
                                {
                                    builder.Append(c);
                                    MoveNext();
                                }
                                wordValue = builder.ToString();
                            }
                            else if (c == '-')
                            {
                                currentToken = TokenType.Link;
                                StringBuilder builder = new StringBuilder();
                                while (!IsEnd && !Char.IsWhiteSpace(c = GetCurrentChar()))
                                {
                                    builder.Append(c);
                                    MoveNext();
                                }
                                wordValue = builder.ToString();
                            }
                            else currentToken = TokenType.Error;
                            break;
                        }
                }
                return currentToken;
            }

            // 3.3 Utils
            static public string DumpTokens(string s)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("Tokens in '{0}': ", s);
                Tokenizer p = new Tokenizer(s);
                while (p.GetNextToken() != TokenType.EndOfInput)
                {
                    builder.Append(p.CurrentToken);
                    builder.Append(", ");
                }
                builder.Append("<EOI>");
                return builder.ToString();
            }

        

    }

    #region old
    //public enum TokenType
    //{
    //    EoF,			//End of File
    //    EoL,			//End of Line
    //    Keyword,		//Keyword
    //    Delimiter,      //Delimiter
    //    LinkSymbol,     //Link symbol
    //    Other		    //Parameter
    //};

    //public enum FirstLevelKeyword
    //{
    //    Class,
    //    Interface,
    //    Abstract
    //};

    //public enum SecondLevelKeyword
    //{
    //    Attributes,
    //    Methods
    //};

    //public enum LinkKeyword
    //{
    //    Link
    //};

    //public class Token
    //{
    //    #region Fields and properties
    //    public TokenType Type {get;set;}
    //    public int Column { get; set; }
    //    public int Line { get; set; }
    //    public string Value { get; set; }

    //    public bool IsEoF{get{return Type==TokenType.EoF;}}
    //    public bool IsEoL{get{return Type==TokenType.EoL;}}
    //    public bool IsKeyword{get{return Type==TokenType.Keyword;}}
    //    public bool IsOther{get{return Type==TokenType.Other;}}
    //    public bool IsDelimiter { get { return Type == TokenType.Delimiter; } }
    //    public bool IsLinkSymbol { get { return Type == TokenType.LinkSymbol; } }

    //    #endregion

    //    #region Constructors
    //    public Token(TokenType type, string value = null, int col = 0, int line = 0)
    //    {
    //        Type = type;
    //        Column = col;
    //        Line = line;
    //        Value = value;
    //    }
    //    #endregion

    //}
    //public class Tokenizer
    //{
    //    #region Fields and properties        
    //    private static List<char> endOfLineDelimiters = new List<char>() { '\n', '\r' };
    //    private static List<char> otherDelimiters = new List<char>() { '{', '}','(',')' };
    //    private static List<string> keyWords = new List<string>() { "Class", "Interface", "Abstract","Attributes","Methods", "Links" };
    //    private static List<string> linkSymbols = new List<string>() { "<>-","<->-","->>",".>","-","-()",".>>","-(","<.","->","><-"};
    //    public string Text {get;set;}
    //    private int currentCharIndex;
    //    public char CurrentChar { get { return currentCharIndex >= Text.Length ? '0': Text[currentCharIndex];}}
    //    public char FollowingChar { get { return currentCharIndex +1 >= Text.Length ? '0' : Text[currentCharIndex+1]; } }
    //    public Token CurrentToken { get; set; }
    //    public int CurrentColumn { get; set; }
    //    public int CurrentLine { get; set; }
    //    public StringBuilder Buffer = new StringBuilder();
    //    public string StringBuffer { get { return Buffer.ToString(); } }
    //    #endregion


    //    #region Constructors
    //    public Tokenizer(string text = "")
    //    {
    //        Text = text;
    //    }
    //    #endregion


    //    #region Methods

    //    internal Token GoToNextToken()
    //    {

    //        int tokenColumn = CurrentColumn;
    //        Buffer.Clear();


    //        if (EndOfFile())
    //            return new Token(TokenType.EoF, "EoF", tokenColumn, CurrentLine);


    //        do
    //        {
    //            if(!Char.IsWhiteSpace(CurrentChar))
    //                Buffer.Append(CurrentChar);
    //        } while (!Char.IsWhiteSpace(NextChar()));


    //        if (CurrentChar == '\n')
    //        {
    //            if (CurrentChar == '\n') NextChar();
    //            return new Token(TokenType.EoL, "\n", tokenColumn, CurrentLine);
    //        }

    //        if (StringBuffer == "")
    //            Buffer.Append(NextChar());

            
    //        if (otherDelimiters.Contains(StringBuffer[0]))
    //            return new Token(TokenType.Delimiter, StringBuffer[0].ToString(), tokenColumn, CurrentLine);
            

    //        return new Token(keyWords.Contains(StringBuffer) ? TokenType.Keyword : linkSymbols.Contains(StringBuffer) ? TokenType.LinkSymbol : TokenType.Other, StringBuffer, tokenColumn, CurrentLine);


            
    //    }

    //    private char NextChar()
    //    {
    //        CurrentColumn++;
    //        currentCharIndex++;
    //        return currentCharIndex >= Text.Length ? ' ' : Text[currentCharIndex];
    //    }


 
    //    private bool EndOfFile()
    //    {
    //        try
    //        {
    //            char c = Text[currentCharIndex+1];
    //            return false;
    //        }catch(IndexOutOfRangeException){
    //            return true;
    //        }
    //    }


    //    #endregion
    //}
#endregion
}
