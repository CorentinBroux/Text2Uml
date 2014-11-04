﻿using System;
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
        Delimiter,      // Delimiter
		Other		    //Parameter
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
        private static List<string> keyWords = new List<string>() { "$Class", "$Interface", "$Abstract" };
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
            //if (!EatWhiteSpace()) return new Token(TokenType.EOF);

            int tokenColumn = CurrentColumn;
            //_buffer.Clear();

            if (endOfLineDelimiters.Contains(CurrentChar))
            {
                NextChar();
                return new Token(TokenType.EoL, "EoL", tokenColumn, CurrentLine);
            }

            if (otherDelimiters.Contains(CurrentChar))
            {
                char c = CurrentChar;
                NextChar();
                return new Token(TokenType.Delimiter, c.ToString(), tokenColumn, CurrentLine);
            }

            do
            {
                Buffer.Append(CurrentChar);
            } while (!Char.IsWhiteSpace(NextChar()));

            return new Token(keyWords.Contains(StringBuffer) ? TokenType.Keyword : TokenType.Other, StringBuffer, tokenColumn, CurrentLine);

            //if (false)//BeginRead())
            //{
            //    while (false)//!EndRead())
            //    {
            //        //if (IsEscapedStringChar()) NextChar();
            //        Buffer.Append(CurrentChar);
            //        NextChar();
            //    }
            //    NextChar();
            //    return new Token(TokenType.Other, StringBuffer, tokenColumn, CurrentLine);
            //}
            //else
            //{
            //    Buffer.Append(CurrentChar);
            //    while (!Char.IsWhiteSpace(NextChar())) Buffer.Append(CurrentChar);
            //    return new Token(TokenType.Keyword, StringBuffer, tokenColumn, CurrentLine);
            //}
        }

        private char NextChar()
        {
            CurrentColumn++;
            currentCharIndex++;
            return currentCharIndex >= Text.Length ? ' ' : Text[currentCharIndex];
        }

        //private bool BeginRead()
        //{
        //    if (othersDelimiters.Contains(CurrentChar))
        //    {
        //        //_beginCharString = CurrentChar;
        //        NextChar();
        //        return true;
        //    }
        //    return false;
        //}
        //private bool EndRead()
        //{
        //    return othersDelimiters.Contains(CurrentChar) || endOfLineDelimiters.Contains(CurrentChar);
        //}
        #endregion
    }
}
