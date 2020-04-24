
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReloadButton : UdonSharpBehaviour
{
    [SerializeField]
    public UDONGun GunBase = null;
    void Start()
    {

    }
    public override void Interact()
    {
        if (GunBase != null)
        {
            GunBase.AmmoCocking();
        }
    }
}
