﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Model
{
    public class Attribute
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

        #region Methods
        public override string ToString()
        {
            return String.Format("{0} {1}", Type, Name);
        }

        public override bool Equals(Object obj)
        {
            ITI.Text2UML.Model.Attribute a = (ITI.Text2UML.Model.Attribute) obj;
            if (this.Name == a.Name && this.Type == a.Type)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + 17 * Type.GetHashCode();
        }
        #endregion
    }
}
