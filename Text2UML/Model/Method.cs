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
        public List<string> ParamTypes { get; set; }
        public string ReturnType { get; set; }

        #endregion

        #region Constructors
        public Method(List<string> paramTypes, string returnType)
        {
            ParamTypes = paramTypes;
            ReturnType = returnType;
        }
        #endregion
    }
}
