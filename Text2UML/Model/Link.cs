using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    public enum LinkTypes
    {
        Include,
        Extends
    };

    class Link
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
            // TODO : implements the method
            return LinkTypes.Extends;
        }

        #endregion
    }
}
