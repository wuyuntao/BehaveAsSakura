using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class I18n
    {
        private static string translationDirectory = "BehaveAsSakura/Translations";
        private static I18n instance = new I18n("en");

        private string language;
        private Dictionary<string, string> translations = new Dictionary<string, string>();

        private I18n(string language)
        {
            this.language = language;

            var path = string.Format("{0}/{1}", translationDirectory, language);
            var xml = Resources.Load(path) as TextAsset;
            if (xml != null)
            {
                using (var stream = new MemoryStream(xml.bytes))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        var rootName = reader.Name;
                        while (reader.Read())
                        {
                            if (reader.IsStartElement() && reader.Name == "Item")
                            {
                                var key = reader.GetAttribute("Key");
                                var value = reader.GetAttribute("Value");

                                if (translations.ContainsKey(key))
                                {
                                    Debug.LogWarningFormat("Ignore duplicate translation key '{0}'", key);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(value))
                                        value = key;

                                    translations.Add(key, value);
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == rootName)
                                break;
                        }
                    }
                }
            }
            else
            {
                throw new InvalidDataException(string.Format("Failed to load translation from '{0}'", path));
            }
        }

        private string Translate(string key)
        {
            string value;
            if (translations.TryGetValue(key, out value))
                return value;
            else
            {
                Debug.LogWarningFormat("Missing translation of key '{0}'", key);

                return key;
            }
        }

        private string Translate(string key, params object[] args)
        {
            string value;
            if (translations.TryGetValue(key, out value))
                return string.Format(value, args);
            else
            {
                Debug.LogWarningFormat("Missing translation of key '{0}'", key);

                return key;
            }
        }

        public static void SetTranslationDirectory(string directory)
        {
            if (directory != translationDirectory)
            {
                translationDirectory = directory;

                instance = new I18n(instance.language);
            }
        }

        public static void SetLanguage(string language)
        {
            if (instance.language != language)
            {
                try
                {
                    instance = new I18n(language);
                }
                catch (InvalidDataException ex)
                {
                    Debug.LogErrorFormat(ex.Message);
                }
            }
        }

        public static string _(string key)
        {
            return instance.Translate(key);
        }

        public static string _(string key, params object[] args)
        {
            return instance.Translate(key, args);
        }
    }
}
