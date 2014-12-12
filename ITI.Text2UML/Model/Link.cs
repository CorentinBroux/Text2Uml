using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML
{
    public enum LinkTypes
    {
        Includes,
        Extends
    };

    public class Link
    {
        

        #region Fields and properties
        public string From { get; set; }
        public string To { get; set; }
        public LinkTypes Type { get; set; }
        #endregion

        #region Constructors
        public Link(string from, string to, LinkTypes type)
        {
            From = from;
            To = to;
            Type = type;
        }
        #endregion

        #region Static methods

        public static LinkTypes GetLinkTypeFromSymbol(string symbol)
        {
            switch (symbol)
            {
                case "-<>":
                    return LinkTypes.Extends;
                case "-<->":
                    return LinkTypes.Extends;
                case "->>":
                    return LinkTypes.Includes;
                case "-.>":
                    return LinkTypes.Extends;
                case "-":
                    return LinkTypes.Extends;
                case "-()":
                    return LinkTypes.Extends;
                case "-.>>":
                    return LinkTypes.Extends;
                case "-(":
                    return LinkTypes.Extends;
                case "-<.":
                    return LinkTypes.Extends;
                case "-->":
                    return LinkTypes.Extends;
                case "-><-":
                    return LinkTypes.Extends;
                default:
                    return LinkTypes.Extends;
                    //throw new InvalidSyntaxException("Error : unknown link sybol");
            }
            
        }

        #endregion
    }
}
