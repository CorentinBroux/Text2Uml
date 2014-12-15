﻿using System;
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
            //Dataweb.NShape.GeneralShapes.NShapeLibraryInitializer.Initialize(null);

            myform = new Form1();
            
            // Initialize WinForms PropertyGrid
            propertyGridHost.Child = myform;
            //TB_PseudoCode.Text = NLParser.Parse(StanfordParser.Parse("A cat is an animal"));
            TB_PseudoCode.Text = NLParser.Parse(TEST("A cat is an animal"));

        }

        static string TEST(string input)
        {
            // Path to models extracted from `stanford-parser-3.5.0-models.jar`
            var jarRoot = @"c:\models\stanford-parser-full-2014-10-31";
            var modelsDirectory = jarRoot + @"\edu\stanford\nlp\models";

            // Loading english PCFG parser from file
            var lp = LexicalizedParser.loadModel(modelsDirectory + @"\lexparser\englishPCFG.ser.gz");

            // This sample shows parsing a list of correctly tokenized words
            var sent = new[] { "This", "is", "an", "easy", "sentence", "." };
            var rawWords = Sentence.toCoreLabelList(sent);
            var tree = lp.apply(rawWords);
            tree.pennPrint();

            // This option shows loading and using an explicit tokenizer
            var sent2 = input;
            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var sent2Reader = new java.io.StringReader(sent2);
            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
            sent2Reader.close();
            var tree2 = lp.apply(rawWords2);

            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(tree2);
            var tdl = gs.typedDependenciesCCprocessed();
            Console.WriteLine("\n{0}\n", tdl);

            // Extract collapsed dependencies from parsed tree
            var tp = new TreePrint("penn,typedDependenciesCollapsed");
            tp.printTree(tree2);
            Console.Read();


            return tree2.toString();
        }

        private void BT_Process_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                Tuple<List<Class>, List<Link>> tuple = Parser.Parse(Formatter.FormatForTokenization(TB_PseudoCode.Text));
                List<Class> boxes = tuple.Item1;
                List<Link> links = tuple.Item2;
                //Parser.ReportDeadLinks(links, boxes);
                Parser.AddLinksToBoxes(links, boxes);
                myform.DrawBoxes(boxes);
                double h = View.Drawer.DrawBoxes(boxes, canvas1);
                canvas1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                canvas1.Height = h;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public MainWindow getThis()
        {
            return this;
        }

        private void BT_Open_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile();
        }

        public void LoadPseudoCodeFromFile()
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
                    TB_PseudoCode.Text = sr.ReadToEnd();
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
    }

    
}