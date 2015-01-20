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
using Dataweb.NShape.Commands;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Globalization;

namespace Text2UML
{
    public partial class Form1 : System.Windows.Forms.UserControl
    {
        Diagram diagram;
        List<Tuple<Shape,string>> drawedShapes = new List<Tuple<Shape,string>>();
        public List<Class> _boxes;
        public List<Link> _links;

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

        #region Draw Boxes/Entity
        public void DrawBoxes(List<ITI.Text2UML.Model.Class> boxes)
        {
            int X = display1.Diagram.Size.Width-100;

            int ymax = 0;

            // Clear existing drawing
            this.diagram.Clear();
            drawedShapes = new List<Tuple<Shape, string>>();

            // Draw boxes
            int x = 100, y = 100;
            foreach (ITI.Text2UML.Model.Class box in boxes)
            {
                
                if (x > X)
                {
                    x = 100;
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
                    foreach (Tuple<Class,LinkTypes, string> tuple in box.Linked)
                    {
                        Class b = tuple.Item1;
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
                        this.project1.Repository.Insert((Shape)arrow, diagram);

                        if (tuple.Item2 == LinkTypes.Extends)
                            arrow.EndCapStyle = project1.Design.CapStyles.ClosedArrow;
                        else
                            arrow.EndCapStyle = project1.Design.CapStyles.OpenArrow;
                        arrow.Connect(ControlPointId.FirstVertex, sh1, ControlPointId.Reference);
                        arrow.Connect(ControlPointId.LastVertex, sh2, ControlPointId.Reference);


                        Point p1 = arrow.GetControlPointPosition(arrow.GetControlPointIds(ControlPointCapabilities.Glue).First());
                        Point p2 = arrow.GetControlPointPosition(arrow.GetControlPointIds(ControlPointCapabilities.Glue).Last());
                        int dx  = p2.X - p1.X;
                        int dy = p2.Y - p1.Y;

                        Point dstPos = Point.Empty;
                        dstPos.X = p1.X + (int)(dx / 2f);
                        dstPos.Y = p1.Y + (int)(dy / 2f);
                        
                        // Get ControlPointId of the label's glue point

                        Dataweb.NShape.GeneralShapes.Label cardin = (Dataweb.NShape.GeneralShapes.Label)project1.ShapeTypes["Label"].CreateInstance();

                        ControlPointId gluePtId = ControlPointId.None;
                        foreach (ControlPointId id in cardin.GetControlPointIds(ControlPointCapabilities.Glue))
                            gluePtId = id;
                        
                        // Move glue point to desired position and connect with the line (Point-To-Shape connection with reference point)
                        cardin.MoveTo(dstPos.X + 20, dstPos.Y + 20);
                        cardin.MoveControlPointTo(gluePtId, dstPos.X, dstPos.Y, ResizeModifiers.None);
                        cardin.Connect(gluePtId, arrow, ControlPointId.Reference);
                        //cardin.MaintainOrientation = true; // Label will rotate when the line's angle changes
                        cardin.SetCaptionText(0, tuple.Item3);
                        diagram.Shapes.Add(cardin);   
                        this.project1.Repository.Insert((Shape)cardin, diagram);
                        
                        
                    }
                    
                }
                
            }
            this.project1.Repository.Update();

        }

        private Shape DrawSingleEntity(ITI.Text2UML.Model.Class box, int x, int y, ref Size size)
        {
            // Draw the box
            ClassSymbol myShape2 = (ClassSymbol)this.project1.ShapeTypes["ClassSymbol"].CreateInstance();
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
           // myShape2.MoveTo(x, y);
            myShape2.X = x;
            myShape2.Y = y;
            diagram.Shapes.Add(myShape2);
            this.project1.Repository.Insert((Shape)myShape2, diagram);
            this.project1.Repository.Update();
            this.display1.Diagram = diagram;
            this.display1.EnsureVisible(new Rectangle(0,0,100,100));
            
            // return the shape
            return myShape2;
        }
        #endregion

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

