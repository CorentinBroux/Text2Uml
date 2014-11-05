using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Text2UML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set path to the sample diagram and the diagram file extension
            xmlStore1.DirectoryName = @"C:\Documents and Settings\All Users\Common Files\NShape\Demo Projects";
            xmlStore1.FileExtension = "nspj";
            // Set the name of the project that should be loaded from the store
            project1.Name = "Circles";
            project1.LibrarySearchPaths.Add(@"C:\Documents and Settings\All Users\Common Files\NShape\bin\Debug");
            project1.AutoLoadLibraries = true;
            // Open the NShape project
            project1.Open();
            // Load the diagram and display it
            display1.LoadDiagram("Diagram 1");

        }
    }
}
