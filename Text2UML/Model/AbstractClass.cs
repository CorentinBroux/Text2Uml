﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    public class AbstractClass : ABox
    {
        #region Constructors
        public AbstractClass(string name)
        {
            Name = name;
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
        }

        public AbstractClass(string name,List<Attribute> attributes, List<Method> methods)
        {
            Name = name;
            Attributes = attributes;
            Methods = methods;
        }
        #endregion
    }
}
