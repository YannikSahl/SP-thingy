using System;
using System.Collections.Generic;
using System.Windows;
using GUI.Properties;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class Application : System.Windows.Application
    {
        public enum Skins
        {
            ColorLess,
            Light,
            Dark,
            Nebula
        }
        public Skins Skin { get; set; } = Skins.Light;
        private readonly Dictionary<Skins, string> skinReferencesDictionary = new Dictionary<Skins, string>
        {
            {Skins.ColorLess, "./skins/colorless.xaml"},
            {Skins.Light, "./skins/light.xaml"},
            {Skins.Dark, "./skins/dark.xaml"},
            {Skins.Nebula, "./skins/nebula.xaml"},
        };

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            //Settings1.Default.Save();
        }

        private Skins GetSkinFromSaveFile()
        {
            var skinIndexFromSettings = Settings1.Default.SkinId;
            try
            {
                var si = (Skins) skinIndexFromSettings;
            }
            catch (Exception e)
            {
                return (Skins)0;
            }

            return (Skins)skinIndexFromSettings;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ChangeSkin(GetSkinFromSaveFile());
        }

        public void ChangeSkin(Skins newSkin)
        {
            Skin = newSkin;
            //Resources.Clear();
            //Resources.MergedDictionaries.Clear();
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

        private void ApplyResources(string src)
        {
            var skinDict = new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) };
            var correctlyLoaded = true;

            foreach (var key in Resources.MergedDictionaries[0].Keys)
            {
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
                    continue;
                }
            }

            if (!correctlyLoaded)
                MessageBox.Show(
                    "Der ausgewählte Skin wurde nicht richtig geladen. Die Resource Namen des Skins müssen mit den Resourcen Namen der App übereinstimmen.",
                    "Fehler beim Laden des Skins",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK
                    );


            //foreach (var mergeDict in dict.MergedDictionaries)
            //{
            //    Resources.MergedDictionaries.Add(mergeDict);
            //}

            //foreach (var key in dict.Keys)
            //{
            //    Resources[key] = dict[key];
            //}
        }
    }
}
