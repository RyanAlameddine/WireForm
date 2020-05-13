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
    internal class LocalSaver : ISaver
    {
        readonly OpenFileDialog openFileDialog;
        readonly SaveFileDialog saveFileDialog;

        public LocalSaver(OpenFileDialog openFileDialog, SaveFileDialog saveFileDialog)
        {
            this.openFileDialog = openFileDialog;
            this.saveFileDialog = saveFileDialog;
        }

        public string GetJson()
        {
            openFileDialog.Filter = "Json|*.json";
            openFileDialog.Title = "Load your wireForm";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName.Length == 0) return "";

            return File.ReadAllText(openFileDialog.FileName);
        }

        public string WriteJson(string json, string locationIdentifier)
        {
            if (locationIdentifier.Length == 0) 
            {
                saveFileDialog.Filter = "Json|*.json";
                saveFileDialog.Title = "Save your wireForm";
                saveFileDialog.ShowDialog();
                locationIdentifier = saveFileDialog.FileName;
            }
            if (locationIdentifier.Length == 0) return "";

            File.WriteAllText(locationIdentifier, json);
            return locationIdentifier;
        }
    }
}
