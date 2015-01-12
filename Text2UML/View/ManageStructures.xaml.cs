using ITI.Text2UML.Parsing.NaturalLanguage.UserInput;
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

namespace Text2UML.View
{
    /// <summary>
    /// Interaction logic for ManageStructures.xaml
    /// </summary>
    public partial class ManageStructures : Window
    {
        public ManageStructures()
        {
            InitializeComponent();
            if (MainWindow.CurrentUserStructureSets == null)
                return;


            list_structures.ItemsSource = MainWindow.CurrentUserStructureSets;

            ContextMenu cm = new ContextMenu();
            MenuItem miDelete = new MenuItem();
            miDelete.Header = "Remove selected structure set";
            miDelete.Click += miDelete_Click;
            cm.Items.Add(miDelete);
            MenuItem miAdd = new MenuItem();
            miAdd.Header = "Load a structure set";
            miAdd.Click +=miAdd_Click;
            cm.Items.Add(miAdd);
            this.list_structures.ContextMenu = cm;
        }


        public void miDelete_Click(Object sender, RoutedEventArgs e)
        {
            UserStructureSet uss = (UserStructureSet)list_structures.SelectedItem;
            if (uss == null)
                return;
            MainWindow.CurrentUserStructureSets.Remove(uss);
            list_structures.ItemsSource = null;
            list_structures.ItemsSource = MainWindow.CurrentUserStructureSets;
        }

        public void miAdd_Click(Object sender, RoutedEventArgs e)
        {
            MainWindow.CurrentUserStructureSets.Add(UserStructureSet.LoadFromFile());
            list_structures.ItemsSource = null;
            list_structures.ItemsSource = MainWindow.CurrentUserStructureSets;
        }
    }
}
