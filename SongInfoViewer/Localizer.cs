using System.Collections.Generic;

namespace SongInfoViewer
{
    public static class Localizer
    {
        public enum Language
        {
            English
        }

        public static Language Lang = Language.English;

        public static Dictionary<string, string[]> LanguageDictionary = new Dictionary<string, string[]>()
        {

        };

        public static string Localize(this string textCode, params string[] format)
        {
            bool found = LanguageDictionary.TryGetValue(textCode, out string[] text);
            if (!found)
            {
                Plugin.Log.Warn($"Localization of {textCode} not found for {Lang}");
                return textCode;
            }

            return string.Format(text[(int)Lang], format);
        }
    }
}
