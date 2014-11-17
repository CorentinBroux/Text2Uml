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

namespace Text2UML
{
    public partial class Form1 : System.Windows.Forms.UserControl
    {
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
          

            
            Diagram diagram = new Diagram("Test NShape diagram");
            this.display1.Diagram = diagram;
            this.cachedRepository1.Insert(diagram);
            // Now add shape to diagram:
            Shape myShape = this.project1.ShapeTypes["Ellipse"].CreateInstance();
            myShape.DisplayService = this.display1;
            myShape.MoveTo(diagram.Width / 2, diagram.Height / 2);  // Move the shape to the desired position
            diagram.Shapes.Add(myShape);
            this.project1.Repository.Insert(myShape, diagram);
            this.project1.Repository.Update();
            
            // Now add shape to diagram:
            Shape myShape2 = this.project1.ShapeTypes["RoundedBox"].CreateInstance();
            myShape2.DisplayService = this.display1;
            myShape2.MoveTo(diagram.Width / 3, diagram.Height / 3);  // Move the shape to the desired position
            diagram.Shapes.Add(myShape2);
            this.project1.Repository.Insert(myShape2, diagram);
            this.project1.Repository.Update();
            

            display1.OpenDiagram("Test NShape diagram");
        }

    }
}
