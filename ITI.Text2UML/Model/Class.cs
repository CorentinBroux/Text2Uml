using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Model
{
    public class Class : ABox
    {
        #region Constructors
        public Class(string name)
        {
            Name = name;
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
            Linked = new List<Class>();
        }

        public Class(string name,List<Attribute> attributes, List<Method> methods)
        {
            Name = name;
            Attributes = attributes;
            Methods = methods;
            Linked = new List<Class>();

        }
        #endregion


    }
}
