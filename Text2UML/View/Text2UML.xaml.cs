﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Text2UML
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Form1 myform = new Form1();
            // Initialize WinForms PropertyGrid
            propertyGridHost.Child = myform;

        }


    }
}
