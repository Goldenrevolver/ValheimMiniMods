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

        private const string embeddedPathFormatNoLanguage = "SortedMenus.Overwrites.";

        private const string craftingStationSortingOverwriteSuffix = "SortedMenus.CraftingStationSortingOverwrites.json";

        private const string loadingStationLog = "Loading {0} crafting station sort overwrite file";
        private const string failedStationLoadLog = "Failed loading {0} crafting station sort overwrite file";

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

            var stationOverwriteDict = LoadStationSortOverwriteFile();

            if (stationOverwriteDict != null)
            {
                InventoryGuiPatch.craftingStationSortingOverwrites = stationOverwriteDict;
            }
        }

        internal static Dictionary<string, string> LoadStationSortOverwriteFile()
        {
            var stationSortOverwriteFilesFound = Directory.GetFiles(Path.GetDirectoryName(Paths.PluginPath), craftingStationSortingOverwriteSuffix, SearchOption.AllDirectories);

            foreach (var stationOverwriteFilePath in stationSortOverwriteFilesFound)
            {
                Helper.Log(string.Format(loadingStationLog, external));

                var dict = LoadExternalOverwriteFile(stationOverwriteFilePath);

                if (dict != null)
                {
                    return dict;
                }
                else
                {
                    Helper.LogWarning(string.Format(failedStationLoadLog, external));
                }
            }

            // if we arrived here, then no external file successfully loaded

            Helper.Log(string.Format(loadingStationLog, embedded));

            var path = embeddedPathFormatNoLanguage + craftingStationSortingOverwriteSuffix;

            var embeddedDict = LoadEmbeddedOverwriteFile(path);

            if (embeddedDict != null)
            {
                return embeddedDict;
            }
            else
            {
                Helper.LogWarning(string.Format(failedStationLoadLog, embedded));
            }

            return null;
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

                var path = string.Format(embeddedPathFormat, infix, currentLanguage);

                var dict = LoadEmbeddedOverwriteFile(path);

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

        internal static Dictionary<string, string> LoadEmbeddedOverwriteFile(string path)
        {
            string translationAsString = ReadEmbeddedTextFile(path);

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