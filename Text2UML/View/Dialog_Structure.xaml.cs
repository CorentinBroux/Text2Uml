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
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string str = list.SelectedItem != null ? list.SelectedItem.ToString() : "";
            TB_Tree.Text = ITI.Text2UML.Parsing.NaturalLanguage.StanfordParser.Stanford_Parse(str);
        }


    }
}
