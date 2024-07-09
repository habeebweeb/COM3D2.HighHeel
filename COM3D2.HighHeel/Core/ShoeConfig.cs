namespace COM3D2.HighHeel.Core;

public class ShoeConfig
{
    public enum ShoeConfigParameter
    {
        BodyOffset,
        FootLAngle,
        FootLMax,
        ToeLAngle,
        ToeL0Angle,
        ToeL01Angle,
        ToeL1Angle,
        ToeL11Angle,
        ToeL2Angle,
        ToeL21Angle,
        FootRAngle,
        FootRMax,
        ToeRAngle,
        ToeR0Angle,
        ToeR01Angle,
        ToeR1Angle,
        ToeR11Angle,
        ToeR2Angle,
        ToeR21Angle,
    }

    public float BodyOffset { get; set; }

    public float FootLAngle { get; set; }

    public float FootLMax { get; set; } = 55f;

    public float ToeLAngle { get; set; } = 280f;

    public float ToeL0Angle { get; set; }

    public float ToeL01Angle { get; set; }

    public float ToeL1Angle { get; set; }

    public float ToeL11Angle { get; set; }

    public float ToeL2Angle { get; set; }

    public float ToeL21Angle { get; set; }

    public float FootRAngle { get; set; }

    public float FootRMax { get; set; } = 55f;

    public float ToeRAngle { get; set; } = 280f;

    public float ToeR0Angle { get; set; }

    public float ToeR01Angle { get; set; }

    public float ToeR1Angle { get; set; }

    public float ToeR11Angle { get; set; }

    public float ToeR2Angle { get; set; }

    public float ToeR21Angle { get; set; }

    public void Deconstruct(
        out float bodyOffset,
        out float footLAngle,
        out float footLMax,
        out float toeLAngle,
        out float toeL0Angle,
        out float toeL01Angle,
        out float toeL1Angle,
        out float toeL11Angle,
        out float toeL2Angle,
        out float toeL21Angle,
        out float footRAngle,
        out float footRMax,
        out float toeRAngle,
        out float toeR0Angle,
        out float toeR01Angle,
        out float toeR1Angle,
        out float toeR11Angle,
        out float toeR2Angle,
        out float toeR21Angle)
    {
        bodyOffset = BodyOffset;
        footLAngle = FootLAngle;
        footLMax = FootLMax;
        toeLAngle = ToeLAngle;

        toeL0Angle = ToeL0Angle;
        toeL01Angle = ToeL01Angle;
        toeL1Angle = ToeL1Angle;
        toeL11Angle = ToeL11Angle;
        toeL2Angle = ToeL2Angle;
        toeL21Angle = ToeL21Angle;

        footRAngle = FootRAngle;
        footRMax = FootRMax;
        toeRAngle = ToeRAngle;

        toeR0Angle = ToeR0Angle;
        toeR01Angle = ToeR01Angle;
        toeR1Angle = ToeR1Angle;
        toeR11Angle = ToeR11Angle;
        toeR2Angle = ToeR2Angle;
        toeR21Angle = ToeR21Angle;
    }
}
