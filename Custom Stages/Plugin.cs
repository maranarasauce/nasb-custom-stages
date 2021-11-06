using BepInEx;
using HarmonyLib;
using Nick;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maranara.AllStars
{
    [BepInPlugin("maranara_custom_stages", "Maranara's Custom Stages", "0.0.2")]
    public class Plugin : BaseUnityPlugin
    {
        CharacterMetaData garfieldData;
        private void OnEnable()
        {
            CustomMapInit();

            var harmony = new Harmony("tester");
            harmony.PatchAll(typeof(Patch1));
        }

        void CustomMapInit()
        {
            string errorInfo = "";

            try
            {
                string stagePath = $"{Application.dataPath}/../Stages";

                errorInfo = "Getting directories";
                List<string> mapList = new List<string>();
                string[] dirs = Directory.GetDirectories(stagePath);
                foreach (string dir in dirs)
                {
                    mapList.Add(Path.GetFileName(dir));
                }
                Maps.maps = mapList.ToArray();

                errorInfo = "Finding game managers";
                GameMetaData[] gameMeta = Resources.FindObjectsOfTypeAll<GameMetaData>();
                AgentIdsToScene[] ids = Resources.FindObjectsOfTypeAll<AgentIdsToScene>();

                errorInfo = "Preparing to loop through maps";
                foreach (string map in Maps.maps)
                {
                    Logger.LogInfo($"Initializing {map}");
                    errorInfo = "Loading map asset bundles";
                    AssetBundle assetsBundle = AssetBundle.LoadFromFile($"{stagePath}/{map}/{map}.nick");
                    AssetBundle nickScene = AssetBundle.LoadFromFile($"{stagePath}/{map}/{map}.nickscene");
                    assetsBundle.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    nickScene.hideFlags = HideFlags.DontUnloadUnusedAsset;

                    errorInfo = "Loading stage asset";
                    StageMetaData stage = assetsBundle.LoadAsset<StageMetaData>($"Assets/Custom/stage_{map}/smd_stage_{map}.asset");
                    errorInfo = "Loading text asset";
                    TextAsset asset = assetsBundle.LoadAsset<TextAsset>($"Assets/Custom/stage_{map}/{map}_localName.txt");
                    string locName = asset.text;
                    errorInfo = "Adding text asset to list";
                    Maps.stageNames.Add("stage_" + map, locName);
                    errorInfo = "Editing stage asset";
                    stage.showId = "apple";

                    errorInfo = "Adding bundles and stages to static lists";
                    Maps.mapAssets.Add(map, assetsBundle);
                    Maps.mapData.Add(map, stage);

                    errorInfo = "Adding stages to menu";
                    AddStageToMenu(map, stage, gameMeta[0], ids[0]);
                    AddMusic(map, stage);
                }
            }
            catch
            {
                Logger.LogError("Error in loading Custom Map while: " + errorInfo);
            }


            void AddMusic(string mapId, StageMetaData stage)
            {
                GameMusic music = Maps.mapAssets[mapId].LoadAsset<GameMusic>($"Assets/Custom/stage_{mapId}/Resources/Music/{stage.musicIdDefault}.asset");

                if (!Maps.stageMusics.ContainsKey(stage.musicIdDefault))
                {
                    Maps.stageMusics.Add(stage.musicIdDefault, music);
                }
            }

            void AddStageToMenu(string mapName, StageMetaData stage, GameMetaData gameMeta, AgentIdsToScene agentIdsToScene)
            {
                List<StageMetaData> stages = new List<StageMetaData>();
                stages.AddRange(gameMeta.stageMetas);
                StageMetaData newStage = gameMeta.stageMetas[0];

                stage.hide = false;
                stage.showId = newStage.showId;
                stage.skins = newStage.skins;
                stage.stageNameAnnouncerId = newStage.stageNameAnnouncerId;
                stage.unlockedByUnlockIds = newStage.unlockedByUnlockIds;
                stage.resMediumPortrait = newStage.resMediumPortrait;
                stage.resMiniPortrait = newStage.resMiniPortrait;
                stage.resPortrait = newStage.resPortrait;
                stage.resPortraitGray = newStage.resPortraitGray;

                stages.Add(stage);

                gameMeta.stageMetas = stages.ToArray();

                agentIdsToScene.IdDict.Add($"stage_{mapName}", $"stage_{mapName}");
            }

        }
    }
}