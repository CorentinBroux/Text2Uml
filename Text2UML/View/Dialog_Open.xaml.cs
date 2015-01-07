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
using System.Windows.Shapes;
using System.IO;

namespace Text2UML.View
{
    /// <summary>
    /// Interaction logic for Dialog_Open.xaml
    /// </summary>
    public partial class Dialog_Open : Window
    {
        //public static string Return { get; set; }
        //public static string Type { get; set; }
        MainWindow MW;
        public Dialog_Open(MainWindow mw)
        {
            InitializeComponent();
            MW=mw;
        }

        private void bt_open_pseudocode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //Return = LoadPseudoCode();
            //Type = "ps";

            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                using (StreamReader sr = new StreamReader(filename))
                {
                    MW.TB_PseudoCode.Text = sr.ReadToEnd();
                }
            }

            MW.pseudocodeTabItem.IsSelected = true;
            MW.TB_PseudoCode.CaretIndex = MW.TB_PseudoCode.Text.Length;
            MW.TB_PseudoCode.ScrollToEnd();
            this.Close();
        }

        private void bt_open_nativcode_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                using (StreamReader sr = new StreamReader(filename))
                {
                    MW.TB_NativeLanguage.Text = sr.ReadToEnd();
                }
            }

            MW.pseudocodeTabItem.IsSelected = true;
            MW.TB_NativeLanguage.CaretIndex = MW.TB_NativeLanguage.Text.Length;
            MW.TB_NativeLanguage.ScrollToEnd();
            this.Close();
        }
    }
}
