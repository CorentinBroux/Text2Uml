using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    public enum LinkTypes
    {

    };

    class Link
    {
        

        #region Fields and properties
        public ABox From { get; set; }
        public ABox To { get; set; }
        #endregion

        #region Constructors
        public Link(ABox from, ABox to)
        {
            From = from;
            To = to;
        }
        #endregion
    }
}
