using HarmonyLib;
using Nick;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Maranara.AllStars
{
    class Patch1
    {

        [HarmonyPatch(typeof(MenuSystem), "GetStagesPreload")]
        [HarmonyPostfix]
        static void GetStagesPostfix(MenuSystem __instance, StagesDataPreload __result)
        {
            __result = Maps.GetStageData();
        }

        [HarmonyPatch(typeof(Localization), "GetText", new Type[] { typeof(string) })]
        [HarmonyPostfix]
        static void GetTextPostfix(string id, ref string __result)
        {
            if (string.IsNullOrWhiteSpace(__result))
            {
                if (Maps.stageNames.ContainsKey(id))
                {
                    __result = Maps.stageNames[id];
                }
            }
        }

        [HarmonyPatch(typeof(GameMusicBank), "GetMusic", new Type[] { typeof(string) })]
        [HarmonyPostfix]
        static void GetMusicPostfix(string id, ref GameMusic __result)
        {
            if (__result == null)
            {
                if (Maps.stageMusics.ContainsKey(id))
                {
                    __result = Maps.stageMusics[id];
                }
            }
            //string script = (__result.script != null) ? __result.script.gameObject.name : "Null";
            //Debug.Log($"Resulted in:\nClip: {__result.clip.name}\nVolume: {__result.volume}\nScript: {script}, \nLoopData: {__result.loopTime},{__result.loopWhere}");
        }

        [HarmonyPatch(typeof(RenderVisualizer), "InitializeStages")]
        [HarmonyPrefix]
        static void InitializeStagesPrefix(RenderVisualizer __instance)
        {
            Transform stagesParent = (Transform)__instance.GetType().GetField("stagesParent", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
            GameObject clone = stagesParent.GetChild(0).gameObject;
            foreach (string map in Maps.maps)
            {
                if (stagesParent.Find(map))
                    continue;
                GameObject instance = GameObject.Instantiate(clone, clone.transform.position, clone.transform.rotation, stagesParent);
                instance.name = map;
                RenderImage image = instance.GetComponent<RenderImage>();
                image.StageMetaData = Maps.mapData[map];
            }
        }

        [HarmonyPatch(typeof(GameAgentStage), "PrepareStage")]
        [HarmonyPostfix]
        static void StagePass(GameAgentStage __instance)
        {
            if (!Maps.stageNames.ContainsKey(__instance.Agent.GameUniqueIdentifier))
            {
                return;
            }

            Renderer[] renderers = __instance.gameObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                Shader shader = Shader.Find(rend.sharedMaterial.shader.name);
                if (shader != null)
                {
                    rend.sharedMaterial.shader = shader;
                }
            }
        }
    }
}
