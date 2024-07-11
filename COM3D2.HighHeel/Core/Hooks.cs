using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using COM3D2.HighHeel.UI;

namespace COM3D2.HighHeel.Core
{
    public static class Hooks
    {
        private static readonly float[] ToeX = { 0f, 0f, 0f, 0f, 0f, 0f };
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


            //var configName = name.Substring(configNameIndex);
            var configName = name.Substring(configNameIndex, 9);

            if (!Plugin.Instance.ShoeDatabase.ContainsKey(configName))
            {
                Plugin.Instance.Logger.LogWarning($"Configuration '{configName}' could not be found!");
                return;
            }

            ShoeConfigs[__instance.body] = configName;

            Plugin.Instance.ImportConfigsAndUpdate(configName);
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

            //var (body, footL, toesL, footR, toesR) = transforms;
            var (body, footL, toesL, toeL0, toeL1, toeL2, footR, toesR, toeR0, toeR1, toeR2) = transforms;
            var (offset, 
                footLAngle, footLMax, toeLAngle, 
                toeL0Angle, toeL01Angle, toeL1Angle, toeL11Angle, toeL2Angle, toeL21Angle, 
                toeL0AngleX, toeL01AngleX, toeL1AngleX, toeL11AngleX, toeL2AngleX, toeL21AngleX, 
                toeL0AngleY, toeL01AngleY, toeL1AngleY, toeL11AngleY, toeL2AngleY, toeL21AngleY, 
                toeL0AngleZ, toeL01AngleZ, toeL1AngleZ, toeL11AngleZ, toeL2AngleZ, toeL21AngleZ, 
                footRAngle, footRMax, toeRAngle, 
                toeR0Angle, toeR01Angle, toeR1Angle, toeR11Angle, toeR2Angle, toeR21Angle,
                toeR0AngleX, toeR01AngleX, toeR1AngleX, toeR11AngleX, toeR2AngleX, toeR21AngleX,
                toeR0AngleY, toeR01AngleY, toeR1AngleY, toeR11AngleY, toeR2AngleY, toeR21AngleY,
                toeR0AngleZ, toeR01AngleZ, toeR1AngleZ, toeR11AngleZ, toeR2AngleZ, toeR21AngleZ) = config;

            body.Translate(Vector3.up * offset, Space.World);

            RotateFoot(footL, footLAngle, footLMax);
            RotateFoot(footR, footRAngle, footRMax);

            //RotateToes(toesL, toeLAngle, true);
            //RotateToes(toesR, toeRAngle, false);

            var IndividualAnglesToeL = new List<IndividualAngles>();
            IndividualAnglesToeL.Add(new IndividualAngles(toeL0AngleX, toeL0AngleY, toeL0AngleZ, "toeL0"));
            IndividualAnglesToeL.Add(new IndividualAngles(toeL01AngleX, toeL01AngleY, toeL01AngleZ, "toeL01"));
            IndividualAnglesToeL.Add(new IndividualAngles(toeL1AngleX, toeL1AngleY, toeL1AngleZ, "toeL1"));
            IndividualAnglesToeL.Add(new IndividualAngles(toeL11AngleX, toeL11AngleY, toeL11AngleZ, "toeL11"));
            IndividualAnglesToeL.Add(new IndividualAngles(toeL2AngleX, toeL2AngleY, toeL2AngleZ, "toeL2"));
            IndividualAnglesToeL.Add(new IndividualAngles(toeL21AngleX, toeL21AngleY, toeL21AngleZ, "toeL21"));
            RotateToesIndividual(toesL, toeLAngle, IndividualAnglesToeL, true);
            //RotateToesIndividual(toesL, toeLAngle, 
            //    new List<IndividualAngles>() {
            //        new IndividualAngles(toeL0AngleX, toeL0AngleY, toeL0AngleZ, "toeL0") { },
            //        new IndividualAngles(toeL01AngleX, toeL01AngleY, toeL01AngleZ, "toeL01") { },
            //        new IndividualAngles(toeL1AngleX, toeL1AngleY, toeL1AngleZ, "toeL1") { },
            //        new IndividualAngles(toeL11AngleX, toeL11AngleY, toeL11AngleZ, "toeL11") { },
            //        new IndividualAngles(toeL2AngleX, toeL2AngleY, toeL2AngleZ, "toeL2") { },
            //        new IndividualAngles(toeL21AngleX, toeL21AngleY, toeL21AngleZ, "toeL21") { }
            //        //new IndividualAngles() { x = toeL0AngleX,   y = toeL0AngleY,    z = toeL0AngleZ },
            //        //new IndividualAngles() { x = toeL01AngleX,  y = toeL01AngleY,   z = toeL01AngleZ },
            //        //new IndividualAngles() { x = toeL1AngleX,   y = toeL1AngleY,    z = toeL1AngleZ },
            //        //new IndividualAngles() { x = toeL11AngleX,  y = toeL11AngleY,   z = toeL11AngleZ },
            //        //new IndividualAngles() { x = toeL2AngleX,   y = toeL2AngleY,    z = toeL2AngleZ },
            //        //new IndividualAngles() { x = toeL21AngleX,  y = toeL21AngleY,   z = toeL21AngleZ }
            //    }, true);
            var IndividualAnglesToeR = new List<IndividualAngles>();
            IndividualAnglesToeR.Add(new IndividualAngles(toeR0AngleX, toeR0AngleY, toeR0AngleZ, "toeR0"));
            IndividualAnglesToeR.Add(new IndividualAngles(toeR01AngleX, toeR01AngleY, toeR01AngleZ, "toeR01"));
            IndividualAnglesToeR.Add(new IndividualAngles(toeR1AngleX, toeR1AngleY, toeR1AngleZ, "toeR1"));
            IndividualAnglesToeR.Add(new IndividualAngles(toeR11AngleX, toeR11AngleY, toeR11AngleZ, "toeR11"));
            IndividualAnglesToeR.Add(new IndividualAngles(toeR2AngleX, toeR2AngleY, toeR2AngleZ, "toeR2"));
            IndividualAnglesToeR.Add(new IndividualAngles(toeR21AngleX, toeR21AngleY, toeR21AngleZ, "toeL21"));
            RotateToesIndividual(toesR, toeRAngle, IndividualAnglesToeR, false);
            //RotateToesIndividual(toesR, toeRAngle,
            //    new List<IndividualAngles>() {
            //        new IndividualAngles(toeR0AngleX, toeR0AngleY, toeR0AngleZ, "toeR0") { },
            //        new IndividualAngles(toeR01AngleX, toeR01AngleY, toeR01AngleZ, "toeR01") { },
            //        new IndividualAngles(toeR1AngleX, toeR1AngleY, toeR1AngleZ, "toeR1") { },
            //        new IndividualAngles(toeR11AngleX, toeR11AngleY, toeR11AngleZ, "toeR11") { },
            //        new IndividualAngles(toeR2AngleX, toeR2AngleY, toeR2AngleZ, "toeR2") { },
            //        new IndividualAngles(toeR21AngleX, toeR21AngleY, toeR21AngleZ, "toeR21") { }
            //        //new IndividualAngles() { x = toeR0AngleX,   y = toeR0AngleY,    z = toeR0AngleZ, },
            //        //new IndividualAngles() { x = toeR01AngleX,  y = toeR01AngleY,   z = toeR01AngleZ },
            //        //new IndividualAngles() { x = toeR1AngleX,   y = toeR1AngleY,    z = toeR1AngleZ },
            //        //new IndividualAngles() { x = toeR11AngleX,  y = toeR11AngleY,   z = toeR11AngleZ },
            //        //new IndividualAngles() { x = toeR2AngleX,   y = toeR2AngleY,    z = toeR2AngleZ },
            //        //new IndividualAngles() { x = toeR21AngleX,  y = toeR21AngleY,   z = toeR21AngleZ }
            //    }, false);

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

