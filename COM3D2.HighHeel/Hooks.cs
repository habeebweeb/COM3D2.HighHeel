using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace COM3D2.HighHeel
{
    public static class Hooks
    {
        private static readonly Dictionary<TBody, MaidTransforms> MaidTransforms = new();

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), "LateUpdate")]
        public static void LateUpdate(TBody __instance)
        {
            if (__instance.boMAN || !__instance.isLoadedBody) return;

            if (Plugin.Instance == null || !Plugin.Instance.Configuration.Enabled.Value) return;

            // return if maid shoes are off or they don't have any on
            if (!__instance.GetSlotVisible(TBody.SlotID.shoes)) return;

            // Maid will float and their feet will spin if there is no animation playing
            if (!__instance.GetAnimation().isPlaying) return;

            if (!MaidTransforms.TryGetValue(__instance, out var info)) return;
            if (!Plugin.Instance.Database.TryGetValue(__instance.maid.status.guid, out var config)) return;

            var (body, footL, footR) = info;
            var (bodyOffset, footLRotation, footRRotation) = config;

            body.Translate(Vector3.up * bodyOffset, Space.World);
            footL.Rotate(Vector3.forward, footLRotation);
            footR.Rotate(Vector3.forward, footRRotation);

            __instance.SkinMeshUpdate();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), nameof(TBody.LoadBody_R))]
        public static void OnLoadBody_R(TBody __instance)
        {
            if (__instance.boMAN) return;
            if (Plugin.Instance == null) return;

            var guid = __instance.maid.status.guid;
            if (!Plugin.Instance.Database.ContainsKey(guid)) Plugin.Instance.Database.Add(guid, new(__instance.maid));

            MaidTransforms[__instance] = new(__instance);
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
        }
    }
}
