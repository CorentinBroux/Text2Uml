#define DEBUG

using System;
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
using Text2UML.Model;
using System.IO;

namespace Text2UML
{
    public partial class MainWindow : Window
    {
        Form1 myform;
        public MainWindow()
        {
            InitializeComponent();

            myform = new Form1();
            
            // Initialize WinForms PropertyGrid
            propertyGridHost.Child = myform;

        }

        private void BT_Process_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            try
            {
                Tuple<List<ABox>, List<Link>> tuple = Parser.ExtractAboxes(Formatter.FormatForTokenization(TB_PseudoCode.Text));
                List<ABox> boxes = tuple.Item1;
                List<Link> links = tuple.Item2;
                Parser.ReportDeadLinks(links, boxes);
                Parser.AddLinksToBoxes(links, boxes);
                myform.DrawBoxes(boxes);
                double h = View.Drawer.DrawBoxes(boxes, canvas1);
                canvas1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                canvas1.Height = h;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }




        }

        public MainWindow getThis()
        {
            return this;
        }

        private void BT_Open_PC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadPseudoCodeFromFile();
        }

        public void LoadPseudoCodeFromFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                using (StreamReader sr = new StreamReader(filename))
                {
                    TB_PseudoCode.Text = sr.ReadToEnd();
                }
            }
        }

        private void submenu_lang_en_Click(object sender, RoutedEventArgs e)
        {
            ChangeCheckedLanguage("en");
        }

        private void submenu_lang_fr_Click(object sender, RoutedEventArgs e)
        {
            ChangeCheckedLanguage("fr");
        }

        private void ChangeCheckedLanguage(string To)
        {
            if(To=="en")
            {
                submenu_lang_fr.IsChecked = false;
                submenu_lang_en.IsChecked = true;
            }else if (To == "fr")
            {
                submenu_lang_en.IsChecked = false;
                submenu_lang_fr.IsChecked = true;
            }
        }


        private void OpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            View.Dialog_Open diao = new View.Dialog_Open(getThis());
            diao.Owner = this;
            diao.ShowInTaskbar = false;
            diao.ShowDialog();
        }


        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            View.Dialog_Save dias = new View.Dialog_Save();
            dias.Owner = this;
            dias.ShowInTaskbar = false;
            dias.ShowDialog();
        }


        private void ExportCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SaveCanvasAsImage();
        }

        private void SaveCanvasAsImage()
        {
            // Configure open file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Images"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Text documents (.png)|*.png"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Save document 
                string filename = dlg.FileName;
                Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas1);
                double dpi = 96d;


                RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);


                var enc = new System.Windows.Media.Imaging.PngBitmapEncoder(); 
                enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(rtb));

                using (var stm = System.IO.File.Create(filename)) 
                { 
                    enc.Save(stm); 
                } 

            }
        }
    }

    
}