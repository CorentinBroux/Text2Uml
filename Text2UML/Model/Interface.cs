using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    class Interface : ABox
    {
        #region Constructors
        public Interface()
        {
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
        }

        public Interface(List<Attribute> attributes, List<Method> methods)
        {
            Attributes = attributes;
            Methods = methods;
        }
        #endregion
    }
}
