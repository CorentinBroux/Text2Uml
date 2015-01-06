using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Text2UML.View
{
    /// <summary>
    /// Interaction logic for Dialog_Structure.xaml
    /// </summary>
    public partial class Dialog_Structure : Window
    {
        public Dialog_Structure()
        {
            InitializeComponent();
        }

        public Dialog_Structure(List<string> sentences)
            : this()
        {
            foreach (string s in sentences)
                this.list.Items.Add(s);
            UpdateFields();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFields();
        }

        private void UpdateFields()
        {
            string str = list.SelectedItem != null ? list.SelectedItem.ToString() : "";
            if (str.Length == 0)
                return;
            TB_Tree.Text = ITI.Text2UML.Parsing.NaturalLanguage.StanfordParser.Stanford_Parse(str);
            List<Tuple<string, string>> tuples = ITI.Text2UML.Parsing.NaturalLanguage.NLParser.GetLowLevelTokens(TB_Tree.Text);
            StringBuilder builder = new StringBuilder();
            foreach (Tuple<string, string> t in tuples)
                builder.AppendFormat("{0} {1} ", t.Item1, t.Item2);
            TB_Regex.Text = builder.ToString().Remove(builder.Length - 1);
        }


    }
}
