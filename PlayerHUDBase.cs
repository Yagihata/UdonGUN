
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerHUDBase : UdonSharpBehaviour
{
    [SerializeField]
    public Scrollbar ScrollBar_Distance;
    [SerializeField]
    public Scrollbar ScrollBar_Scale;
    [SerializeField]
    public Scrollbar ScrollBar_Height;
    [SerializeField]
    public LocalPlayerManager Player;
    void Start()
    {

    }
    void Update()
    {
        var player = Networking.LocalPlayer;
        var rot = player.GetBoneRotation(HumanBodyBones.Head);
        var forward = rot * Vector3.forward;
        var pos = player.GetBonePosition(HumanBodyBones.Head) + (forward * ScrollBar_Distance.value) + new Vector3(0, ScrollBar_Height.value - 0.5f, 0);
        if (!Player.IsVR)
            pos.y = player.GetPosition().y + 1.2f + (ScrollBar_Height.value - 0.5f);
        this.gameObject.GetComponent<Transform>().position = pos;
        this.gameObject.GetComponent<Transform>().rotation = rot;
        var scale = ScrollBar_Scale.value * 2f / 4.5f;
        this.gameObject.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);
    }
}
