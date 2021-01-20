namespace COM3D2.HighHeel
{
    public static class Utility
    {
        public static bool TryParseDefault(string value, float @default, out float result)
        {
            var success = float.TryParse(value, out result);
            result = success ? result : @default;

            return success;
        }
    }
}
