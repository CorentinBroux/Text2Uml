using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage.Tools
{
    [Serializable]
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

        public Tree(string data)
        {
            this.Root = NLParser.GetTree(data).Root;
        }
        #endregion

        #region Methods

        public static Tree DeepCopy(Tree tree)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, tree);
                ms.Position = 0;

                return (Tree)formatter.Deserialize(ms);
            }
        }

        public Tree GetSubTree(int level)
        {
            Tree tree = DeepCopy(this);
            List<Node> nodes = tree.Root.GetAllChildren();
            foreach(Node node in nodes)
                if (node.Level >= level)
                    node.Children.Clear();
            return tree;
        }

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
