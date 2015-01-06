using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage.Tools
{
    public class Node
    {
        #region Fields and properties
        public string Value { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }
        public int Level { get; set; }
        #endregion

        #region Constructors

        public Node()
        {
            Children = new List<Node>();
            Level = 0;
        }

        public Node(string value)
            : this()
        {
            Value = value;
        }

        public Node(string value, Node parent)
            : this(value)
        {
            SetParent(parent);
        }

        public Node(string value, Node parent, List<Node> children)
            : this(value, parent)
        {
            AddChildren(children);
        }
        #endregion

        #region Methods

        public void SetParent(Node parent)
        {
            Parent = parent;
            if (!Parent.Children.Contains(this))
                Parent.Children.Add(this);
            Level = Parent.Level + 1;
        }

        public void AddChild(Node child)
        {
            Children.Add(child);
            child.SetParent(this);
        }

        public void AddChildren(List<Node> children)
        {
            foreach (Node child in children)
                AddChild(child);
        }

        public List<Node> GetAllChildren()
        {
            List<Node> nodes = new List<Node>();
            nodes.AddRange(Children);
            foreach (Node node in Children)
                nodes.AddRange(node.GetAllChildren());
            return nodes;
        }

        public override bool Equals(object obj)
        {
            Node node = (Node)obj;
            if (this.Value != node.Value || this.Children.Count != node.Children.Count) return false;
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (!this.Children[i].Equals(node.Children[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

    }
}
