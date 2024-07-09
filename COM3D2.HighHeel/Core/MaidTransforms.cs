using UnityEngine;

namespace COM3D2.HighHeel.Core
{
    public class MaidTransforms
    {
        public readonly Transform Body;

        public readonly Transform FootL;
        public readonly Transform[] ToesL;

        public readonly Transform ToeL0;
        public readonly Transform ToeL1;
        public readonly Transform ToeL2;

        public readonly Transform FootR;
        public readonly Transform[] ToesR;

        public readonly Transform ToeR0;
        public readonly Transform ToeR1;
        public readonly Transform ToeR2;

        public MaidTransforms(TBody body)
        {
            Body = body.GetBone("Bip01");
            FootL = body.GetBone("Bip01 L Foot");
            FootR = body.GetBone("Bip01 R Foot");

            ToeL0 = body.GetBone("Bip01 L Toe0");
            ToeL1 = body.GetBone("Bip01 L Toe1");
            ToeL2 = body.GetBone("Bip01 L Toe2");

            ToeR0 = body.GetBone("Bip01 R Toe0");
            ToeR1 = body.GetBone("Bip01 R Toe1");
            ToeR2 = body.GetBone("Bip01 R Toe2");

            ToesL = new[]
            {
                CMT.SearchObjName(FootL, "Bip01 L Toe0"),
                CMT.SearchObjName(ToeL0, "Bip01 L Toe01"),
                CMT.SearchObjName(FootL, "Bip01 L Toe1"),
                CMT.SearchObjName(ToeL1, "Bip01 L Toe11"),
                CMT.SearchObjName(FootL, "Bip01 L Toe2"),
                CMT.SearchObjName(ToeL2, "Bip01 L Toe21"),
            };

            ToesR = new[]
            {
                CMT.SearchObjName(FootR, "Bip01 R Toe0"),
                CMT.SearchObjName(ToeR0, "Bip01 R Toe01"),
                CMT.SearchObjName(FootR, "Bip01 R Toe1"),
                CMT.SearchObjName(ToeR1, "Bip01 R Toe11"),
                CMT.SearchObjName(FootR, "Bip01 R Toe2"),
                CMT.SearchObjName(ToeR2, "Bip01 R Toe21"),
            };
        }

        public void Deconstruct(
            out Transform body, 
            out Transform footL, out Transform[] toesL, 
            out Transform toeL0, out Transform toeL1, out Transform toeL2,
            out Transform footR, out Transform[] toesR,
            out Transform toeR0, out Transform toeR1, out Transform toeR2
        )
        {
            body = Body;

            footL = FootL;
            toesL = ToesL;

            toeL0 = ToeL0;
            toeL1 = ToeL1;
            toeL2 = ToeL2;

            footR = FootR;
            toesR = ToesR;

            toeR0 = ToeR0;
            toeR1 = ToeR1;
            toeR2 = ToeR2;
        }
    }
}
