using UnityEngine;

namespace COM3D2.HighHeel.Core
{
    public class MaidTransforms
    {
        public readonly Transform Body;
        public readonly Transform FootL;
        public readonly Transform[] ToesL;
        public readonly Transform FootR;
        public readonly Transform[] ToesR;

        public MaidTransforms(TBody body)
        {
            Body = body.GetBone("Bip01");
            FootL = body.GetBone("Bip01 L Foot");
            FootR = body.GetBone("Bip01 R Foot");

            ToesL = new[]
            {
                CMT.SearchObjName(FootL, "Bip01 L Toe0"),
                CMT.SearchObjName(FootL, "Bip01 L Toe1"),
                CMT.SearchObjName(FootL, "Bip01 L Toe2"),
            };

            ToesR = new[]
            {
                CMT.SearchObjName(FootR, "Bip01 R Toe0"),
                CMT.SearchObjName(FootR, "Bip01 R Toe1"),
                CMT.SearchObjName(FootR, "Bip01 R Toe2"),
            };
        }

        public void Deconstruct(
            out Transform body, out Transform footL, out Transform[] toesL, out Transform footR, out Transform[] toesR
        )
        {
            body = Body;
            footL = FootL;
            toesL = ToesL;
            footR = FootR;
            toesR = ToesR;
        }
    }
}
