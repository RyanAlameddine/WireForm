using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wireform;

namespace WinformsWireform
{
    class LocalSaveable : ISaveable
    {
        OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;

        public LocalSaveable(OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog)
        {
            this.openFileDialog = openFileDialog;
            this.saveFileDialog = saveFileDialog;
        }

        public string GetJson(out string locationIdentifier)
        {
            openFileDialog.Filter = "Json|*.json";
            openFileDialog.Title = "Load your wireForm";
            openFileDialog.ShowDialog();

            locationIdentifier = openFileDialog.FileName;

            if (locationIdentifier == "") return "";

            return File.ReadAllText(locationIdentifier);
        }

        public string WriteJson(string json, string locationIdentifier)
        {
            if (locationIdentifier == "") 
            {
                saveFileDialog.Filter = "Json|*.json";
                saveFileDialog.Title = "Save your wireForm";
                saveFileDialog.ShowDialog();
                locationIdentifier = saveFileDialog.FileName;
            }
            if (locationIdentifier == "") return "";

            File.WriteAllText(locationIdentifier, json);
            return locationIdentifier;
        }
    }
}
