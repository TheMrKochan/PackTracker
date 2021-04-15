using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using PackTracker.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PackTracker.Storage
{
    internal class XmlHistory : IHistoryStorage
    {
        public History Fetch()
        {
            var History = new History();

            var path = Path.Combine(Config.AppDataPath, "PackTracker", "History.xml");
            if (File.Exists(path))
            {
                var Xml = new XmlDocument();
                Xml.Load(path);
                var Root = Xml.SelectSingleNode("history");

                if (Root != null)
                {
                    var Packs = Root.SelectNodes("pack");

                    if (Packs.Count > 0)
                    {
                        foreach (XmlNode Pack in Packs)
                        {
                            if (int.TryParse(Pack.Attributes["id"]?.Value, out var packId) && long.TryParse(Pack.Attributes["time"]?.Value, out var ticks))
                            {
                                var Time = new DateTime(ticks);
                                var Cards = Pack.SelectNodes("card");

                                if (Cards.Count > 0)
                                {
                                    var HistoryCards = new List<Entity.Card>();

                                    foreach (XmlNode Card in Cards)
                                    {
                                        var cardId = Card.Attributes["id"]?.Value;
                                        if (!string.IsNullOrEmpty(cardId))
                                        {
                                            var HDTCard = Database.GetCardFromId(cardId);
                                            var premium = Card.Attributes["premium"]?.Value;

                                            HistoryCards.Add(new Entity.Card(HDTCard, premium == "premium"));
                                        }
                                        else
                                        {
                                            return new History();
                                        }
                                    }

                                    History.Add(new Pack(packId, Time, HistoryCards));
                                }
                                else
                                {
                                    return new History();
                                }
                            }
                            else
                            {
                                return new History();
                            }
                        }
                    }
                }
            }

            return History;
        }

        public void Store(History History)
        {
            var Xml = new XmlDocument();
            Xml.AppendChild(Xml.CreateXmlDeclaration("1.0", "UTF-8", null));

            XmlNode Root = Xml.CreateElement("history");
            Xml.AppendChild(Root);

            foreach (var Pack in History)
            {
                XmlNode PackNode = Xml.CreateElement("pack");
                Root.AppendChild(PackNode);

                var Time = Xml.CreateAttribute("time");
                Time.Value = Pack.Time.Ticks.ToString();
                PackNode.Attributes.Append(Time);

                var PackId = Xml.CreateAttribute("id");
                PackId.Value = Pack.Id.ToString();
                PackNode.Attributes.Append(PackId);

                foreach (var Card in Pack.Cards)
                {
                    XmlNode CardNode = Xml.CreateElement("card");
                    PackNode.AppendChild(CardNode);

                    var CardId = Xml.CreateAttribute("id");
                    CardId.Value = Card.HDTCard.Id;
                    CardNode.Attributes.Append(CardId);

                    if (Card.Premium)
                    {
                        var Premium = Xml.CreateAttribute("premium");
                        Premium.Value = "premium";
                        CardNode.Attributes.Append(Premium);
                    }
                }
            }

            var path = Path.Combine(Config.AppDataPath, "PackTracker");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, "History.xml");

            Xml.Save(path);
        }
    }
}
