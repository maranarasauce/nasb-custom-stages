using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Maranara.AllStars
{
    [System.Serializable]
    public class RendererInfo : MonoBehaviour
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }
    
    [System.Serializable]
    public class LightInfo : MonoBehaviour
    {
        public Light light;
        public int lightmapBaketype;
        public int mixedLightingMode;
    }

    [ExecuteAlways]
    public class PrefabLightmapData : MonoBehaviour
    {
        public RendererInfo[] m_RendererInfo;
        public Texture2D[] m_Lightmaps;
        public Texture2D[] m_LightmapsDir;
        public Texture2D[] m_ShadowMasks;
        public LightInfo[] m_LightInfo;


        void Awake()
        {
            Init();
        }

        void Init()
        {
            Debug.Log("init");

            if (m_RendererInfo == null || m_RendererInfo.Length == 0)
                return;

            var lightmaps = LightmapSettings.lightmaps;
            int[] offsetsindexes = new int[m_Lightmaps.Length];
            int counttotal = lightmaps.Length;
            List<LightmapData> combinedLightmaps = new List<LightmapData>();

            for (int i = 0; i < m_Lightmaps.Length; i++)
            {
                bool exists = false;
                for (int j = 0; j < lightmaps.Length; j++)
                {

                    if (m_Lightmaps[i] == lightmaps[j].lightmapColor)
                    {
                        exists = true;
                        offsetsindexes[i] = j;

                    }

                }
                if (!exists)
                {
                    offsetsindexes[i] = counttotal;
                    var newlightmapdata = new LightmapData
                    {
                        lightmapColor = m_Lightmaps[i],
                        lightmapDir = m_LightmapsDir.Length == m_Lightmaps.Length ? m_LightmapsDir[i] : default(Texture2D),
                        shadowMask = m_ShadowMasks.Length == m_Lightmaps.Length ? m_ShadowMasks[i] : default(Texture2D),
                    };

                    combinedLightmaps.Add(newlightmapdata);

                    counttotal += 1;


                }

            }

            var combinedLightmaps2 = new LightmapData[counttotal];

            lightmaps.CopyTo(combinedLightmaps2, 0);
            combinedLightmaps.ToArray().CopyTo(combinedLightmaps2, lightmaps.Length);

            bool directional = true;

            foreach (Texture2D t in m_LightmapsDir)
            {
                if (t == null)
                {
                    directional = false;
                    break;
                }
            }

            LightmapSettings.lightmapsMode = (m_LightmapsDir.Length == m_Lightmaps.Length && directional) ? LightmapsMode.CombinedDirectional : LightmapsMode.NonDirectional;
            ApplyRendererInfo(m_RendererInfo, offsetsindexes, m_LightInfo);
            LightmapSettings.lightmaps = combinedLightmaps2;
        }

        void OnEnable()
        {

            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }

        // called when the game is terminated
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }



        static void ApplyRendererInfo(RendererInfo[] infos, int[] lightmapOffsetIndex, LightInfo[] lightsInfo)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];

                info.renderer.lightmapIndex = lightmapOffsetIndex[info.lightmapIndex];
                info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;

                // You have to release shaders.
                Material[] mat = info.renderer.sharedMaterials;
                for (int j = 0; j < mat.Length; j++)
                {
                    if (mat[j] != null && Shader.Find(mat[j].shader.name) != null)
                        mat[j].shader = Shader.Find(mat[j].shader.name);
                }

            }

            for (int i = 0; i < lightsInfo.Length; i++)
            {
                LightBakingOutput bakingOutput = new LightBakingOutput();
                bakingOutput.isBaked = true;
                bakingOutput.lightmapBakeType = (LightmapBakeType)lightsInfo[i].lightmapBaketype;
                bakingOutput.mixedLightingMode = (MixedLightingMode)lightsInfo[i].mixedLightingMode;

                lightsInfo[i].light.bakingOutput = bakingOutput;

            }


        }
    }

}
