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
using ITI.Text2UML.Parsing.NaturalLanguage.UserInput;
using ITI.Text2UML.Model;
using ITI.Text2UML.Parsing.PseudoCode;

namespace Text2UML.View
{
    /// <summary>
    /// Interaction logic for Dialog_Structure.xaml
    /// </summary>
    public partial class Dialog_Structure : Window
    {
        UserStructureSet uss;
        Form1 hostedForm;
        public Dialog_Structure()
        {
            InitializeComponent();
            hostedForm = new Form1();
            propertyGridHost.Child = hostedForm;
            uss = new UserStructureSet();
        }

        public Dialog_Structure(List<string> sentences)
            : this()
        {
            foreach (string s in sentences)
                this.list.Items.Add(s);
            UpdateFields();
        }

        private void GenerateUML()
        {
            try
            {
                Tuple<List<Class>, List<Link>> tuple = PCParser.Parse(TB_PseudoCode.Text);
                List<Class> boxes = tuple.Item1;
                List<Link> links = tuple.Item2;
                PCParser.AddLinksToBoxes(links, boxes);
                hostedForm.DrawBoxes(boxes);
            }
            catch
            {
                
            }
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
            if(builder.Length > 0)
                TB_Regex.Text = builder.ToString().Remove(builder.Length - 1);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserStructureType type = this.radio1.IsChecked == true ? UserStructureType.ByTree : UserStructureType.ByRegex;
            string input = this.radio1.IsChecked == true ? this.TB_Tree.Text : this.TB_Regex.Text;
            uss.AddStructure(new UserStructure(type,input,this.TB_PseudoCode.Text));
        }

        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            if (uss.Structures.Count > 0)
                uss.SaveToFile();
            this.Close();
        }

        private void TB_PseudoCode_KeyUp(object sender, KeyEventArgs e)
        {
            GenerateUML();
        }


    }
}
