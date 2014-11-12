#define DEBUG

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Text2UML.Model;
using System.IO;

namespace Text2UML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            


            // DEBUG - PARSER TEST

            //try
            //{
            //    string s = "";
            //    foreach (ABox box in Parser.ExtractAboxes("$Class Toto { $Attributes int age string name } $Abstract Titi { $Methods bool IsAlive(int, int) void Eat(food) } $Interface Tata { other stuff } $Links { a -> b \n y -> z }"))
            //    {
            //        s += box.GetType().ToString() + " " + box.Name + "\n" + "Attributes: \n";
            //        foreach (Model.Attribute a in box.Attributes)
            //        {
            //            s += "\t" + a.Name + " : " + a.Type + "\n";
            //        }
            //        s += "\nMethods: \n";
            //        foreach (Method m in box.Methods)
            //        {
            //            s += "\t" + m.ReturnType + " " + m.Name + "(";
            //            int i = 1;
            //            foreach (string str in m.ParamTypes)
            //            {
            //                s += str;
            //                if (i < m.ParamTypes.Count)
            //                    s += ", ";
            //                i++;
            //            }
            //            s += ")\n";
            //        }
            //        s += "\n\n";
            //    }
            //    MessageBox.Show(s);
            //}
            //catch (InvalidSyntaxException ise)
            //{
            //    MessageBox.Show(ise.Message);
            //}

        }

        private void BT_Process_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                //string source = TB_PseudoCode.Text;
                //source = source.Replace(Environment.NewLine, " ");
                //string[] strings = source.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
                //StringBuilder sb = new StringBuilder();
                //for (int i = 0; i < strings.Length; i++)
                //{
                //    sb.Append(strings[i]);
                //    if (i < strings.Length - 1)
                //        sb.Append(" ");
                //}
                
                
                //MessageBox.Show(source); // DEBUG



                //string s = "";
                //foreach (ABox box in Parser.ExtractAboxes(Formatter.FormatForTokenization(TB_PseudoCode.Text)))
                //{
                //    s += box.GetType().Name + " " + box.Name + "\n" + "Attributes: \n";
                //    foreach (Model.Attribute a in box.Attributes)
                //    {
                //        s += "\t" + a.Name + " : " + a.Type + "\n";
                //    }
                //    s += "\nMethods: \n";
                //    foreach (Method m in box.Methods)
                //    {
                //        s += "\t" + m.ReturnType + " " + m.Name + "(";
                //        int i = 1;
                //        foreach (string str in m.ParamTypes)
                //        {
                //            s += str;
                //            if (i < m.ParamTypes.Count)
                //                s += ", ";
                //            i++;
                //        }
                //        s += ")\n";
                //    }
                //    s += "\n\n";
                //}
                //MessageBox.Show(s);



                //View.Drawer.DrawBorder(Parser.ExtractAboxes(Formatter.FormatForTokenization(TB_PseudoCode.Text))[0], canvas1, 10, 10);
                View.Drawer.DrawBoxes(Parser.ExtractAboxes(Formatter.FormatForTokenization(TB_PseudoCode.Text)), canvas1);
                canvas1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                canvas1.Height = 1000;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void BT_Open_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile();
        }

        private void LoadPseudoCodeFromFile()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
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
            View.Dialog_Open diao = new View.Dialog_Open();
            diao.Owner = this;
            diao.ShowDialog();
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            View.Dialog_Save dias = new View.Dialog_Save();
            dias.Owner = this;
            dias.ShowDialog();
        }
        
    }

    
}