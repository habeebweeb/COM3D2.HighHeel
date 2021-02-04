namespace COM3D2.HighHeel.Core
{
    public class ShoeConfig
    {
        public enum ShoeConfigParameter { BodyOffset, FootLAngle, FootLMax, ToeLAngle, FootRAngle, FootRMax, ToeRAngle }
        public float BodyOffset { get; set; }
        public float FootLAngle { get; set; }
        public float FootLMax { get; set; } = 55f;
        public float ToeLAngle { get; set; } = 280f;
        public float FootRAngle { get; set; }
        public float FootRMax { get; set; } = 55f;
        public float ToeRAngle { get; set; } = 280f;
    
        public void Deconstruct(
            out float bodyOffset, out float footLAngle, out float footLMax, out float toeLAngle, out float footRAngle,
            out float footRMax, out float toeRAngle
        )
        {
            bodyOffset = BodyOffset;
            footLAngle = FootLAngle;
            footLMax = FootLMax;
            toeLAngle = ToeLAngle;
            footRAngle = FootRAngle;
            footRMax = FootRMax;
            toeRAngle = ToeRAngle;
        }
    }
}