        public void SaveText2UMLProject(string filename, string nativelanguage )
        {
            #region _boxes x and y initialize (placement on diagram)
            foreach (Shape s in diagram.Shapes)
            {
                Console.WriteLine(s.GetType().ToString());
                if (s.GetType().ToString() != "Dataweb.NShape.GeneralShapes.Polyline" && s.GetType().ToString() != "Dataweb.NShape.GeneralShapes.Label")
                {
                    RectangleBase s2 = (RectangleBase)s;
                    string name = s2.GetCaptionText(0);

                    
                    int i = name.IndexOf("\n");
                    name = name.Substring(0, i);

                    foreach (ITI.Text2UML.Model.Class box in _boxes)
                    {
                        if (name == box.Name)
                        {
                            box.x = s.X;
                            box.y = s.Y;
                        }
                    }
                }

            }
            #endregion

            #region XML creation script
            try
            {
                var xEle =  new XElement("Root",
                    
                            new XElement("Shapes",
                            from box in _boxes
                            select new XElement("Shape",
                                           new XAttribute("Name", box.Name),

                                           new XElement("Attributes",
                                               from att in box.Attributes
                                                 select new XElement("Attribute",
                                                     new XAttribute("Name", att.Name),
                                                      new XAttribute("Type", att.Type))),

                                           new XElement("Methods", 
                                               from met in box.Methods
                                                 select new XElement("Method",
                                                     new XAttribute("Name", met.Name),
                                                      new XAttribute("ReturnType", met.ReturnType),

                                                      new XElement("ParamTypes",
                                                          from param in met.ParamTypes
                                                          select new XElement("ParamType", param.ToString())
                                                          ))),

                                           new XAttribute("IsLinked", box.IsLinked),

                                            new XElement("Linked",
                                               from tup in box.Linked
                                               select new XElement("Link",
                                                   new XAttribute("ClassName", tup.Item1.Name),
                                                    new XAttribute("LinkType", tup.Item2))),

                                            new XAttribute("x", box.x),
                                            new XAttribute("y", box.y)
                                       )),

                                new XElement("Links",
                                from link in _links
                                 select new XElement("Link",
                                             new XAttribute("From", link.From),
                                             new XAttribute("To", link.To),
                                             new XAttribute("Type", link.Type.ToString()),
                                             new XAttribute("Label", link.Label)
                                           )),

                                new XAttribute("NativeLanguage", nativelanguage)

                                       );

                xEle.Save(filename);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion

        }

        public void LoadText2UMLDiagram(List<Class> boxes, List<Link> links)
        {
            DrawBoxesAfterLoad(boxes);
        }

        #region Draw Boxes/Entity After Load
        public void DrawBoxesAfterLoad(List<ITI.Text2UML.Model.Class> boxes)
        {
            // Clear existing drawing
            this.diagram.Shapes.Clear();
            this.diagram.Clear();
            drawedShapes = new List<Tuple<Shape, string>>();
            
            // Draw boxes
            int X = display1.Diagram.Size.Width - 100;

            int ymax = 0;

            foreach (ITI.Text2UML.Model.Class box in boxes)
            {
                int x = box.x, y = box.y;
                if (x > X)
                {
                    x = 100;
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
                    sh1 = DrawSingleBoxAfterLoad(box, ref size1);
                    drawedShapes.Add(Tuple.Create(sh1, box.Name));
                }


                if (box.IsLinked == true)
                {
                    foreach (Tuple<Class, LinkTypes, string> tuple in box.Linked)
                    {
                        Class b = tuple.Item1;
                        bool drawed = false;
                        Shape sh2 = null;
                        foreach (Tuple<Shape, string> t in drawedShapes)
                            if (t.Item2 == b.Name)
                            {
                                drawed = true;
                                sh2 = t.Item1;
                            }

                        if (drawed == false)
                        {
                            Size size2 = new Size();
                            sh2 = DrawSingleBoxAfterLoad(b, ref size2);
                            drawedShapes.Add(Tuple.Create(sh2, b.Name));
                        }

                        Polyline arrow = (Polyline)project1.ShapeTypes["Polyline"].CreateInstance();
                        diagram.Shapes.Add(arrow);
                        this.project1.Repository.Insert((Shape)arrow, diagram);
                        if (tuple.Item2 == LinkTypes.Extends)
                            arrow.EndCapStyle = project1.Design.CapStyles.ClosedArrow;
                        else
                            arrow.EndCapStyle = project1.Design.CapStyles.OpenArrow;
                        arrow.Connect(ControlPointId.FirstVertex, sh1, ControlPointId.Reference);
                        arrow.Connect(ControlPointId.LastVertex, sh2, ControlPointId.Reference);

                         Point p1 = arrow.GetControlPointPosition(arrow.GetControlPointIds(ControlPointCapabilities.Glue).First());
                        Point p2 = arrow.GetControlPointPosition(arrow.GetControlPointIds(ControlPointCapabilities.Glue).Last());
                        int dx  = p2.X - p1.X;
                        int dy = p2.Y - p1.Y;

                        Point dstPos = Point.Empty;
                        dstPos.X = p1.X + (int)(dx / 2f);
                        dstPos.Y = p1.Y + (int)(dy / 2f);
                        
                        // Get ControlPointId of the label's glue point

                        Dataweb.NShape.GeneralShapes.Label cardin = (Dataweb.NShape.GeneralShapes.Label)project1.ShapeTypes["Label"].CreateInstance();

                        ControlPointId gluePtId = ControlPointId.None;
                        foreach (ControlPointId id in cardin.GetControlPointIds(ControlPointCapabilities.Glue))
                            gluePtId = id;
                        
                        // Move glue point to desired position and connect with the line (Point-To-Shape connection with reference point)
                        cardin.MoveTo(dstPos.X + 20, dstPos.Y + 20);
                        cardin.MoveControlPointTo(gluePtId, dstPos.X, dstPos.Y, ResizeModifiers.None);
                        cardin.Connect(gluePtId, arrow, ControlPointId.Reference);
                        //cardin.MaintainOrientation = true; // Label will rotate when the line's angle changes
                        cardin.SetCaptionText(0, tuple.Item3);
                        diagram.Shapes.Add(cardin);   
                        this.project1.Repository.Insert((Shape)cardin, diagram);
                    }

                }

            }

        }
        #endregion

        #region Draw boxe afer laod
        private Shape DrawSingleBoxAfterLoad(ITI.Text2UML.Model.Class box, ref Size size)
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
            RectangleBase myShape3 = (RectangleBase)this.project1.ShapeTypes["RoundedBox"].CreateInstance();
            myShape3.DisplayService = this.display1;
            size = MeasureString(s);
            myShape3.Height = size.Height + 30;
            myShape3.Width = size.Width + 30;
            myShape3.SetCaptionText(0, s);
            // myShape2.MoveTo(x, y);
            myShape3.X = box.x;
            myShape3.Y = box.y;
            diagram.Shapes.Add(myShape3);
            this.project1.Repository.Insert((Shape)myShape3, diagram);
            this.project1.Repository.Update();
            this.display1.Diagram = diagram;


            // return the shape
            return myShape3;
        }
        #endregion

