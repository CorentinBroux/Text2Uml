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
using ITI.Text2UML.Parsing.NaturalLanguage.UserInput;
using Text2UML.View;
using System.IO;
using Dataweb.NShape.Layouters;
using System.Xml.Linq;
using System.Text.RegularExpressions;


namespace Text2UML
{
    public partial class MainWindow : Window
    {
        Form1 myform;
        List<string> previousSentences = new List<string>();
        List<string> unknownSentences = new List<string>();
        public static List<UserStructureSet> CurrentUserStructureSets;


        public MainWindow()
        {
            InitializeComponent();
            CurrentUserStructureSets = new List<UserStructureSet>();
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

        private void GenerateUML(bool warnings = true)
        {
            try
            {
                if (warnings == true)
                    TB_PseudoCode.Text = PCFormatter.Format(TB_PseudoCode.Text);
                Tuple<List<Class>, List<Link>> tuple = PCParser.Parse(TB_PseudoCode.Text);
                List<Class> boxes = tuple.Item1;
                List<Link> links = tuple.Item2;
                myform._boxes = boxes;
                myform._links = links;
                PCParser.AddLinksToBoxes(links, boxes);
                myform.DrawBoxes(boxes);
            }
            catch (Exception ex)
            {
                if (warnings == true)
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

        //private void submenu_lang_en_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeCheckedLanguage("en");
        //}

        //private void submenu_lang_fr_Click(object sender, RoutedEventArgs e)
        //{
        //    ChangeCheckedLanguage("fr");
        //}

        //private void ChangeCheckedLanguage(string To)
        //{
        //    if (To == "en")
        //    {
        //        submenu_lang_fr.IsChecked = false;
        //        submenu_lang_en.IsChecked = true;
        //    }
        //    else if (To == "fr")
        //    {
        //        submenu_lang_en.IsChecked = false;
        //        submenu_lang_fr.IsChecked = true;
        //    }
        //}


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
            dlg.FileName = "PseudoCode";
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

        public void SaveNativeCode()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "NativeCode";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Save document 
                System.IO.File.WriteAllText(dlg.FileName, TB_NativeLanguage.Text);
                
            }
        }

        public void SaveText2UML()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Text2UML";
            dlg.DefaultExt = ".t2u";
            dlg.Filter = "Text2UML (.t2u)|*.t2u";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                myform.SaveText2UMLProject(dlg.FileName, TB_NativeLanguage.Text);
            }

        }

        public void LoadText2UML(string filename)
        {
            //Load xml
            XDocument xdoc = XDocument.Load(filename);

            List<Class> boxes = new List<Class>();
            List<Link> links = new List<Link>();

            List<Tuple<String, String, LinkTypes>> TmpTles = new List<Tuple<String, String, LinkTypes>>();

            #region Boxes load
            //Run query
            var shs = from sh in xdoc.Descendants("Shapes")
                      select new
                      {
                          Children = sh.Descendants("Shape")
                      };

            //Loop through results
            foreach (var sh in shs)
            {
                foreach (var att in sh.Children)
                {
                    
                    Class tmpBox = new Class(att.Attribute("Name").Value);

                    foreach(var attr in att.Descendants("Attribute"))
                    {
                        ITI.Text2UML.Model.Attribute tmpatt = new ITI.Text2UML.Model.Attribute(attr.Attribute("Type").Value, attr.Attribute("Name").Value);
                        tmpBox.Attributes.Add(tmpatt);
                    }

                    foreach (var mth in att.Descendants("Method"))
                    {
                        Method tmpmth = new Method();

                        tmpmth.Name = mth.Attribute("Name").Value;
                        tmpmth.ReturnType = mth.Attribute("ReturnType").Value;

                        tmpmth.ParamTypes = mth.Elements("ParamType").Select(xe => xe.Value).ToList();

                        tmpBox.Methods.Add(tmpmth);
                    }

                    if (att.Attribute("IsLinked").Value == "true")
                        tmpBox.IsLinked = true;
                    else
                        tmpBox.IsLinked = false;

                    foreach (var lk in att.Descendants("Link"))
                    {


                        LinkTypes lt;
                        if (lk.Attribute("LinkType").ToString() == "Includes")
                        {
                            lt = LinkTypes.Includes;
                        }
                        else
                        {
                            lt = LinkTypes.Extends;
                        }

                        Tuple<String, String, LinkTypes> tmptupl = new Tuple<String, String, LinkTypes>(att.Attribute("Name").Value, lk.Attribute("ClassName").Value, lt);

                        TmpTles.Add(tmptupl);
                    }
                    

                    tmpBox.x = Convert.ToInt32(att.Attribute("x").Value);
                    tmpBox.y = Convert.ToInt32(att.Attribute("y").Value);
                       

                    boxes.Add(tmpBox);
                }
            }

            foreach(var tpl in TmpTles)
            {
                Class tmpClass = boxes.Find(x => x.Name == tpl.Item2);
                Class tmpClass2 = boxes.Find(x => x.Name == tpl.Item1);
                tmpClass2.Linked.Add(new Tuple<Class, LinkTypes>(tmpClass, tpl.Item3));
            }

            #endregion

            #region Links load
            //Run query
            var lks = from lk in xdoc.Descendants("Links")
                       select new
                       {
                           Children = lk.Descendants("Link")
                       };

            //Loop through results
            foreach (var lk in lks)
            {
                foreach (var att in lk.Children)
                {
                    LinkTypes lt;
                    if(att.Attribute("Type").ToString() == "Includes")
                    {
                        lt = LinkTypes.Includes;
                    }
                    else
                    {
                         lt = LinkTypes.Extends;
                    }
                    Link tmpLink = new Link(att.Attribute("From").Value, att.Attribute("To").Value, lt);

                    links.Add(tmpLink);
                }
            }
            #endregion


            //Run query
            var nls = from nl in xdoc.Descendants("Root")
                      select new
                      {
                          Children = nl.Attribute("NativeLanguage")
                      };

            foreach(var nl in nls)
            {
            TB_NativeLanguage.Text = nl.Children.Value;
            }

            myform.LoadText2UMLDiagram(boxes, links);
        }

