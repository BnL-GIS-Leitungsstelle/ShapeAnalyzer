using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeAnalyzer.Utils
{
    public class OpenFileDialogUtils
    {
        public static string ShowOpenFileDialog(string filter)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = filter;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}
