using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    abstract class ABox
    {
        #region Fields and properties
        public List<Attribute> Attributes { get; set; }
        public List<Method> Methods { get; set; }
        #endregion
    }
}
