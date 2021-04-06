using Hearthstone_Deck_Tracker;
using System.IO;
using System.Xml;

namespace PackTracker.Storage
{
    internal class XmlSettings : ISettingsStorage
    {
        public Settings Fetch()
        {
            var Settings = new Settings();

            var path = Path.Combine(Config.AppDataPath, "PackTracker", "Settings.xml");
            if (File.Exists(path))
            {
                var Xml = new XmlDocument();
                Xml.Load(path);
                var Root = Xml.SelectSingleNode("settings");

                if (Root != null)
                {
                    if (bool.TryParse(Root.SelectSingleNode("spoil").InnerText, out var spoil))
                    {
                        Settings.Spoil = spoil;
                    }

                    try
                    {
                        if (bool.TryParse(Root.SelectSingleNode("pityoverlay").InnerText, out var pityoverlay))
                        {
                            Settings.PityOverlay = pityoverlay;
                        }
                    }
                    catch
                    {
                        Settings.PityOverlay = true;
                    }

                    if (bool.TryParse(Root.SelectSingleNode("update").InnerText, out var update))
                    {
                        Settings.Update = update;
                    }
                }
            }

            return Settings;
        }

        public void Store(Settings Settings)
        {
            var Xml = new XmlDocument();
            Xml.AppendChild(Xml.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlNode Root = Xml.CreateElement("settings");
            Xml.AppendChild(Root);

            XmlNode SpoilNode = Xml.CreateElement("spoil");
            SpoilNode.InnerText = Settings.Spoil.ToString();
            Root.AppendChild(SpoilNode);

            XmlNode PityOverlayNode = Xml.CreateElement("pityoverlay");
            PityOverlayNode.InnerText = Settings.PityOverlay.ToString();
            Root.AppendChild(PityOverlayNode);

            XmlNode UpdateNode = Xml.CreateElement("update");
            UpdateNode.InnerText = Settings.Update.ToString();
            Root.AppendChild(UpdateNode);


            var path = Path.Combine(Config.AppDataPath, "PackTracker");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, "Settings.xml");

            Xml.Save(path);
        }
    }
}
