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
    class FileView : DockPanel
    {
        private static Dictionary<string, SolidColorBrush> presetList = new Dictionary<string, SolidColorBrush>
        {
            {".jpg", new SolidColorBrush(Colors.Red)},
            {".pptx", new SolidColorBrush(Colors.DodgerBlue)},
            {".png", new SolidColorBrush(Colors.Orange)},
            {".pdf", new SolidColorBrush(Colors.DarkRed)}
        };

        public enum view
        {
            text,
            textAndImage
        }

        public static view _view = view.textAndImage;

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
                _fileName = Path.GetFileName(path).Trim(_fileExtension.ToCharArray());
                
                _displayText = _fileName;

                SetupSelf();
                // erstmal image einsetzen danach text um last child zu füllen
                if (_view == view.textAndImage)
                    AddImageToView();
            }
            else
            {
                _displayText = "Datei existiert nicht!";
            }

            AddTextToView();
        }

        private void SetupSelf()
        {
            this.LastChildFill = true;
            this.Background = new SolidColorBrush(Colors.WhiteSmoke);
            this.Margin = new Thickness(5);
        }

        private void AddTextToView()
        {
            var tView = new TextBlock();
            tView.Width = 100;
            tView.TextWrapping = TextWrapping.Wrap;
            tView.Text = _displayText;
            tView.VerticalAlignment = VerticalAlignment.Center;
            tView.HorizontalAlignment = HorizontalAlignment.Left;
            tView.Padding = new Thickness(10,0,0,0);
            this.Children.Add(tView);
            DockPanel.SetDock(tView, Dock.Left);
        }

        private void AddImageToView()
        {
            // setup grid
            var grid = new Grid();
            var row = new RowDefinition();
            row.Height = GridLength.Auto;
            var col = new ColumnDefinition();
            col.Width = GridLength.Auto;
            grid.RowDefinitions.Add(row);
            grid.ColumnDefinitions.Add(col);
            grid.HorizontalAlignment = HorizontalAlignment.Right;

            // setup and add button to grid
            var fView = new Button();
            fView.Width = 50;
            fView.Height = fView.Width * 1.414d;
            fView.BorderThickness = new Thickness(2);
            fView.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            fView.Margin = new Thickness(5);
            fView.Background = _color;
            grid.Children.Add(fView);

            // setup and add text to grid
            var fViewText = new TextBlock();
            fViewText.IsHitTestVisible = false;
            fViewText.Text = (string)_fileExtension;
            fViewText.FontSize = 15;
            fViewText.FontWeight = FontWeights.Bold;
            fViewText.VerticalAlignment = VerticalAlignment.Center;
            fViewText.HorizontalAlignment = HorizontalAlignment.Center;
            grid.Children.Add(fViewText);

            this.Children.Add(grid);
            DockPanel.SetDock(grid, Dock.Right);
        }

        /*protected override void OnClick()
        {
            //File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            //System.Diagnostics.Process.Start(@"D:\");
            base.OnClick();
        }*/

        public string GetFileName()
        {
            return _fileName;
        }
    }
}
