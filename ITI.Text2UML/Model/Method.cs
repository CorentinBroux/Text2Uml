using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Model
{
    public class Method
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

        #region Methods
        public override string ToString()
        {
            string par = "";
            foreach (string s in ParamTypes)
            {
                par += s + " ";
            }
            if(par.EndsWith(" "))
                par = par.Remove(par.Length - 1, 1);
            return String.Format("{0} {1}({2})", ReturnType, Name, par);
        }

        public bool Equals(Method m)
        {
            if (this.Name == m.Name && this.ReturnType == m.ReturnType && this.ParamTypes.Count == m.ParamTypes.Count)
            {
                for (int i = 0; i < this.ParamTypes.Count; i++)
                {
                    if (this.ParamTypes[i] != m.ParamTypes[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}
