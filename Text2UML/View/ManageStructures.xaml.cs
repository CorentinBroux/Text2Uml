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
        public ManageStructures(List<UserStructureSet> usss = null)
        {
            InitializeComponent();
            if (usss == null)
                return;

            foreach (UserStructureSet uss in usss)
                this.list_structures.Items.Add(uss);

            this.list_structures.ContextMenu = new ContextMenu().menu
        }
    }
}
