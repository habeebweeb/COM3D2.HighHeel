using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.HighHeel.Core;

public static class HighHeelBodyOffset
{
    private static readonly Dictionary<TBody, float> BodyOffsets = new Dictionary<TBody, float>();

    public static float GetBodyOffset(TBody body)
    {
        if (BodyOffsets.TryGetValue(body, out float offset))
        {
            return offset;
        }

        return 0f;
    }

    public static float GetSnityouOutScale(TBody body)
    {
        if (BodyOffsets.TryGetValue(body, out float offset))
        {
            // SnityouOutScale = Thigh_SCL_.Scale ** 0.9
            float scale = (float)Math.Pow(Math.Pow(body.bonemorph.SnityouOutScale, 1 / 0.9) * (1 + offset / 2 / Vector3.Distance(body.Thigh_L.position, body.Calf_L.position)), 0.9);
            return scale;
        }

        return body.bonemorph.SnityouOutScale;
    }

    public static void SetBodyOffset(TBody body, float offset)
    {
        if (body != null)
        {
            BodyOffsets[body] = offset;
        }
    }

#pragma warning disable CS8604
    public static void Clean()
    {
        foreach (var key in BodyOffsets.Keys.ToArray())
        {
            if (key == null)
            {
                BodyOffsets.Remove(key);
            }
        }
    }
#pragma warning restore CS8604
}
