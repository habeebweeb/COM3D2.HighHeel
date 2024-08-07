using UnityEngine;

namespace COM3D2.HighHeel.Core;

public class ShoeTransforms(TBodySkin skin)
{
    public readonly Transform? ShoeL = CMT.SearchObjName(skin.obj_tr, "Bip01 L Foot", false);
    public readonly Transform? ShoeR = CMT.SearchObjName(skin.obj_tr, "Bip01 R Foot", false);
    public readonly Vector3 OriginalScale = skin.body.GetBone("Bip01 L Foot").localScale;

    public Vector3 Position = Vector3.zero;
    public Vector3 Rotation = Vector3.zero;
    public Vector3 LocalScale = Vector3.zero;
}
