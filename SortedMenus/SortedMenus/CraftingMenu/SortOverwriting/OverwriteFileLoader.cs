using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.Serialization;

namespace SortedMenus
{
    internal class OverwriteFileLoader
    {
        public static string[] supportedEmbeddedLanguages = new[] { "English" };

        private const string embeddedPathFormat = "SortedMenus.Overwrites.{0}.{1}.json";

        private const string nameOverwriteInfix = "SortedMenus.NameOverwrites";
        private const string sortOverwriteInfix = "SortedMenus.SortOverwrites";

        private const string loadingLog = "Loading {0} overwrite file '{1}' for language '{2}'";
        private const string failedLoadLog = "Failed loading {0} overwrite file '{1}' for language '{2}'";
        private const string external = "external";
        private const string embedded = "embedded";

        internal static void LoadOverwrites()
        {
            var nameOverwriteDict = LoadOverwriteFiles(nameOverwriteInfix);

            if (nameOverwriteDict != null)
            {
                ValidateNameOverwrites(nameOverwriteDict);
                OverwritePatches.importedNameOverwrites = nameOverwriteDict;
            }

            var sortOverwriteDict = LoadOverwriteFiles(sortOverwriteInfix);

            if (sortOverwriteDict != null)
            {
                OverwritePatches.importedSortOverwrites = sortOverwriteDict;
            }
        }

        internal static Dictionary<string, string> LoadOverwriteFiles(string infix)
        {
            var currentLanguage = Localization.instance.GetSelectedLanguage();

            var nameOverwriteFilesFound = Directory.GetFiles(Path.GetDirectoryName(Paths.PluginPath), $"{infix}.*.json", SearchOption.AllDirectories);

            foreach (var overwriteFilePath in nameOverwriteFilesFound)
            {
                var languageKey = Path.GetFileNameWithoutExtension(overwriteFilePath).Split('.')[2];

                if (languageKey == currentLanguage)
                {
                    Helper.Log(string.Format(loadingLog, external, infix, currentLanguage));

                    var dict = LoadExternalOverwriteFile(overwriteFilePath);

                    if (dict != null)
                    {
                        return dict;
                    }
                    else
                    {
                        Helper.LogWarning(string.Format(failedLoadLog, external, infix, currentLanguage));
                    }

                    break;
                }
            }

            // if we arrived here, then no external file successfully loaded

            if (supportedEmbeddedLanguages.Contains(currentLanguage))
            {
                Helper.Log(string.Format(loadingLog, embedded, infix, currentLanguage));

                var dict = LoadEmbeddedOverwriteFile(infix, currentLanguage);

                if (dict != null)
                {
                    return dict;
                }
                else
                {
                    Helper.LogWarning(string.Format(failedLoadLog, embedded, infix, currentLanguage));
                }
            }

            return null;
        }

        internal static void ValidateNameOverwrites(Dictionary<string, string> dict)
        {
            var pairs = dict.ToList();

            foreach (var pair in pairs)
            {
                // we can remove inside the loop, because we are iterating over a shallow copied list
                dict.Remove(pair.Key);

                if (pair.Key.IndexOf('$') == 0)
                {
                    string actualKey = pair.Key.Substring(1);

                    dict[actualKey] = pair.Value;

                    if (!Localization.instance.m_translations.ContainsKey(actualKey))
                    {
                        Helper.Log($"{pair.Key} may not be a valid translation key to overwrite (no matching translation found during late check)");
                    }
                }
                else
                {
                    Helper.Log($"{pair.Key} is not a valid translation key to overwrite (must start with $)");
                }
            }
        }

        internal static Dictionary<string, string> LoadExternalOverwriteFile(string path)
        {
            string translationAsString = File.ReadAllText(path);

            if (translationAsString == null)
            {
                return null;
            }

            return ParseStringToDictionary(translationAsString);
        }

        internal static Dictionary<string, string> LoadEmbeddedOverwriteFile(string infix, string language)
        {
            string translationAsString = ReadEmbeddedTextFile(string.Format(embeddedPathFormat, infix, language));

            if (translationAsString == null)
            {
                return null;
            }

            return ParseStringToDictionary(translationAsString);
        }

        internal static Dictionary<string, string> ParseStringToDictionary(string translationAsString)
        {
            Dictionary<string, string> parsedDict = new DeserializerBuilder().IgnoreFields().Build().Deserialize<Dictionary<string, string>>(translationAsString);

            if (parsedDict == null || parsedDict.Count == 0)
            {
                return null;
            }

            return parsedDict;
        }

        public static string ReadEmbeddedTextFile(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                return null;
            }

            using (MemoryStream memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);

                var bytes = memStream.Length > 0 ? memStream.ToArray() : null;

                return bytes != null ? Encoding.UTF8.GetString(bytes) : null;
            }
        }
    }
}