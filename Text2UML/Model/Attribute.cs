using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    class Attribute
    {
        #region Fields and properties
        public string Name { get; set; }
        public string Type { get; set; }
        #endregion

        #region Constructors
        public Attribute(string type, string name)
        {
            Name = name;
            Type = type;
        }
        #endregion
    }
}
