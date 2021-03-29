using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace COM3D2.HighHeel.Core
{
    public static class Hooks
    {
        private static readonly float[] ToeX = { 15f, -5f, -5f };
        private static readonly Dictionary<TBody, MaidTransforms> MaidTransforms = new();
        private static readonly Dictionary<TBody, string> ShoeConfigs = new();

        [HarmonyPostfix]
        [HarmonyPatch(
            typeof(TBodySkin), nameof(TBodySkin.Load), typeof(MPN), typeof(Transform), typeof(Transform),
            typeof(Dictionary<string, Transform>), typeof(string), typeof(string), typeof(string), typeof(string),
            typeof(int), typeof(bool), typeof(int)
        )]
        public static void OnTBodySkinLoad(TBodySkin __instance)
        {
            if (__instance.SlotId != TBody.SlotID.shoes) return;
            if (Plugin.Instance == null) return;

            if (ShoeConfigs.ContainsKey(__instance.body))
            {
                Plugin.Instance.Logger.LogDebug(
                    $"{nameof(OnTBodySkinLoad)}: ShoeConfigs already contains {__instance.obj.name}. How?"
                );

                ShoeConfigs.Remove(__instance.body);
            }

            var name = __instance.obj.name;
            int configNameIndex;

            if ((configNameIndex = name.IndexOf("hhmod_", StringComparison.Ordinal)) < 0) return;

            var configName = name.Substring(configNameIndex);

            if (!Plugin.Instance.ShoeDatabase.ContainsKey(configName))
            {
                Plugin.Instance.Logger.LogWarning($"Configuration '{configName}' could not be found!");
                return;
            }

            ShoeConfigs[__instance.body] = configName;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), "LateUpdate")]
        public static void LateUpdate(TBody __instance)
        {
            if (__instance.boMAN || !__instance.isLoadedBody) return;

            if (Plugin.Instance == null || !Plugin.Instance.Configuration.Enabled.Value) return;

            // return if maid shoes are off or they don't have any on
            if (!__instance.GetSlotVisible(TBody.SlotID.shoes)) return;

            // Maid will float and their feet will spin if there is no animation playing
            if (!Plugin.IsDance && !__instance.GetAnimation().isPlaying) return;

            if (!MaidTransforms.TryGetValue(__instance, out var transforms)) return;

            ShoeConfig config;

            if (Plugin.Instance.EditMode) config = Plugin.Instance.EditModeConfig;
            else
            {
                if (!ShoeConfigs.TryGetValue(__instance, out var configName)) return;
                if (!Plugin.Instance.ShoeDatabase.TryGetValue(configName, out config)) return;
            }

            var (body, footL, toesL, footR, toesR) = transforms;
            var (offset, footLAngle, footLMax, toeLAngle, footRAngle, footRMax, toeRAngle) = config;

            body.Translate(Vector3.up * offset, Space.World);

            RotateFoot(footL, footLAngle, footLMax);
            RotateFoot(footR, footRAngle, footRMax);

            RotateToes(toesL, toeLAngle, true);
            RotateToes(toesR, toeRAngle, false);

            __instance.SkinMeshUpdate();

            static void RotateFoot(Transform foot, float angle, float max)
            {
                // 270 degrees represents a foot rotation where the toes point upwards
                const float minimumAngle = 270f;
                var rotation = foot.localRotation.eulerAngles;
                var z = rotation.z;

                if (!Utility.BetweenAngles(z, minimumAngle, max)) return;

                rotation.z = Utility.ClampAngle(z + angle, minimumAngle, max);

                foot.localRotation = Quaternion.Euler(rotation);
            }

            static void RotateToes(IList<Transform> toes, float angle, bool left)
            {
                var inverse = left ? 1f : -1f;

                for (var i = 0; i < 3; i++)
                {
                    var offset = 0f;

                    if (i != 1)
                    {
                        offset = angle switch
                        {
                            > 260f => 0f,
                            < 240f => 15f,
                            _ => 5f,
                        };
                    }

                    toes[i].localRotation = Quaternion.Euler(ToeX[i] * inverse, 0f, angle + offset);
                }
            }
        }

        [HarmonyPostfix,
         HarmonyPatch(typeof(TBody), nameof(TBody.LoadBody_R), typeof(string), typeof(Maid), typeof(int), typeof(bool))]
        public static void OnLoadBody_R(TBody __instance)
        {
            if (__instance.boMAN) return;
            if (Plugin.Instance == null) return;

            MaidTransforms[__instance] = new(__instance);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TBodySkin), nameof(TBodySkin.DeleteObj))]
        public static void OnTBodySkinDeleteObj(TBodySkin __instance)
        {
            if (__instance.SlotId != TBody.SlotID.shoes) return;
            if (Plugin.Instance == null) return;

            if (ShoeConfigs.ContainsKey(__instance.body)) ShoeConfigs.Remove(__instance.body);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Maid), nameof(Maid.Uninit))]
        public static void OnMaidUninit(Maid __instance)
        {
            if (!__instance.body0.isLoadedBody) return;

            OnMaidBodyDestroy(__instance.body0);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), "OnDestroy")]
        public static void OnMaidBodyDestroy(TBody __instance)
        {
            if (MaidTransforms.ContainsKey(__instance)) MaidTransforms.Remove(__instance);
            if (ShoeConfigs.ContainsKey(__instance)) ShoeConfigs.Remove(__instance);
        }
    }
}
