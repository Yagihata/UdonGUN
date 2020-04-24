
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RespawnButton : UdonSharpBehaviour
{
    [SerializeField]
    public SabageManager GameManager;
    void Start()
    {

    }

    public override void Interact()
    {
        GameManager.SendCustomNetworkEvent(NetworkEventTarget.All, "EndGame");
    }
}
