﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Model
{
    public class Class
    {

        #region Fields and properties
        public string Name { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<Method> Methods { get; set; }
        public bool IsLinked { get; set; }
        public List<Tuple<Class,LinkTypes, string>> Linked { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        #endregion

        #region Constructors
        public Class(string name)
        {
            Name = name;
            Attributes = new List<Attribute>();
            Methods = new List<Method>();
            Linked = new List<Tuple<Class, LinkTypes, string>>();
        }

        public Class(string name,List<Attribute> attributes, List<Method> methods)
        {
            Name = name;
            Attributes = attributes;
            Methods = methods;
            Linked = new List<Tuple<Class, LinkTypes, string>>();

        }
        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Class {0}\n", Name);
            foreach (Attribute a in Attributes)
                builder.AppendFormat("\t{0}\n", a.ToString());
            foreach (Method m in Methods)
                builder.AppendFormat("\t{0}\n", m.ToString());
            builder.Append("\n");
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            Class c = (Class)obj;
            if (this.Name.Equals(c.Name,StringComparison.InvariantCultureIgnoreCase) && this.Attributes.Count == c.Attributes.Count && this.Methods.Count == c.Methods.Count)
            {
                for (int i = 0; i < this.Attributes.Count; i++)
                {
                    if (this.Attributes[i] != c.Attributes[i])
                        return false;
                }
                for (int i = 0; i < this.Methods.Count; i++)
                {
                    if (this.Methods[i] != c.Methods[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Attributes.GetHashCode() + 17 * Methods.GetHashCode();
        }
        #endregion

    }
}
