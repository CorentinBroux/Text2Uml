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
using System.IO;
using Text2UML.Model;

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
            
            // Set path to the sample diagram and the diagram file extension
            xmlStore1.DirectoryName = @"C:\Users\Public\Documents\NShape\Text2Uml";
            xmlStore1.FileExtension = "nspj";
            // Set the name of the project that should be loaded from the store
            project1.Name = "Circles";
            project1.LibrarySearchPaths.Add(@"C:\Users\Public\Documents\NShape\bin\Debug");
            project1.AutoLoadLibraries = true;
            // Open the NShape project
            this.project1.Open();
          

            
            diagram = new Diagram("Test NShape diagram");
            this.display1.Diagram = diagram;
            this.cachedRepository1.Insert(diagram);


            // Now add shape to diagram:
            //Shape myShape = this.project1.ShapeTypes["Ellipse"].CreateInstance();
            //myShape.DisplayService = this.display1;
            //myShape.MoveTo(diagram.Width / 2, diagram.Height / 2);  // Move the shape to the desired position
            //diagram.Shapes.Add(myShape);
            //this.project1.Repository.Insert(myShape, diagram);
            //this.project1.Repository.Update();

            // Now add shape to diagram:
            //RectangleBase myShape2 = (RectangleBase)this.project1.ShapeTypes["RoundedBox"].CreateInstance();
            //myShape2.DisplayService = this.display1;
            //myShape2.SetCaptionText(0, "test\nnewline");
            //myShape2.MoveTo(diagram.Width / 3, diagram.Height / 3);  // Move the shape to the desired position
            //diagram.Shapes.Add(myShape2);
            //this.project1.Repository.Insert((Shape)myShape2, diagram);
            //this.project1.Repository.Update();

            Class myclass = new Class("Toto");
            myclass.Attributes.Add(new Model.Attribute("int","age"));
            myclass.Attributes.Add(new Model.Attribute("string","name"));
            List<string> ls = new List<string>(){"food","drink"};
            myclass.Methods.Add(new Method("void","eat",ls));
            //DrawSingleBox(myclass,100,100);

            display1.OpenDiagram("Test NShape diagram");
        }


        public void DrawBoxes(List<ABox> boxes)
        {
            // Clear existing drawing
            diagram.Clear();
            drawedShapes = new List<Tuple<Shape, string>>();

            // Draw boxes
            int x = 100, y = 100;
            //foreach (ABox box in boxes)
            while(boxes.Count>0) // Throw exception ????
            {
                ABox box = boxes[0];
                boxes.Remove(box);
                Shape sh1 = DrawSingleBox(box, x, y);
                drawedShapes.Add(Tuple.Create(sh1, box.Name));
                x += 220;
                if (box.IsLinked == true)
                {
                    foreach (ABox b in box.Linked)
                    {
                        boxes.Remove(b);
                        bool drawed = false;
                        Shape sh2=null;
                        foreach (Tuple<Shape,string> t in drawedShapes)
                        {
                            if (t.Item2 == b.Name)
                            {
                                drawed = true;
                                sh2 = t.Item1;
                            }
                                
                        }
                        if (drawed == false)
                        {
                            sh2 = DrawSingleBox(b, x, y);
                            drawedShapes.Add(Tuple.Create(sh2, b.Name));
                        }
                            

                        PolylineBase arrow = (PolylineBase)project1.ShapeTypes["Polyline"].CreateInstance();
                        // Add shape to the diagram
                        diagram.Shapes.Add(arrow);
                        // Connect one of the line shape's endings (first vertex) to the referring shape's reference point
                        arrow.Connect(ControlPointId.FirstVertex, sh1, ControlPointId.Reference);
                        // Connect the other of the line shape's endings (last vertex) to the referred shape
                        arrow.Connect(ControlPointId.LastVertex, sh2, ControlPointId.Reference);
                    }
                    
                }
                x += 220;
            }
        }

        private Shape DrawSingleBox(ABox box, int x, int y)
        {
            
            
            // Generate string
            string s = box.Name+"\n_______________";

            foreach (Model.Attribute att in box.Attributes)
            {
                s += "\n\t" + att.Type + " " + att.Name;
            }
            s += "\n_______________";
            foreach (Model.Method met in box.Methods)
            {
                s += "\n\t" + met.ReturnType + " " + met.Name + "(";
                foreach (string str in met.ParamTypes)
                {
                    s += str + ", ";
                }
                if(met.ParamTypes.Count>0)
                    s = s.Remove(s.Length - 2);
                s += ")";
            }


            // Draw the box
            RectangleBase myShape2 = (RectangleBase)this.project1.ShapeTypes["RoundedBox"].CreateInstance();
            myShape2.DisplayService = this.display1;
            myShape2.Height = 250;
            myShape2.Width = 200;
            myShape2.SetCaptionText(0, s);
            myShape2.MoveTo(x, y);
            diagram.Shapes.Add(myShape2);
            this.project1.Repository.Insert((Shape)myShape2, diagram);
            this.project1.Repository.Update();


            // return the shape
            return myShape2;
        }


    }
}
