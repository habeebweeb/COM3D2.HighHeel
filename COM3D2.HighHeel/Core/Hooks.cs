using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

namespace COM3D2.HighHeel.Core;

#pragma warning disable SA1313 // Harmony parameter names break this rule
public static class Hooks
{
    private static readonly float[] ToeX = [15f, 0f, -5f, 0f, -5f, 0f];

    private static readonly Dictionary<TBody, MaidTransforms> MaidTransforms = [];
    private static readonly Dictionary<TBody, string> ShoeConfigs = [];

    [HarmonyPostfix]
    [HarmonyPatch(
        typeof(TBodySkin),
        nameof(TBodySkin.Load),
        typeof(MPN),
        typeof(Transform),
        typeof(Transform),
        typeof(Dictionary<string, Transform>),
        typeof(string),
        typeof(string),
        typeof(string),
        typeof(string),
        typeof(int),
        typeof(bool),
        typeof(int))]
    public static void OnTBodySkinLoad(TBodySkin __instance)
    {
        if (__instance.SlotId != TBody.SlotID.shoes)
            return;

        if (Plugin.Instance == null)
            return;

        if (ShoeConfigs.ContainsKey(__instance.body))
        {
            Plugin.Instance.Logger.LogDebug(
                $"{nameof(OnTBodySkinLoad)}: ShoeConfigs already contains {__instance.obj.name}. How?");

            ShoeConfigs.Remove(__instance.body);
        }

        HighHeelBodyOffset.Clean();

        var name = __instance.obj.name;
        int configNameIndex;

        if ((configNameIndex = name.IndexOf("hhmod_", StringComparison.Ordinal)) < 0)
            return;

        var configName = name.Substring(configNameIndex);

        if (!Plugin.Instance.ShoeDatabase.ContainsKey(configName))
        {
            Plugin.Instance.Logger.LogWarning($"Configuration '{configName}' could not be found!");
            return;
        }

        ShoeConfigs[__instance.body] = configName;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TBody), "LateUpdate")]
    public static void LateUpdate(TBody __instance)
    {
        if (__instance.boMAN || !__instance.isLoadedBody)
            return;

        if (Plugin.Instance == null || !Plugin.Instance.Configuration.Enabled.Value)
            return;

        // return if maid shoes are off or they don't have any on
        if (!__instance.GetSlotVisible(TBody.SlotID.shoes))
            return;

        // Maid will float and their feet will spin if there is no animation playing
        if (!Plugin.IsDance && !__instance.GetAnimation().isPlaying)
            return;

        if (!MaidTransforms.TryGetValue(__instance, out var transforms))
            return;

        ShoeConfig config;

        if (Plugin.Instance.EditMode)
        {
            config = Plugin.Instance.EditModeConfig;
        }
        else
        {
            if (!ShoeConfigs.TryGetValue(__instance, out var configName))
                return;

            if (!Plugin.Instance.ShoeDatabase.TryGetValue(configName, out config))
                return;
        }

        var (_, footL, toesL, _, _, _, footR, toesR, _, _, _) = transforms;
        var (offset, footLAngle, footLMax, toeLAngle, toeL0Angle, toeL01Angle, toeL1Angle, toeL11Angle, toeL2Angle,
            toeL21Angle, footRAngle, footRMax, toeRAngle, toeR0Angle, toeR01Angle, toeR1Angle, toeR11Angle, toeR2Angle,
            toeR21Angle) = config;

        HighHeelBodyOffset.SetBodyOffset(__instance, offset);

        RotateFoot(footL, footLAngle, footLMax);
        RotateFoot(footR, footRAngle, footRMax);

        RotateToesIndividual(
            toesL, toeLAngle, [toeL0Angle, toeL01Angle, toeL1Angle, toeL11Angle, toeL2Angle, toeL21Angle], true);

        RotateToesIndividual(
            toesR, toeRAngle, [toeR0Angle, toeR01Angle, toeR1Angle, toeR11Angle, toeR2Angle, toeR21Angle], false);

        __instance.SkinMeshUpdate();

        static void RotateFoot(Transform foot, float angle, float max)
        {
            // 270 degrees represents a foot rotation where the toes point upwards
            const float minimumAngle = 270f;
            var rotation = foot.localRotation.eulerAngles;
            var z = rotation.z;

            if (!Utility.BetweenAngles(z, minimumAngle, max))
                return;

            rotation.z = Utility.ClampAngle(z + angle, minimumAngle, max);

            foot.localRotation = Quaternion.Euler(rotation);
        }

        static void RotateToesIndividual(IList<Transform> toes, float angle, List<float> individualAngle, bool left)
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

                toes[i].localRotation = Quaternion.Euler(ToeX[i] * inverse, 0f, angle + offset + individualAngle[i]);
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TBody), nameof(TBody.LoadBody_R), typeof(string), typeof(Maid))]
    public static void OnLoadBody_R(TBody __instance)
    {
        if (__instance.boMAN)
            return;

        if (Plugin.Instance == null)
            return;

        MaidTransforms[__instance] = new(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TBodySkin), nameof(TBodySkin.DeleteObj))]
    public static void OnTBodySkinDeleteObj(TBodySkin __instance)
    {
        if (__instance.SlotId != TBody.SlotID.shoes)
            return;

        if (Plugin.Instance == null)
            return;

        if (ShoeConfigs.ContainsKey(__instance.body))
            ShoeConfigs.Remove(__instance.body);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Maid), nameof(Maid.Uninit))]
    public static void OnMaidUninit(Maid __instance)
    {
        if (!__instance.body0.isLoadedBody)
            return;

        OnMaidBodyDestroy(__instance.body0);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TBody), "OnDestroy")]
    public static void OnMaidBodyDestroy(TBody __instance)
    {
        if (MaidTransforms.ContainsKey(__instance))
            MaidTransforms.Remove(__instance);

        if (ShoeConfigs.ContainsKey(__instance))
            ShoeConfigs.Remove(__instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Maid), "Update")]
    public static IEnumerable<CodeInstruction> MaidUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher codeMatcher = new CodeMatcher(instructions);
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Sub))
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Maid), "body0")))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HighHeelBodyOffset), nameof(HighHeelBodyOffset.GetBodyOffset))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Add));
        codeMatcher.MatchForward(false, new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Maid), "body0")))
            .Advance(1)
            .RemoveInstructionsWithOffsets(0, 1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HighHeelBodyOffset), nameof(HighHeelBodyOffset.GetSnityouOutScale))));
        return codeMatcher.InstructionEnumeration();
    }
}
#pragma warning restore
