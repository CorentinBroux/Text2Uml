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

                string source = TB_PseudoCode.Text;
                source = source.Replace(Environment.NewLine, " ");
                //string[] strings = source.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
                //StringBuilder sb = new StringBuilder();
                //for (int i = 0; i < strings.Length; i++)
                //{
                //    sb.Append(strings[i]);
                //    if (i < strings.Length - 1)
                //        sb.Append(" ");
                //}
                
                
                //MessageBox.Show(source); // DEBUG



                string s = "";
                foreach (ABox box in Parser.ExtractAboxes(source))
                {
                    s += box.GetType().ToString() + " " + box.Name + "\n" + "Attributes: \n";
                    foreach (Model.Attribute a in box.Attributes)
                    {
                        s += "\t" + a.Name + " : " + a.Type + "\n";
                    }
                    s += "\nMethods: \n";
                    foreach (Method m in box.Methods)
                    {
                        s += "\t" + m.ReturnType + " " + m.Name + "(";
                        int i = 1;
                        foreach (string str in m.ParamTypes)
                        {
                            s += str;
                            if (i < m.ParamTypes.Count)
                                s += ", ";
                            i++;
                        }
                        s += ")\n";
                    }
                    s += "\n\n";
                }
                MessageBox.Show(s);
            }
            catch (InvalidSyntaxException ise)
            {
                MessageBox.Show(ise.Message);
            }
        }
    }
}