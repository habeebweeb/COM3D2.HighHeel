namespace COM3D2.HighHeel
{
    public record MaidConfig
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public float BodyOffset { get; init; }
        public float FootLAngle { get; init; }
        public float FootRAngle { get; init; }

        public MaidConfig() { }

        public MaidConfig(Maid maid, float bodyOffset = 0f, float footLAngle = 0f, float footRAngle = 0f)
            : this(bodyOffset, footLAngle, footRAngle)
        {
            (_, FirstName, LastName) = maid.status;
        }

        private MaidConfig(float bodyOffset, float footLAngle, float footRAngle)
        {
            BodyOffset = bodyOffset;
            FootLAngle = footLAngle;
            FootRAngle = footRAngle;
        }
        public void Deconstruct(out float bodyOffset, out float footLAngle, out float footRAngle)
        {
            bodyOffset = BodyOffset;
            footLAngle = FootLAngle;
            footRAngle = FootRAngle;
        }
    }
}
