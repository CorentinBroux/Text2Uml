using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage.Tools
{
    public class Tree
    {
        #region Fields and properties
        public Node Root { get; set; }
        #endregion

        #region Constructors
        public Tree()
        {

        }

        public Tree(Node root)
        {
            Root = root;
        }
        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            Tree tree = (Tree)obj;
            return this.Root.Equals(tree.Root);
        }

        public override int GetHashCode()
        {
            return Root.GetHashCode();
        }
        #endregion
    }
}
