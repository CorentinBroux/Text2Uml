using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using Text2UML.Model;

namespace Text2UML.View
{
    class Drawer
    {
        public static Border DrawBorder(ABox box, Canvas c, int x, int y)
        {
            // 1. Create the border
            Border b = new Border();
            //b.Width = 150;
            //b.Height = 200;
            b.BorderThickness = new System.Windows.Thickness(2);
            b.BorderBrush = Brushes.Black;
            b.Margin = new System.Windows.Thickness(x, y, 0, 0);

            // 2. Divide in three parts

            // 2.1. Create the StackPanel

            StackPanel mainSP = new StackPanel();


            // 2.2. Create and add the title area

            Label title = new Label()
            {
                Content = box.Name,
                FontSize = 16,
                FontWeight = System.Windows.FontWeights.Bold
            };
            mainSP.Children.Add(title);


            // 2.3. Create and add the attributes area

            StackPanel attributesSP = new StackPanel();

            foreach (Model.Attribute a in box.Attributes)
            {
                Label at1 = new Label()
                {
                    Content = a.Type + " " + a.Name,
                };
                attributesSP.Children.Add(at1);
            }


            Border attributesBorder = new Border();
            attributesBorder.BorderThickness = new System.Windows.Thickness(0, 1, 0, 0);
            attributesBorder.BorderBrush = Brushes.Black;
            attributesBorder.Child = attributesSP;
            mainSP.Children.Add(attributesBorder);


            // 2.3. Create and add the methods area

            StackPanel methodsSP = new StackPanel();

            foreach (Method m in box.Methods)
            {
                string s = m.ReturnType + " " + m.Name + "(";
                int i = 1;
                foreach (string str in m.ParamTypes)
                {
                    s += str;
                    if (i < m.ParamTypes.Count)
                        s += ", ";
                    i++;
                }
                s += ")";

                Label meth1 = new Label()
                {
                    Content = s,
                };
                methodsSP.Children.Add(meth1);
            }
            
            Border methodsBorder = new Border();
            methodsBorder.BorderThickness = new System.Windows.Thickness(0, 1, 0, 0);
            methodsBorder.BorderBrush = Brushes.Black;
            methodsBorder.Child = methodsSP;
            mainSP.Children.Add(methodsBorder);



            // 3. Add the StackPanel to the border

            b.Child = mainSP;


            // 4. Add the border to the canvas

            c.Children.Add(b);


            // 5. Return the border (representing a box)

            return b;
        }
    }
}
