using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage.UserInput
{
    [Serializable]
    public class UserStructure
    {
        #region Fields and properties
        public UserStructureType Type { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        #endregion

        #region Constructors
        public UserStructure()
        {

        }

        public UserStructure(UserStructureType type)
            : this()
        {
            Type = type;
        }

        public UserStructure(UserStructureType type, string input, string output)
            : this(type)
        {
            Input = input;
            Output = output;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            UserStructure us = (UserStructure)obj;
            return Input == us.Input && Output == us.Output;
        }

        public override int GetHashCode()
        {
            return Input.GetHashCode() * Output.GetHashCode(); // ***************************** ???? 
        }

        public override string ToString()
        {
            return String.Format("Structure\n\tType : {0}\n\tInput : {1}\n\tOutput : {2}", Type.ToString(), Input, Output);
        }
        #endregion
    }

    [Serializable]
    public enum UserStructureType
    {
        ByRegex,
        ByTree
    }
}
