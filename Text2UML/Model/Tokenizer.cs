using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    public enum TokenType
    {
        EoF,			//End of File
		EoL,			//End of Line
		Keyword,		//Keyword
        Delimiter,      //Delimiter
        LinkSymbol,     //Link symbol
		Other		    //Parameter
    };

    public enum FirstLevelKeyword
    {
        Class,
        Interface,
        Abstract
    };

    public enum SecondLevelKeyword
    {
        Attributes,
        Methods
    };

    public enum LinkKeyword
    {
        Link
    };

    class Token
    {
        #region Fields and properties
        public TokenType Type {get;set;}
		public int Column { get; set; }
		public int Line { get; set; }
		public string Value { get; set; }

        public bool IsEoF{get{return Type==TokenType.EoF;}}
        public bool IsEoL{get{return Type==TokenType.EoL;}}
        public bool IsKeyword{get{return Type==TokenType.Keyword;}}
        public bool IsOther{get{return Type==TokenType.Other;}}
        public bool IsDelimiter { get { return Type == TokenType.Delimiter; } }
        public bool IsLinkSymbol { get { return Type == TokenType.LinkSymbol; } }

        #endregion

        #region Constructors
		public Token(TokenType type, string value = null, int col = 0, int line = 0)
		{
			Type = type;
			Column = col;
			Line = line;
			Value = value;
		}
        #endregion

    }
    class Tokenizer
    {
        #region Fields and properties        
        private static List<char> endOfLineDelimiters = new List<char>() { '\n', '\r' };
        private static List<char> otherDelimiters = new List<char>() { '{', '}','(',')' };
        private static List<string> keyWords = new List<string>() { "Class", "Interface", "Abstract","Attributes","Methods", "Links" };
        private static List<string> linkSymbols = new List<string>() { "<>-","<->-","->>",".>","-","-()",".>>","-(","<.","->","><-"};
        public string Text {get;set;}
        private int currentCharIndex;
        public char CurrentChar { get { return currentCharIndex >= Text.Length ? '0': Text[currentCharIndex];}}
        public char FollowingChar { get { return currentCharIndex +1 >= Text.Length ? '0' : Text[currentCharIndex+1]; } }
        public Token CurrentToken { get; set; }
        public int CurrentColumn { get; set; }
        public int CurrentLine { get; set; }
        public StringBuilder Buffer = new StringBuilder();
        public string StringBuffer { get { return Buffer.ToString(); } }
        #endregion


        #region Constructors
        public Tokenizer(string text = "")
        {
            Text = text;
        }
        #endregion


        #region Methods

        internal Token GoToNextToken()
        {

            int tokenColumn = CurrentColumn;
            Buffer.Clear();


            if (EndOfFile())
                return new Token(TokenType.EoF, "EoF", tokenColumn, CurrentLine);


            do
            {
                if(!Char.IsWhiteSpace(CurrentChar))
                    Buffer.Append(CurrentChar);
            } while (!Char.IsWhiteSpace(NextChar()));


            if (CurrentChar == '\n')
            {
                if (CurrentChar == '\n') NextChar();
                return new Token(TokenType.EoL, "\n", tokenColumn, CurrentLine);
            }

            if (StringBuffer == "")
                Buffer.Append(NextChar());

            
            if (otherDelimiters.Contains(StringBuffer[0]))
                return new Token(TokenType.Delimiter, StringBuffer[0].ToString(), tokenColumn, CurrentLine);
            

            return new Token(keyWords.Contains(StringBuffer) ? TokenType.Keyword : linkSymbols.Contains(StringBuffer) ? TokenType.LinkSymbol : TokenType.Other, StringBuffer, tokenColumn, CurrentLine);


            
        }

        private char NextChar()
        {
            CurrentColumn++;
            currentCharIndex++;
            return currentCharIndex >= Text.Length ? ' ' : Text[currentCharIndex];
        }


 
        private bool EndOfFile()
        {
            try
            {
                char c = Text[currentCharIndex+1];
                return false;
            }catch(IndexOutOfRangeException){
                return true;
            }
        }


        #endregion
    }
}
