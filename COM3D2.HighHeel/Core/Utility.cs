using UnityEngine;

namespace COM3D2.HighHeel.Core
{
    public static class Utility
    {
        public static float ClampAngle(float angle, float min, float max)
        {
            var normalizedMin = Normalize180(min - angle);
            var normalizedMax = Normalize180(max - angle);

            if (normalizedMin <= 0f && normalizedMax >= 0f) return angle;

            return Mathf.Abs(normalizedMin) < Mathf.Abs(normalizedMax) ? min : max;
        }

        public static bool BetweenAngles(float angle, float min, float max) =>
            Normalize180(min - angle) <= 0f && Normalize180(max - angle) >= 0f;

        public static float Normalize180(float angle)
        {
            angle %= 360f;
            angle = (angle + 360f) % 360f;
            if (angle > 180f) angle -= 360f;

            return angle;
        }
    }
}
