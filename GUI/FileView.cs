using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace GUI
{
    internal class FileView : DockPanel
    {
        #region constructors

        public FileView(string path)
        {
            _filePath = Path.GetFullPath(path);
            _directoryPath = Path.GetDirectoryName(_filePath);
            if (File.Exists(_filePath))
            {
                // init colors
                _presetList = new Dictionary<string, LinearGradientBrush>();
                SetColorDict();

                _fileExtension = Path.GetExtension(_filePath);
                if (!_presetList.TryGetValue(_fileExtension, out _iconColor))
                    _iconColor = new LinearGradientBrush(Colors.Azure, Colors.Azure, 0);
                _fileName = Path.GetFileName(_filePath);
                _fileName = _fileName.Remove(_fileName.Length - _fileExtension.Length);
                _displayText = _fileName;

                SetupSelf();
                // erstmal image einsetzen danach text um last child zu füllen
                if (FileViewStyle == View.TextAndImage)
                    AddImageToView();
            }
            else
            {
                // file not found
                _displayText = _filePath + ": Datei existiert nicht!";
            }

            AddTextToView();
        }

        #endregion

        #region statics

        private static Dictionary<string, LinearGradientBrush> _presetList;

        public enum View
        {
            Text,
            TextAndImage
        }

        #endregion

        #region members

        public static View FileViewStyle = View.TextAndImage;

        // 
        private readonly LinearGradientBrush _iconColor;
        private readonly string _displayText;
        private readonly string _fileExtension;
        private readonly string _filePath;
        private readonly string _directoryPath;
        private readonly string _fileName;

        private const string BaseColorName = "BackgroundTernaryColor";
        private const string MouseOverColor = "BackgroundSecondaryColor";

        #endregion

        #region methods

        /// <summary>
        ///     gets System.Drawing.Color from hex number
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        private Color DrawingColorFromHex(string hexColor)
        {
            var argb = int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }

        /// <summary>
        ///     gets System.Windows.Media.Color from hex number
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        private System.Windows.Media.Color MediaColorFromHex(string hexColor)
        {
            return (System.Windows.Media.Color) ColorConverter.ConvertFromString(hexColor);
        }

        /// <summary>
        ///     takes a list of Colors (string hex) of length 6 and projects it as a LinearGradientBrush
        /// </summary>
        /// <param name="hexColorList"></param>
        /// <returns></returns>
        private LinearGradientBrush CreateLinearBrushFromColors(string[] hexColorList)
        {
            LinearGradientBrush gradientBrushTemp;
            // 6-er Liste
            double[] pointList = {0d, 0.333, 0.334, 0.666, 0.667, 1d};
            if (hexColorList.Length != pointList.Length)
                return null;

            var gradStopCollTemp = new GradientStopCollection();
            for (var i = 0; i < pointList.Length; i++)
            {
                var GradStopTemp = new GradientStop(MediaColorFromHex(hexColorList[i]), pointList[i]);
                gradStopCollTemp.Add(GradStopTemp);
            }

            gradientBrushTemp = new LinearGradientBrush(gradStopCollTemp);
            gradientBrushTemp.StartPoint = new Point(0.5, 0);
            gradientBrushTemp.EndPoint = new Point(0.5, 1);

            return gradientBrushTemp;
        }

        /// <summary>
        ///     sets up all the template colors
        /// </summary>
        private void SetColorDict()
        {
            // PPTX
            string[] pptx_hexColorList =
            {
                "#FFFF3622",
                "#FFFF4C17",
                "#FFFF6A24",
                "#FFFF8825",
                "#FFFF9C38",
                "#FFFF660F"
            };
            // create LinearGradientBrush
            var pptx_gradBrush = CreateLinearBrushFromColors(pptx_hexColorList);
            // add Brush to Dict
            _presetList.Add(".pptx", pptx_gradBrush);

            // PNG
            string[] png_hexColorList =
            {
                "#0E6803",
                "#00D927",
                "#3CAA01",
                "#50E800",
                "#01B722",
                "#004D1E"
            };
            // create LinearGradientBrush
            var png_gradBrush = CreateLinearBrushFromColors(png_hexColorList);
            // add Brush to Dict
            _presetList.Add(".png", png_gradBrush);

            // JPEG
            string[] jpeg_hexColorList =
            {
                "#001ACA",
                "#249DFF",
                "#33C2FF",
                "#0074EC",
                "#0B92E8",
                "#002A99"
            };
            // create LinearGradientBrush
            var jpeg_gradBrush = CreateLinearBrushFromColors(jpeg_hexColorList);
            // add Brush to Dict
            _presetList.Add(".jpeg", jpeg_gradBrush);

            // PDF
            string[] pdf_hexColorList =
            {
                "#FF0004",
                "#E30004",
                "#FD0004",
                "#A20608",
                "#CC0306",
                "#820A0C"
            };
            // create LinearGradientBrush
            var pdf_gradBrush = CreateLinearBrushFromColors(pdf_hexColorList);
            // add Brush to Dict
            _presetList.Add(".pdf", pdf_gradBrush);
        }

        /// <summary>
        ///     sets up WPF base class elements
        /// </summary>
        private void SetupSelf()
        {
            LastChildFill = true;
            //this.Background = _baseColor;
            SetResourceReference(BackgroundProperty, BaseColorName);
            Margin = new Thickness(5);
        }

        /// <summary>
        ///     adds a textpreview of the name
        /// </summary>
        private void AddTextToView()
        {
            var tView = new TextBlock();
            //tView.Width = 100;
            tView.TextWrapping = TextWrapping.Wrap;
            tView.Text = _displayText;
            tView.Foreground =
                (SolidColorBrush) ((Application) System.Windows.Application.Current).Resources["ForegroundColor"];
            //tView.SetResourceReference(B, "ForegroundColor");
            tView.FontSize = (double) ((Application) System.Windows.Application.Current).Resources["FontSizeLarger"];
            tView.VerticalAlignment = VerticalAlignment.Center;
            tView.HorizontalAlignment = HorizontalAlignment.Left;
            tView.Padding = new Thickness(10, 0, 0, 0);
            Children.Add(tView);
            SetDock(tView, Dock.Left);
        }

        /// <summary>
        ///     adds the icon with color and extension name to base class element
        /// </summary>
        private void AddImageToView()
        {
            // setup grid
            var grid = new Grid();
            var row = new RowDefinition
            {
                Height = GridLength.Auto
            };
            var col = new ColumnDefinition
            {
                Width = GridLength.Auto
            };
            grid.RowDefinitions.Add(row);
            grid.ColumnDefinitions.Add(col);
            grid.HorizontalAlignment = HorizontalAlignment.Right;

            // setup and add button to grid
            var fView = new Button
            {
                Width = 50
            };
            fView.Height = fView.Width * 1.414d;
            fView.BorderThickness = new Thickness(2);
            fView.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            fView.Margin = new Thickness(5);
            fView.Background = _iconColor;
            fView.IsHitTestVisible = false;
            grid.Children.Add(fView);

            // setup and add text to grid
            var fViewText = new TextBlock
            {
                IsHitTestVisible = false,
                Text = _fileExtension,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(fViewText);

            Children.Add(grid);
            SetDock(grid, Dock.Right);
        }

        #endregion

        #region events

        /// <summary>
        ///     OnMouseLeftButtonUp event opens directory file is in
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // File.Exists() for file
            if (Directory.Exists(_directoryPath)) Process.Start("explorer.exe", _directoryPath);
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        ///     onMouseEnter event changes color to highlighted
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            //this.Background = MouseOverColor;
            SetResourceReference(BackgroundProperty, MouseOverColor);
            base.OnMouseEnter(e);
        }

        /// <summary>
        ///     onMouseLeave event changes color back to normal
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            //this.Background = _baseColor;
            SetResourceReference(BackgroundProperty, BaseColorName);
            base.OnMouseLeave(e);
        }

        #endregion
    }
}