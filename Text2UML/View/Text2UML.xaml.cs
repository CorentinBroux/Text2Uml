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
#if DEBUG
using java.io;
using edu.stanford.nlp.process;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.trees;
using edu.stanford.nlp.parser.lexparser;
using Console = System.Console;
#endif

namespace Text2UML
{
    public partial class MainWindow : Window
    {
        Form1 myform;
        public MainWindow()
        {
            InitializeComponent();

            myform = new Form1();
            propertyGridHost.Child = myform;
            
            // Parse a sentence to first load StanfordParser and avoid wait times
            NLParser.Parse(TEST("This is a test."));

        }

        /// <summary>
        /// Parse the sentence into a syntaxic tree.
        /// </summary>
        /// <param name="input">Sentence.</param>
        /// <returns>String representing the syntaxic tree.</returns>
        static string TEST(string input)
        {
            // Path to models extracted from `stanford-parser-3.5.0-models.jar`
            var jarRoot = @"c:\models\stanford-parser-full-2014-10-31";
            var modelsDirectory = jarRoot + @"\edu\stanford\nlp\models";

            // Loading english PCFG parser from file
            var lp = LexicalizedParser.loadModel(modelsDirectory + @"\lexparser\englishPCFG.ser.gz");


            // This option shows loading and using an explicit tokenizer
            var sent2 = input;
            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var sent2Reader = new java.io.StringReader(sent2);
            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
            sent2Reader.close();
            var tree = lp.apply(rawWords2);

            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(tree);
            var tdl = gs.typedDependenciesCCprocessed();

            // Return tree expression
            return tree.toString();
        }

        private void BT_Process_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
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
            if(To=="en")
            {
                submenu_lang_fr.IsChecked = false;
                submenu_lang_en.IsChecked = true;
            }else if (To == "fr")
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
            // Reinitialize specialized types
            NLGrammar.Types = new List<Tuple<string,string>>();
            char[] sentenceSeparators = { '.','!','?' };
            List<string> input = TB_NativeLanguage.Text.Split(sentenceSeparators,StringSplitOptions.RemoveEmptyEntries).ToList();
            string output = "";
            string error = "";
            NLParser.j = 1;
            foreach (string s in input)
            {
                string str = NLParser.Parse(TEST(s));
                if (str != "Unknown")
                    output += str + " ";
                else
                    error += String.Format("Unknown structure :\n{0}\n\n", s);
            }
            if (error.Length > 0)
                System.Windows.MessageBox.Show(String.Format("Some sentences may not have been parsed !\n\n{0}", error), "Parsing error");
            TB_PseudoCode.Text = output;
            if(output.Length > 0)
                GenerateUML();
        }

        private void BT_Open_NL_Click(object sender, RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile(false);
        }
    }

    
}