        #region Layouter - Shape organizer
        private static void ExecuteLayouter(ILayouter layouter, Int32 timeout)
        {
            layouter.Prepare();
            layouter.SaveState();
            layouter.Execute(timeout); 
        }

        private void ExecuteCommand(AggregatedCommand aggregatedCommand, ICommand command)
        {
            aggregatedCommand.Add(command);
            command.Execute();
        }

        public void OrganizeShapes()
        {
            if (display1.Diagram.Shapes.Count == 0)
            {
                return;
            }

            List<Shape> ShapesToLayout = new List<Shape>();

            // First, place all shapes to the same position
            foreach (Shape s in display1.Diagram.Shapes)
            {
                if (s.GetType().Name != "Label")
                {
                    ShapesToLayout.Add(s);
                    s.X = 100;
                    s.Y = 100;
                }

            }

            const int stepTimeout = 10;

            // Aggregated command for executing the 4 layouting steps at once
            AggregatedCommand aggregatedCommand = new AggregatedCommand(project1.Repository);

            ExpansionLayouter expansionLayouter = new ExpansionLayouter(project1);
            expansionLayouter.HorizontalCompression = 50;
            expansionLayouter.VerticalCompression = 80;
            expansionLayouter.AllShapes = ShapesToLayout;
            expansionLayouter.Shapes = ShapesToLayout;
            ExecuteLayouter(expansionLayouter, stepTimeout);
            ExecuteCommand(aggregatedCommand, expansionLayouter.CreateLayoutCommand());


            // Create the layouter and set up layout parameters
            RepulsionLayouter layouter = new RepulsionLayouter(project1);
            // Set the repulsion force and its range
            layouter.SpringRate = 9;
            layouter.Repulsion = 10;
            layouter.RepulsionRange = 330;
            // Set the friction and the mass of the shapes
            layouter.Friction = 10;
            layouter.Mass = 100;
            // Set all shapes 
            layouter.AllShapes = ShapesToLayout;
            // Set shapes that should be layouted
            layouter.Shapes = ShapesToLayout;

            layouter.AllShapes = ShapesToLayout;
            layouter.Shapes = ShapesToLayout;
           ExecuteLayouter(layouter, stepTimeout);
           ExecuteCommand(aggregatedCommand, layouter.CreateLayoutCommand());
            

            expansionLayouter.HorizontalCompression = 200;
            expansionLayouter.VerticalCompression = 200;
            expansionLayouter.AllShapes = ShapesToLayout;
            expansionLayouter.Shapes = ShapesToLayout;
            ExecuteLayouter(expansionLayouter, stepTimeout);
            ExecuteCommand(aggregatedCommand, expansionLayouter.CreateLayoutCommand());

            // Add aggregated command to the history. 
            // Do not execute it as each step was executed before.
            project1.History.AddCommand(aggregatedCommand);

            expansionLayouter.Fit(50, 50, display1.Diagram.Width - 100, display1.Diagram.Height - 100);

        }
        #endregion

        public void ResetDiagram()
        {
            diagram.Clear();
        }

    }
}
