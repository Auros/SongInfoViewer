using TMPro;
using HarmonyLib;
using UnityEngine;
using SongDataCore.BeatStar;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SongInfoViewer
{

    [HarmonyPatch(typeof(LevelListTableCell), "RefreshVisuals")]
    internal class LevelListTableCell_RefreshVisuals
    {
        private static BeatStarDatabase _database;
        internal static Dictionary<string, string> _levelIDDict = new Dictionary<string, string>();
        internal static Color _defaultHighlightColor = new Color(0f, .753f, 1f, .251f);

        private static readonly Gradient _colorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.black, 0),
                new GradientColorKey(Color.red, .33f),
                new GradientColorKey(Color.yellow, .66f),
                new GradientColorKey(Color.green, 1f)
            }
        };

        internal static void Prefix(ref Image ____highlightImage, ref TextMeshProUGUI ____songNameText, ref TextMeshProUGUI ____authorText)
        {
            if (SongDataCore.Plugin.Songs.IsDataAvailable())
            {
                if (_database == null)
                    _database = SongDataCore.Plugin.Songs;
                if (____highlightImage != null)
                    if (_levelIDDict.ContainsKey(____songNameText.text + ____authorText.text))
                    {
                        var id = _levelIDDict[____songNameText.text + ____authorText.text];
                        if (_database.Data.Songs.ContainsKey(id))
                        {
                            var map = _database.Data.Songs[id];
                            var color = _colorGradient.Evaluate(map.rating); //Color.Lerp(Color.red, Color.green, map.rating);
                            color.a = .5f;
                            ____highlightImage.color = color;
                        }
                        else
                        {
                            if (____highlightImage != null)
                                ____highlightImage.color = _defaultHighlightColor;
                        }
                    }
                    else
                    {
                        if (____highlightImage != null)
                            ____highlightImage.color = _defaultHighlightColor;
                    }
            }
        }
    }

    [HarmonyPatch(typeof(LevelListTableCell), "SetDataFromLevelAsync")]
    [HarmonyAfter("com.kyle1413.BeatSaber.SongCore")]
    internal class LevelListTableCell_SetDataFromLevelAsync
    {
        internal static void Postfix(IPreviewBeatmapLevel level, ref TextMeshProUGUI ____authorText)
        {
            var songName = string.Format("{0} <size=80%>{1}</size>", level.songName, level.songSubName);
            var songAuthor = ____authorText.text;
            
            if (!LevelListTableCell_RefreshVisuals._levelIDDict.ContainsKey(songName + songAuthor))
            {
                LevelListTableCell_RefreshVisuals._levelIDDict.Add(songName + songAuthor, level.levelID.Replace("custom_level_", "").ToLower());
            }
        }
    }
}