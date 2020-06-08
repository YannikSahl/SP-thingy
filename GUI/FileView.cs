using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.SharePoint.Client;
using File = System.IO.File;

namespace GUI
{
    class FileView : Button
    {
        private static Dictionary<string, SolidColorBrush> presetList = new Dictionary<string, SolidColorBrush>
        {
            {".jpg", new SolidColorBrush(Colors.Red)},
            {".pptx", new SolidColorBrush(Colors.DodgerBlue)},
            {".png", new SolidColorBrush(Colors.Orange)},
            {".pdf", new SolidColorBrush(Colors.DarkRed)}
        };

        private SolidColorBrush _color;
        private string _displayText;
        private string _fileExtension;
        private string _filePath;
        private string _fileName;

        public FileView(string path)
        {
            if (File.Exists(path))
            {
                _filePath = path;
                _fileExtension = Path.GetExtension(path);
                if(! FileView.presetList.TryGetValue(_fileExtension, out _color))
                    _color = new SolidColorBrush(Colors.Azure);
                _fileName = Path.GetFileName(path);
                _displayText = _fileExtension;
            }

            base.Width = 30;
            base.Height = 50;
            base.Margin = new Thickness(5);
            base.Background = _color;
            base.AddText(_displayText);
        }

        protected override void OnClick()
        {
            //File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            //System.Diagnostics.Process.Start(@"D:\");
            base.OnClick();
        }

        public string GetFileName()
        {
            return _fileName;
        }
    }
}
