using UnityEngine;

namespace COM3D2.HighHeel
{
    public class MaidTransforms
    {
        public readonly Transform Body;
        public readonly Transform FootL;
        public readonly Transform FootR;

        public MaidTransforms(TBody body)
        {
            Body = body.GetBone("Bip01");
            FootL = body.GetBone("Bip01 L Foot");
            FootR = body.GetBone("Bip01 R Foot");
        }

        public void Deconstruct(out Transform body, out Transform footL, out Transform footR)
        {
            body = Body;
            footL = FootL;
            footR = FootR;
        }
    }
}
