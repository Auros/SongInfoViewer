using System.Linq;
using UnityEngine;
using BS_Utils.Utilities;
using System.Collections;

namespace SongInfoViewer
{
    public class SongInfoViewer : MonoBehaviour
    {
        private SIVHost _host;
        private LevelFilteringNavigationController _levelFilteringNav;

        private void Awake()
        {
            BSEvents.levelSelected += LevelSelectionChanged;

            _levelFilteringNav = Resources.FindObjectsOfTypeAll<LevelFilteringNavigationController>().FirstOrDefault();
            _levelFilteringNav.didSelectAnnotatedBeatmapLevelCollectionEvent += LevelFilteringNav_didSelectAnnotatedBeatmapLevelCollectionEvent;
        }

        private void OnDestroy()
        {
            BSEvents.levelSelected -= LevelSelectionChanged;

            _levelFilteringNav.didSelectAnnotatedBeatmapLevelCollectionEvent -= LevelFilteringNav_didSelectAnnotatedBeatmapLevelCollectionEvent;
        }

        public void SetHost(SIVHost host)
            => _host = host;

        private void LevelSelectionChanged(object _, IPreviewBeatmapLevel beatmap)
            => _host.SelectionChanged(beatmap);

        private void LevelFilteringNav_didSelectAnnotatedBeatmapLevelCollectionEvent(object _, object __, object ___, object ____)
            => _host.SelectionChanged(null);
        
    }
}
