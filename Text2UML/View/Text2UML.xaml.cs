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


            //#if DEBUG
            //Tokenizer tokenizer = new Tokenizer("$Class titi { $Methods run() }");
            //string output = "";
            //Token token;

            //do
            //{
            //    token = tokenizer.GoToNextToken();
            //    output += "value : " + token.Value + "\ntype : " + token.Type + "\nlength : " + token.Value.Length + "\n\n";
            //} while (token.Type != TokenType.EoF);

            //MessageBox.Show(output);
            //#endif


            try
            {
                string s = "";
                foreach (ABox box in Parser.ExtractAboxes("$Class Toto { $Attributes int age string name } $Abstract Titi { $Methods bool IsAlive(int, int) void Eat(food) } $Interface Tata { other stuff } $Links { a -> b \n y -> z }"))
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