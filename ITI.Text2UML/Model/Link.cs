using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Model
{
    public enum LinkTypes
    {
        Includes,
        Extends,
    };

    public class Link
    {


        #region Fields and properties
        public string From { get; set; }
        public string To { get; set; }
        public LinkTypes Type { get; set; }
        public string Label { get; set; }
        #endregion

        #region Constructors
        public Link(string from, string to, LinkTypes type)
        {
            From = from;
            To = to;
            Type = type;
            Label = "";
        }
        #endregion

        #region Methods

        public override string ToString()
        {
            switch (Type)
            {
                case LinkTypes.Extends:
                    return String.Format("{0} -> {1} {2}", From, To, Label);
                case LinkTypes.Includes:
                    return String.Format("{0} --> {1} {2}", From, To, Label);
                default:
                    return String.Format("{0} -> {1} {2}", From, To, Label);
            }

        }

        public override bool Equals(object obj)
        {
            Link l = (Link)obj;
            return From == l.From && To == l.To;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() + 17 * To.GetHashCode();
        }
        #endregion

        #region Static methods

        public static LinkTypes GetLinkTypeFromSymbol(string symbol)
        {
            switch (symbol)
            {
                case "->":
                    return LinkTypes.Extends;
                case "-->":
                    return LinkTypes.Includes;
                default:
                    return LinkTypes.Extends;
                //throw new InvalidSyntaxException("Error : unknown link sybol");
            }

        }

        #endregion
    }
}
