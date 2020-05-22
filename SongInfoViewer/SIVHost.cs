using HMUI;
using System;
using System.Linq;
using UnityEngine;
using IPA.Utilities;
using System.Globalization;
using SongDataCore.BeatStar;
using BeatSaberMarkupLanguage.Notify;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Data;
using System.Collections.Generic;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.Parser;

namespace SongInfoViewer
{
    public class SIVHost : INotifiableHost
    {
        private BeatStarDatabase _database;
        private LevelCollectionTableView _levelCollectionTableView;

        public event PropertyChangedEventHandler PropertyChanged;

        public static FieldAccessor<LevelCollectionTableView, TableView>.Accessor TableView = FieldAccessor<LevelCollectionTableView, TableView>.GetAccessor("_tableView");
        public static FieldAccessor<TableView, List<TableCell>>.Accessor VisibleCells = FieldAccessor<TableView, List<TableCell>>.GetAccessor("_visibleCells");
        public static FieldAccessor<LevelListTableCell, Image>.Accessor Background = FieldAccessor<LevelListTableCell, Image>.GetAccessor("_bgImage");

        [UIParams]
        public BSMLParserParams parserParams;

        private readonly Gradient _colorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.black, 0),
                new GradientColorKey(Color.red, .33f),
                new GradientColorKey(Color.yellow, .66f),
                new GradientColorKey(Color.green, 1f)
            }
        };

        public void SelectionChanged(IPreviewBeatmapLevel beatmap)
        {
            if (_levelCollectionTableView == null)
                _levelCollectionTableView = Resources.FindObjectsOfTypeAll<LevelCollectionTableView>().FirstOrDefault();
            if (_database == null)
                _database = SongDataCore.Plugin.Songs;
            var hash = beatmap == null ? "" : beatmap.levelID.Replace("custom_level_", "").ToLower();
            if (SongDataCore.Plugin.Songs.IsDataAvailable() && _database.Data.Songs.ContainsKey(hash))
            {
                var song = _database.Data.Songs[hash];
                MapActive = true;
                MapNotActive = false;

                Key = song.key;
                Rating = song.rating.ToString();
                Downloads = song.downloadCount.ToString();
                Upvotes = song.upVotes.ToString();
                Downvotes = song.downVotes.ToString();
            }
            else
            {
                MapActive = false;
                MapNotActive = true;
                parserParams.EmitEvent("close-desc");
            }
        }

        private bool _mapFound = false;
        [UIValue("map-active")]
        public bool MapActive
        {
            get => _mapFound;
            set
            {
                _mapFound = value;
                NotifyPropertyChanged();
            }
        }

        private bool _mapNotFound = true;
        [UIValue("map-not-active")]
        public bool MapNotActive
        {
            get => _mapNotFound;
            set
            {
                _mapNotFound = value;
                NotifyPropertyChanged();
            }
        }

        private string _key = "Key: ";
        [UIValue("key")]
        public string Key
        {
            get => _key;
            set
            {
                _key = "<b>Key:</b> " + value.StripTMPTags();
                NotifyPropertyChanged();
            }
        }

        private string _rating = "Rating: ";
        [UIValue("rating")]
        public string Rating
        {
            get => _rating;
            set
            {
                

                float val = float.Parse(value);
                _rating = $"<b>Rating:</b> <color=#{ColorUtility.ToHtmlStringRGB(_colorGradient.Evaluate(val))}>" + val.ToString("P", CultureInfo.InvariantCulture) + "</color>";
                NotifyPropertyChanged();
            }
        }

        private string _downloads = "Downloads: ";
        [UIValue("downloads")]
        public string Downloads
        {
            get => _downloads;
            set
            {
                _downloads = $"<b>Downloads: </b>" + value;
                NotifyPropertyChanged();
            }
        }

        private string _upvotes = "Upvotes: ";
        [UIValue("upvotes")]
        public string Upvotes
        {
            get => _upvotes;
            set
            {
                _upvotes = $"<b>Upvotes: </b>" + value;
                NotifyPropertyChanged();
            }
        }

        private string _downvotes = "Downvotes: ";
        [UIValue("downvotes")]
        public string Downvotes
        {
            get => _downvotes;
            set
            {
                _downvotes = $"<b>Downvotes: </b>" + value;
                NotifyPropertyChanged();
            }
        }

        private string _desc = "Description: ";
        [UIValue("desc")]
        public string Desc
        {
            get => _desc;
            set
            {
                _desc = value.StripTMPTags();
                NotifyPropertyChanged();
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"Error Invoking PropertyChanged: {ex.Message}");
                Plugin.Log.Error(ex);
            }
        }

        [UIAction("toggle")]
        public void Toggle()
        {
            var tableView = TableView(ref _levelCollectionTableView);
            var cells = VisibleCells(ref tableView);

            foreach (var cell in cells)
            {
                if (cell is LevelListTableCell)
                {
                    var c = cell as LevelListTableCell;
                    Background(ref c).enabled = true;

                    Plugin.Log.Info(Background(ref c).color.ToString());
                    Background(ref c).color = new Color(1f, 0f, 0f, .5f);
                }
            }
        }
    }


    static class StringExtensions
    {
        /*
        * Original code by @lolPants, put into an Extension class.
        * 
        * The second replace is not necessary, but is there for completeness. If you need to strip TMP tags regularly,
        * you can easily remove it for added performance.
        */
        public static string StripTMPTags(this string source) => source.Replace(@"<", "<\u200B").Replace(@">", "\u200B>");
    }
}
