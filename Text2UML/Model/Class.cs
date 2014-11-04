using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    class Class : ABox
    {
        #region Constructors
        public Class()
        {
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
        }

        public Class(List<Attribute> attributes, List<Method> methods)
        {
            Attributes = attributes;
            Methods = methods;
        }
        #endregion
    }
}
