﻿using System;
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
using System.Windows.Shapes;

namespace Text2UML.View
{
    /// <summary>
    /// Interaction logic for Dialog_Open.xaml
    /// </summary>
    public partial class Dialog_Save : Window
    {
        public Dialog_Save()
        {
            InitializeComponent();
        }

        private void bt_save_pc_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)this.Owner;
            mw.SavePseudoCode();
            this.Close();
        }
    }
}
