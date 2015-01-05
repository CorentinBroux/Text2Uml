using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralShapes;
using System.IO;
using ITI.Text2UML;
using ITI.Text2UML.Model;
using Dataweb.NShape.SoftwareArchitectureShapes;
using Dataweb.NShape.Layouters;

namespace Text2UML
{
    public partial class Form1 : System.Windows.Forms.UserControl
    {
        Diagram diagram;
        List<Tuple<Shape,string>> drawedShapes = new List<Tuple<Shape,string>>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory();

            // Set path to the sample diagram and the diagram file extension
            xmlStore1.DirectoryName = path;
            xmlStore1.FileExtension = "nspj";
            // Set the name of the project that should be loaded from the store
            project1.Name = "Text2UML";
            project1.LibrarySearchPaths.Add(path);
            // Open the NShape project
            project1.AddLibrary(typeof(RoundedBox).Assembly, false);
            project1.Create();
          

            
            diagram = new Diagram("Test NShape diagram");
            this.display1.Diagram = diagram;
            this.cachedRepository1.Insert(diagram);

            display1.OpenDiagram("Test NShape diagram");
          
        }


        public void DrawBoxes(List<ITI.Text2UML.Model.Class> boxes)
        {
            const int X = 800; // DEBUG value

            int ymax = 0;

            // Clear existing drawing
            diagram.Clear();
            drawedShapes = new List<Tuple<Shape, string>>();

            // Draw boxes
            int x = 120, y = 100;
            foreach (ITI.Text2UML.Model.Class box in boxes)
            {
                if (x > X)
                {
                    x = 120;
                    y += ymax;

                }
                bool drawed1 = false;
                Shape sh1 = null;
                foreach (Tuple<Shape, string> t in drawedShapes)
                    if (t.Item2 == box.Name)
                    {
                        drawed1 = true;
                        sh1 = t.Item1;
                    }

                if (drawed1 == false)
                {
                    Size size1 = new Size();
                    sh1 = DrawSingleBox(box, x, y, ref size1);
                    drawedShapes.Add(Tuple.Create(sh1, box.Name));
                    x += size1.Width + 50;
                    ymax = size1.Height + 50 > ymax ? size1.Height + 50 : ymax;
                }

                
                if (box.IsLinked == true)
                {
                    foreach (Class b in box.Linked)
                    {
                        bool drawed = false;
                        Shape sh2=null;
                        foreach (Tuple<Shape,string> t in drawedShapes)
                            if (t.Item2 == b.Name)
                            {
                                drawed = true;
                                sh2 = t.Item1;
                            }
                        
                        if (drawed == false)
                        {
                            Size size2 = new Size();
                            sh2 = DrawSingleBox(b, x, y, ref size2);
                            drawedShapes.Add(Tuple.Create(sh2, b.Name));
                            x += size2.Width + 100;
                            ymax = size2.Height + 50 > ymax ? size2.Height + 50 : ymax;
                        }
                            
                        Polyline arrow = (Polyline)project1.ShapeTypes["Polyline"].CreateInstance();
                        diagram.Shapes.Add(arrow);
                        arrow.EndCapStyle = project1.Design.CapStyles.ClosedArrow;
                        arrow.Connect(ControlPointId.FirstVertex, sh1, ControlPointId.Reference);
                        arrow.Connect(ControlPointId.LastVertex, sh2, ControlPointId.Reference);
                    }
                    
                }
                
            }

        }

        private Shape DrawSingleEntity(ITI.Text2UML.Model.Class box, int x, int y, ref Size size)
        {
            // Draw the box
            ClassSymbol myShape2 = (ClassSymbol)this.project1.ShapeTypes["ClassSymbol"].CreateInstance();
            //Template template = project1.Repository.GetTemplate("Class");
            //ClassSymbol myShape2 = (ClassSymbol)template.CreateShape();
            myShape2.DisplayService = this.display1;
            //size = MeasureString(s);
            myShape2.Height = size.Height + 20;
            myShape2.Width = size.Width + 20;
            myShape2.MoveTo(x, y);
            myShape2.Text = box.Name;
            diagram.Shapes.Add(myShape2);
            this.project1.Repository.Insert((Shape)myShape2, diagram);
            this.project1.Repository.Update();


            // Set string
            string s = "";
            foreach (ITI.Text2UML.Model.Attribute att in box.Attributes)
            {
                myShape2.AddColumn("\n\t" + att.Type + " " + att.Name);
            }
            s += "\n_______________";
            foreach (ITI.Text2UML.Model.Method met in box.Methods)
            {
                s += "\n\t" + met.ReturnType + " " + met.Name + "(";
                foreach (string str in met.ParamTypes)
                {
                    s += str + ", ";
                }
                if (met.ParamTypes.Count > 0)
                    s = s.Remove(s.Length - 2);
                s += ")";
            }

            // return the shape
            return myShape2;
        }

        private Shape DrawSingleBox(ITI.Text2UML.Model.Class box, int x, int y, ref Size size)
        {

            // Generate string
            string s = box.Name + "\n_______________";

            foreach (ITI.Text2UML.Model.Attribute att in box.Attributes)
            {
                s += "\n\t" + att.Type + " " + att.Name;
            }
            s += "\n_______________";
            foreach (ITI.Text2UML.Model.Method met in box.Methods)
            {
                s += "\n\t" + met.ReturnType + " " + met.Name + "(";
                foreach (string str in met.ParamTypes)
                {
                    s += str + ", ";
                }
                if (met.ParamTypes.Count > 0)
                    s = s.Remove(s.Length - 2);
                s += ")";
            }


            // Draw the box
            RectangleBase myShape2 = (RectangleBase)this.project1.ShapeTypes["RoundedBox"].CreateInstance();
            myShape2.DisplayService = this.display1;
            size = MeasureString(s);
            myShape2.Height = size.Height + 30;
            myShape2.Width = size.Width + 30;
            myShape2.SetCaptionText(0, s);
            myShape2.MoveTo(x, y);
            diagram.Shapes.Add(myShape2);
            this.project1.Repository.Insert((Shape)myShape2, diagram);
            this.project1.Repository.Update();


            // return the shape
            return myShape2;
        }


        private Size MeasureString(string str)
        {
            Font font = new Font("Arial", 12.0F);
            Size textSize = TextRenderer.MeasureText(str, font);
            return textSize;
        }

        public void SaveDiagramAsImage()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Diagram";
            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG Images (.png)|*.png";

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Save document 
                diagram.CreateImage(ImageFileFormat.Png, null, 1, false, Color.White).Save(dlg.FileName);

            }
            
        }

        public void OrganizeShapes()
        {
            // First, place all shapes to the same position
            foreach (Shape s in display1.Diagram.Shapes)
            {
                s.X = 100;
                s.Y = 100;
            }

            // Create the layouter and set up layout parameters
            RepulsionLayouter layouter = new RepulsionLayouter(project1);
            // Set the repulsion force and its range
            layouter.SpringRate = 2;
            layouter.Repulsion = 3;
            layouter.RepulsionRange = 500;
            // Set the friction and the mass of the shapes
            layouter.Friction = 0;
            layouter.Mass = 50;
            // Set all shapes 
            layouter.AllShapes = this.display1.Diagram.Shapes;
            // Set shapes that should be layouted
            layouter.Shapes = this.display1.Diagram.Shapes;
            //
            // Now prepare and execute the layouter
            layouter.Prepare();
            layouter.Execute(10);
            // Fit the result into the diagram bounds
            layouter.Fit(50, 50, display1.Diagram.Width - 100, display1.Diagram.Height - 100);
        }

    }
}
