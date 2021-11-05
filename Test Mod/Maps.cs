using BepInEx;
using Nick;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StagesUIMetaData;

namespace Maranara.AllStars
{
    public static class Maps
    {
        public static string[] maps;
        public static Dictionary<string, AssetBundle> mapAssets = new Dictionary<string, AssetBundle>();
        public static Dictionary<string, StageMetaData> mapData = new Dictionary<string, StageMetaData>();
        public static Dictionary<string, string> stageNames = new Dictionary<string, string>();
        public static Dictionary<string, GameMusic> stageMusics = new Dictionary<string, GameMusic>();

        public static StagesDataPreload StagesDataPreload;
        internal static void GenerateStageData()
        {
            if (StagesDataPreload == null)
            {
                StagesDataPreload = Resources.Load<StagesDataPreload>("StagesDataPreload");
                
                foreach (string map in maps)
                {
                    Debug.Log($"Assets/Custom/stage_{map}/Resources/Portraits/{map}_large");
                    AssetBundle bundle = mapAssets[map];
                    UnityEngine.Sprite largeSprite = bundle.LoadAsset<Sprite>($"Assets/Custom/stage_{map}/Resources/Portraits/{map}_large.png");
                    UnityEngine.Sprite mediumSprite = bundle.LoadAsset<Sprite>($"Assets/Custom/stage_{map}/Resources/Portraits/{map}_medium.png");
                    UnityEngine.Sprite smallSprite = bundle.LoadAsset<Sprite>($"Assets/Custom/stage_{map}/Resources/Portraits/{map}_small.png");
                    UnityEngine.Sprite thumbsSprite = bundle.LoadAsset<Sprite>($"Assets/Custom/stage_{map}/Resources/Portraits/{map}_stage_select_thumbnail.png");

                    StagesUIMetaData.StageUIElements element = new StageUIElements()
                    {
                        ID = $"stage_{map}",
                        StageLarge = largeSprite,
                        StageMedium = mediumSprite,
                        StageSmall = smallSprite,
                        StageSelectThumbnail = thumbsSprite
                    };

                    StagesDataPreload.StagesUIMetaData.StageUIElementsList.Add(element);
                }
            }
        }

        public static StagesDataPreload GetStageData()
        {
            if (StagesDataPreload == null)
            {
                GenerateStageData();
            }
            return StagesDataPreload;
        }

        public static readonly string[] replaceableShaders = new string[]
        {
            "Nick/NickGenericLightMaps",
            "NickGeneric",
            "NickGeneric Version2",
            "Nick/NickDome",
            "Nick/NickAdditive",
            "Nick/NickGoalPost",
            "Nick/NickResultsPlane",
            "Nick/NickShipSand",
            "Nick/NickSludge",
            "Nick/NickTube",
            "Nick/NickVegetationKelp",
            "NickCharacters",
            "Platform/NickPlatform",
            "NickVegetation",
            "NickVegetationJungle",
            "NickVegetationNoEmissive",
            "NickCharactersGhosts"
        };
    }
}
