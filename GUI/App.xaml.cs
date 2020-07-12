using System;
using System.Collections.Generic;
using System.Windows;
using GUI.Properties;

namespace GUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class Application : System.Windows.Application
    {
        // alle vorhandenen skins/themes
        public enum Skins
        {
            ColorLess,
            Light,
            Dark,
            Nebula
        }

        // Pfade zu allen themes
        private readonly Dictionary<Skins, string> skinReferencesDictionary = new Dictionary<Skins, string>
        {
            {Skins.ColorLess, "./skins/colorless.xaml"},
            {Skins.Light, "./skins/light.xaml"},
            {Skins.Dark, "./skins/dark.xaml"},
            {Skins.Nebula, "./skins/nebula.xaml"}
        };

        // derzeitiges theme
        public Skins Skin { get; set; } = Skins.Light;

        /// <summary>
        ///     loads theme from settings
        /// </summary>
        /// <returns></returns>
        private Skins GetSkinFromSaveFile()
        {
            var skinIndexFromSettings = Settings1.Default.SkinId;
            try
            {
                var si = (Skins) skinIndexFromSettings;
            }
            catch (Exception e)
            {
                return 0;
            }

            return (Skins) skinIndexFromSettings;
        }

        /// <summary>
        ///     on startup event, load theme
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ChangeSkin(GetSkinFromSaveFile());
        }

        /// <summary>
        ///     change theme
        /// </summary>
        /// <param name="newSkin"></param>
        public void ChangeSkin(Skins newSkin)
        {
            Skin = newSkin;
            try
            {
                ApplyResources(skinReferencesDictionary[newSkin]);
                Settings1.Default.SkinId = (int) Skin;
                Settings1.Default.Save();
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     applies theme selection
        /// </summary>
        /// <param name="src"></param>
        private void ApplyResources(string src)
        {
            var skinDict = new ResourceDictionary {Source = new Uri(src, UriKind.Relative)};
            var correctlyLoaded = true;

            // iterate through every visual resource entry and replace
            foreach (var key in Resources.MergedDictionaries[0].Keys)
                try
                {
                    var value = skinDict[key];
                    if (value == null)
                        correctlyLoaded = false;
                    Resources[key] = value;
                }
                catch (KeyNotFoundException e)
                {
                    correctlyLoaded = false;
                }

            // errormessage
            if (!correctlyLoaded)
                MessageBox.Show(
                    "Der ausgewählte Skin wurde nicht richtig geladen. Die Resource Namen des Skins müssen mit den Resourcen Namen der App übereinstimmen.",
                    "Fehler beim Laden des Skins",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK
                );
        }
    }
}