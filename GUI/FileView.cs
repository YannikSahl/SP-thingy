using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;
using File = System.IO.File;
using SystemColors = System.Drawing.SystemColors;

namespace GUI
{
    class FileView : DockPanel
    {
        //private static Dictionary<string, SolidColorBrush> presetList = new Dictionary<string, SolidColorBrush>
        //{
        //    {".jpg", new SolidColorBrush(Colors.Red)},
        //    {".pptx", new SolidColorBrush(Colors.DodgerBlue)},
        //    {".png", new SolidColorBrush(Colors.Orange)},
        //    {".pdf", new SolidColorBrush(Colors.DarkRed)}
        //};
        private static Dictionary<string, LinearGradientBrush> _presetList;

        public enum View
        {
            Text,
            TextAndImage
        }

        public static View _view = View.TextAndImage;

        //private SolidColorBrush _color;
        private LinearGradientBrush _color;
        private string _displayText;
        private string _fileExtension;
        private string _filePath;
        private string _fileName;

        public FileView(string path)
        {
            _presetList = new Dictionary<string, LinearGradientBrush>();
            SetColorDict();
            if (File.Exists(path))
            {
                _filePath = path;
                _fileExtension = Path.GetExtension(path);
                if(! FileView._presetList.TryGetValue(_fileExtension, out _color))
                    _color = new LinearGradientBrush(Colors.Azure, Colors.Azure, 0);//_color = new SolidColorBrush(Colors.Azure);
                _fileName = Path.GetFileName(path).Trim(_fileExtension.ToCharArray());
                
                _displayText = _fileName;

                SetupSelf();
                // erstmal image einsetzen danach text um last child zu füllen
                if (_view == View.TextAndImage)
                    AddImageToView();
            }
            else
            {
                _displayText = "Datei existiert nicht!";
            }

            AddTextToView();
        }

        private System.Drawing.Color DrawingColorFromHex(string hexColor)
        {
            int argb = Int32.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }

        private System.Windows.Media.Color MediaColorFromHex(string hexColor)
        {
            return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hexColor);
        }

        private void SetColorDict()
        {
            // pptx gradient erstellen
            var col = new GradientStop(MediaColorFromHex("#FFFF660F"), 1);
            GradientStopCollection g = new GradientStopCollection();
            g.Add(col);
            col = new GradientStop(MediaColorFromHex("#FFFF4C17"), 0.306);
            g.Add(col);
            col = new GradientStop(MediaColorFromHex("#FFFF6A24"), 0.317);
            g.Add(col);
            col = new GradientStop(MediaColorFromHex("#FFFF8825"), 0.689);
            g.Add(col);
            col = new GradientStop(MediaColorFromHex("#FFFF9C38"), 0.698);
            g.Add(col);
            col = new GradientStop(MediaColorFromHex("#FFFF3622"), 0);
            g.Add(col);
            LinearGradientBrush ln = new LinearGradientBrush(g);
            ln.StartPoint = new System.Windows.Point(0.5, 0);
            ln.EndPoint = new System.Windows.Point(0.5, 1);
            _presetList.Add(".pptx", ln);

            // hier noch PDF und PNG gradienten erstellen
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
            fView.BorderThickness = new Thickness(0);
            fView.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            fView.Margin = new Thickness(5);
            fView.Background = _color;
            grid.Children.Add(fView);

            // setup and add text to grid
            var fViewText = new TextBlock();
            fViewText.IsHitTestVisible = false;
            fViewText.Text = _fileExtension;
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
