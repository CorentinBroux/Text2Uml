using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    class Method
    {
        # region Fields and properties
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<string> ParamTypes { get; set; }
        

        #endregion

        #region Constructors
        public Method(string returnType, string name, List<string> paramTypes)
        {
            ReturnType = returnType;
            ParamTypes = paramTypes;
            Name = name;
        }

        public Method()
        {

        }
        #endregion
    }
}
