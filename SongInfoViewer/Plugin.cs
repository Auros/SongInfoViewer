using IPA;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using BS_Utils.Utilities;
using IPALogger = IPA.Logging.Logger;
using BeatSaberMarkupLanguage.GameplaySetup;

namespace SongInfoViewer
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static Harmony Harmony { get; set; }
        internal static IPALogger Log { get; set; }

        internal SIVHost _host;

        

        [Init]
        public Plugin(IPALogger logger)
        {
            // Grab our instances from BSIPA
            Instance = this;
            Log = logger;
        }

        [OnEnable]
        public void OnEnabled()
        {
            Harmony = new Harmony("dev.auros.songinfoviewer");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

            BSEvents.lateMenuSceneLoadedFresh += OnLateMenuSceneLoadedFresh;
            _host = new SIVHost();
            
            GameplaySetup.instance.AddTab("SongInfoViewer", "SongInfoViewer.song-info-view.bsml", _host);
        }

        [OnDisable]
        public void OnDisabled()
        {
            Harmony?.UnpatchAll();
            Harmony = null;

            BSEvents.lateMenuSceneLoadedFresh -= OnLateMenuSceneLoadedFresh;
            _host = null;

            GameplaySetup.instance.RemoveTab("SongInfoViewer");
        }

        private void OnLateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO so)
        {
            var siv = new GameObject("Song Info Viewer").AddComponent<SongInfoViewer>();
            siv.SetHost(_host);
        }
    }
}
