using UnityEngine;

namespace COM3D2.HighHeel.Core
{
    public class ShoeTransforms
    {
        public readonly Transform? ShoeL;
        public readonly Transform? ShoeR;
        public readonly Vector3 OriginalScale;

        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 LocalScale = Vector3.zero;

        public ShoeTransforms(TBodySkin skin)
        {
            ShoeL = CMT.SearchObjName(skin.obj_tr, "Bip01 L Foot", false);
            ShoeR = CMT.SearchObjName(skin.obj_tr, "Bip01 R Foot", false);
            OriginalScale = skin.body.GetBone("Bip01 L Foot").localScale;
        }
    }
}
