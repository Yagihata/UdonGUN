
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DropMagButton : UdonSharpBehaviour
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
            GunBase.DropMagazin();
        }
    }
}
