using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    public abstract class ABox
    {
        #region Fields and properties
        public string Name { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<Method> Methods { get; set; }
        public bool IsLinked { get; set; }
        public List<ABox> Linked { get; set; }
        #endregion


        
    }
}