        private void SaveCanvasAsImage()
        {
            myform.SaveDiagramAsImage();
        }

        private void BT_Process_NL_Click(object sender, RoutedEventArgs e)
        {
            //TB_PseudoCode.Text = "";
            string msg = NL_Process();
            if (msg.Length > 0)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(msg + "\n\n\nWould you like to define these structures now ?", "Parsing error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    Dialog_Structure ds = new Dialog_Structure(unknownSentences);
                    ds.Owner = this;
                    ds.ShowInTaskbar = false;
                    ds.ShowDialog();
                }
            }

        }

        private string NL_Process()
        {
            // Reinitialize specialized types
            NLGrammar.Types = new List<Tuple<string, string>>();
            char[] sentenceSeparators = { '.', '!', '?' };
            List<string> input = TB_NativeLanguage.Text.Split(sentenceSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();//.Except(previousSentences).ToList();
            //previousSentences = TB_NativeLanguage.Text.Split(sentenceSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
            //if (input.Count == 0)
            //    input = previousSentences;
            string output = "";
            string error = "";
            NLParser.j = 1;
            NLParser.Matches = new List<Tuple<List<string>, string>>();
            //myform.ResetDiagram();
            unknownSentences = new List<string>();
            foreach (string s in input)
            {
                string str = NLParser.Parse(StanfordParser.Stanford_Parse(s), CurrentUserStructureSets);
                if (str != "Unknown")
                    output += str + " ";
                else if (!String.IsNullOrWhiteSpace(s))
                {
                    error += String.Format("Unknown structure :\n{0}\n\n", s);
                    unknownSentences.Add(s);
                }

            }
            if (unknownSentences.Count > 0)
                LB_Status.Content = String.Format("{0} unknown sentences. Click 'process' for more details.", unknownSentences.Count.ToString());
            else LB_Status.Content = "";

            // Matches
            foreach (Tuple<List<string>, string> t in NLParser.Matches)
                foreach (string s in t.Item1)
                    output = Regex.Replace(output, @"\b" + s + @"\b", t.Item2);

            TB_PseudoCode.Text = output;

            if (output.Length > 0)
                GenerateUML();
            if (error.Length > 0)
                return String.Format("Some sentences may not have been parsed !\n\n{0}", error);
            return "";
        }

        private void BT_Open_NL_Click(object sender, RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile(false);
            NL_Process();
        }

        private void TB_NativeLanguage_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Enter)
            {
                if (Is_Auto_Process.IsChecked == true)
                    NL_Process();
            }
        }

        private void TB_PseudoCode_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            GenerateUML(false);
        }

        private void BT_Organize_Shapes_Click(object sender, RoutedEventArgs e)
        {
            myform.OrganizeShapes();
        }

        private void BT_LoadStructures_Click(object sender, RoutedEventArgs e)
        {
            CurrentUserStructureSets.Add(UserStructureSet.LoadFromFile());
        }

        private void submenu_structures_Click(object sender, RoutedEventArgs e)
        {
            ManageStructures ms = new ManageStructures();
            ms.Owner = this;
            ms.ShowInTaskbar = false;
            ms.ShowDialog();
        }


    }
}