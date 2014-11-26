using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML
{
    public class Formatter
    {
        public static string FormatForTokenization(string inputString)
        {
            inputString = inputString.Replace(Environment.NewLine, " ");
            ReplaceSpacialCharsWithBlankSpaces(ref inputString);
            SetSpacesArroundSeparators(ref inputString);
            DeleteMultipleBlankSpaces(ref inputString);
            return inputString;
        }

        private static void ReplaceSpacialCharsWithBlankSpaces(ref string inputString)
        {
            char[] charsToReplace = { '\t' };
            foreach (char c in charsToReplace)
            {
                inputString.Replace(c, ' ');
            }
        }

        private static void SetSpacesArroundSeparators(ref string inputString)
        {            
            inputString = System.Text.RegularExpressions.Regex.Replace(inputString, @"\s*{\s*", " { ");
            inputString = System.Text.RegularExpressions.Regex.Replace(inputString, @"\s*}\s*", " } ");
        }

        private static void DeleteMultipleBlankSpaces(ref string inputString)
        {
            inputString = System.Text.RegularExpressions.Regex.Replace(inputString, @"\s+", " ");
        }
        
    }
}