            static void RotateToesIndividual(IList<Transform> toes, float correctionAngle, List<IndividualAngles> individualAngles, bool left)
            {
                var inverse = left ? 1f : -1f;

                for (var i = 0; i < 6; i++)
                {
                    //var thisToeAngles = individualAngles[i];
                    //var rotation = toes[i].localRotation.eulerAngles;
                    //var x = rotation.x;
                    //var y = rotation.y;
                    //var z = rotation.z;
                    //rotation.x = x + thisToeAngles.x;
                    //rotation.y  = y + thisToeAngles.y;
                    //rotation.z  = z + thisToeAngles.z;
                    //toes[i].localRotation = Quaternion.Euler(rotation);

                    var thisToeAngles = individualAngles[i];
                    var rotation = toes[i].localRotation.eulerAngles;
                    rotation.x = thisToeAngles.x;
                    rotation.y = thisToeAngles.y;
                    rotation.z = thisToeAngles.z;
                    toes[i].localRotation = Quaternion.Euler(rotation);
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), nameof(TBody.LoadBody_R))]
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

    public class IndividualAngles
    { 
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public IndividualAngles(float xJson, float yJson, float zJson, string boneName)
        {
            switch (boneName)
            {
                case "toeL0":
                    x = xJson + 359.783478f;
                    y = yJson + 0.489697933f;
                    z = zJson + 280.0905f;
                    break;
                case "toeL01":
                    x = xJson + -0.000153921559f;
                    y = yJson + 0.0000699606f;
                    z = zJson + 9.406873f;
                    break;
                case "toeL1":
                    x = xJson + 354.6391f;
                    y = yJson + 2.3375845f;
                    z = zJson + 283.451019f;
                    break;
                case "toeL11":
                    x = xJson + -0.00006798167f;
                    y = yJson + -0.00009544926f;
                    z = zJson + 0.0000599776577f;
                    break;
                case "toeL2":
                    x = xJson + 3.581009f;
                    y = yJson + 358.882568f;
                    z = zJson + 286.8532f;
                    break;
                case "toeL21":
                    x = xJson + 0.0000686013955f;
                    y = yJson + -0.0000236358665f;
                    z = zJson + 3.88611221f;
                    break;

                case "toeR0":
                    x = xJson + 0.216394484f;
                    y = yJson + 359.510223f;
                    z = zJson + 280.0905f;
                    break;
                case "toeR01":
                    x = xJson + 0.0000808577752f;
                    y = yJson + 0.0000769748149f;
                    z = zJson + 9.406906f;
                    break;
                case "toeR1":
                    x = xJson + 5.36096144f;
                    y = yJson + 357.662262f;
                    z = zJson + 283.45108f;
                    break;
                case "toeR11":
                    x = xJson + -0.000161576667f;
                    y = yJson + 0.00000360192917f;
                    z = zJson + -0.0000507995355f;
                    break;
                case "toeR2":
                    x = xJson + 356.4189f;
                    y = yJson + 1.11770535f;
                    z = zJson + 286.8531f;
                    break;
                case "toeR21":
                    x = xJson + 0.00007788669f;
                    y = yJson + 0.0000164253543f;
                    z = zJson + 3.886178f;
                    break;
            }
        }
    }
}
