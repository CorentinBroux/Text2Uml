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

        public static double GlobalMaxWidth = 1000;

 
        // return canvas height
        public static double DrawBoxes(List<ABox> boxes, Canvas c)
        {
            try
            {
                int x = 10;
                int y = 10;
                double maxHeight = 0;
                double maxWidth = 0;

                // Remove existings objects from canvas
                c.Children.Clear();

                // Draw the first box
                Border b = DrawBorder(boxes[0], c, x, y);

                // Get the dimensions of the first box
                b.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                double w = b.DesiredSize.Width;
                double h = b.DesiredSize.Height;
                boxes.RemoveAt(0);

                maxHeight = maxHeight < h ? h : maxHeight;
                maxWidth = maxWidth < w ? w : maxWidth;

                // Draw  boxes
                foreach (ABox box in boxes)
                {
                    b = DrawBorder(box, c, (int)w + 50, y);
                    b.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                    w = b.DesiredSize.Width;
                    h = b.DesiredSize.Height;
                    
                    if (w > GlobalMaxWidth) // new line
                    {
                        c.Children.RemoveAt(c.Children.Count - 1);
                        y = (int)maxHeight + 50;
                        maxHeight = 0;
                        b = DrawBorder(box, c, x, y);
                        b.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                        w = b.DesiredSize.Width;
                        h = b.DesiredSize.Height;
                    }

                    maxHeight = maxHeight < h ? h : maxHeight;
                    maxWidth = maxWidth < w ? w : maxWidth;
                }

                c.Height = (int)maxHeight ;
                c.Width = (int)maxWidth;
                return maxHeight;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return 0;
            }
            
        }
        public static Border DrawBorder(ABox box, Canvas c, int x, int y)
        {
            // 1. Create the border
            Border b = new Border();
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
