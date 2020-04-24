
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MagazinSlot : UdonSharpBehaviour
{
    [SerializeField]
    public UDONGun GunBase;
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RELMAG")
        {
            GunBase.AmmoReload();
            Destroy(other.gameObject);
        }
    }
}
