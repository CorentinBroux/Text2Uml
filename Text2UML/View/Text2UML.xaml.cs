using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ITI.Text2UML;
using ITI.Text2UML.Model;
using ITI.Text2UML.Parsing.NaturalLanguage;
using ITI.Text2UML.Parsing.PseudoCode;
using Text2UML.View;
using System.IO;
using Dataweb.NShape.Layouters;


namespace Text2UML
{
    public partial class MainWindow : Window
    {
        Form1 myform;
        List<string> previousSentences = new List<string>();
        public MainWindow()
        {
            InitializeComponent();

            myform = new Form1();
            propertyGridHost.Child = myform;

            // Parse a sentence to first load StanfordParser and avoid wait times
            NLParser.Parse(StanfordParser.Stanford_Parse("This is a test."));

        }



        private void BT_Process_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ITI.Text2UML.Parsing.NaturalLanguage.NLGrammar.Types = new List<Tuple<string, string>>();
            GenerateUML();
        }

        private void GenerateUML()
        {
            try
            {
                TB_PseudoCode.Text = PCFormatter.Format(TB_PseudoCode.Text);
                Tuple<List<Class>, List<Link>> tuple = PCParser.Parse(TB_PseudoCode.Text);
                List<Class> boxes = tuple.Item1;
                List<Link> links = tuple.Item2;
                PCParser.AddLinksToBoxes(links, boxes);
                myform.DrawBoxes(boxes);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public MainWindow getThis()
        {
            return this;
        }

        private void BT_Open_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile(true);
        }

        public void LoadPseudoCodeFromFile(bool IsPseudoCode)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                using (StreamReader sr = new StreamReader(filename))
                {
                    if (IsPseudoCode == true)
                        TB_PseudoCode.Text = sr.ReadToEnd();
                    else
                        TB_NativeLanguage.Text = sr.ReadToEnd();
                }
            }
        }

        private void submenu_lang_en_Click(object sender, RoutedEventArgs e)
        {
            ChangeCheckedLanguage("en");
        }

        private void submenu_lang_fr_Click(object sender, RoutedEventArgs e)
        {
            ChangeCheckedLanguage("fr");
        }

        private void ChangeCheckedLanguage(string To)
        {
            if (To == "en")
            {
                submenu_lang_fr.IsChecked = false;
                submenu_lang_en.IsChecked = true;
            }
            else if (To == "fr")
            {
                submenu_lang_en.IsChecked = false;
                submenu_lang_fr.IsChecked = true;
            }
        }


        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            View.Dialog_Open diao = new View.Dialog_Open(getThis());
            diao.Owner = this;
            diao.ShowInTaskbar = false;
            diao.ShowDialog();
        }


        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            View.Dialog_Save dias = new View.Dialog_Save();
            dias.Owner = this;
            dias.ShowInTaskbar = false;
            dias.ShowDialog();
        }


        private void ExportCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SaveCanvasAsImage();
        }

        public void SavePseudoCode()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Pseudocode";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Save document 
                System.IO.File.WriteAllText(dlg.FileName, TB_PseudoCode.Text);

            }
        }

        private void SaveCanvasAsImage()
        {
            myform.SaveDiagramAsImage();
        }

        private void BT_Process_NL_Click(object sender, RoutedEventArgs e)
        {
            TB_PseudoCode.Text = "";
            string msg = NL_Process();
            if (msg.Length > 0)
                System.Windows.MessageBox.Show(msg, "Parsing error");
        }

        private string NL_Process()
        {
            // Reinitialize specialized types
            NLGrammar.Types = new List<Tuple<string, string>>();
            char[] sentenceSeparators = { '.', '!', '?' };
            List<string> input = TB_NativeLanguage.Text.Split(sentenceSeparators, StringSplitOptions.RemoveEmptyEntries).Except(previousSentences).ToList();
            previousSentences = TB_NativeLanguage.Text.Split(sentenceSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            string output = "";
            string error = "";
            NLParser.j = 1;
            foreach (string s in input)
            {
                string str = NLParser.Parse(StanfordParser.Stanford_Parse(s));
                if (str != "Unknown")
                    output += str + " ";
                else
                    error += String.Format("Unknown structure :\n{0}\n\n", s);
            }
            if (error.Length > 0)
                return String.Format("Some sentences may not have been parsed !\n\n{0}", error);
            TB_PseudoCode.Text += output;
            if (output.Length > 0)
                GenerateUML();
            return "";
        }

        private void BT_Open_NL_Click(object sender, RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile(false);
            NL_Process();
        }

        private void TB_NativeLanguage_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Space || e.Key == Key.OemPeriod || e.Key ==  Key.OemComma)
                NL_Process();
        }

        private void TB_PseudoCode_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == Key.Space || e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Enter)
            //    GenerateUML();
        }

        private void BT_Organize_Shapes_Click(object sender, RoutedEventArgs e)
        {
            myform.OrganizeShapes();
        }


    }
}