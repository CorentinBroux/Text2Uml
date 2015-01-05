using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage.Tools
{
    class Node
    {
        #region Fields and properties
        public string Value { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }
        #endregion

        #region Constructors

        public Node()
        {
            Children = new List<Node>();
        }

        public Node(string value) : this()
        {
            Value = value;
        }

        public Node(string value, Node parent)
            : this(value)
        {
            Parent = parent;
        }

        public Node(string value, Node parent, List<Node> children)
            : this(value, parent)
        {
            Children = children;
        }
        #endregion

        #region Methods

        public void SetParent(Node parent)
        {
            Parent = parent;
            if (!Parent.Children.Contains(parent))
                Parent.Children.Add(this);
        }

        public void AddChild(Node child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void AddChildren(List<Node> children)
        {
            foreach (Node child in children)
                AddChild(child);
        }
        #endregion

    }
}
