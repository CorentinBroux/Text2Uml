using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage.UserInput
{
    [Serializable]
    public class UserStructureSet
    {
        #region Fields and properties
        public string Name { get; set; }
        public List<UserStructure> Structures { get; set; }
        [NonSerialized]
        public string path;
        #endregion

        #region Constructors
        public UserStructureSet()
        {
            Structures = new List<UserStructure>();
        }

        public UserStructureSet(string name)
            : this()
        {
            Name = name;
        }

        public UserStructureSet(string name, List<UserStructure> structures)
            : this(name)
        {
            Structures = structures;
        }
        #endregion

        #region Methods
        public void AddStructure(UserStructure structure)
        {
            if(!Structures.Contains(structure))
                Structures.Add(structure);
        }
        public void SaveToFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = Name;
            dlg.DefaultExt = ".uss";
            dlg.Filter = "User Structure Set (.uss)|*.uss";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
                using (var ms = new MemoryStream())
                using (var fs = new FileStream(dlg.FileName, FileMode.Create))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, this);
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }
        }

        public static UserStructureSet LoadFromFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".uss";
            dlg.Filter = "User Structure Set (.uss)|*.uss";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                UserStructureSet uss;
                using (FileStream fs = new FileStream(dlg.FileName, FileMode.Open))
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    uss = (UserStructureSet)formatter.Deserialize(ms); // error if empty file
                }
                uss.path = dlg.FileName;
                return uss;
            }
            else return null;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, path);
        }

        #endregion
    }
}
