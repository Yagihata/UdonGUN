
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class INVButton_Throw : UdonSharpBehaviour
{
    [SerializeField]
    public InventoryBase InventoryBase_Object;
    void Start()
    {

    }
    public override void Interact()
    {
        /*var pos = InventoryBase_Object.MagazinSpawner.GetComponent<Transform>().position;
        GameObject mag_prefab;
        if (InventoryBase_Object.UsingMainGun)
            mag_prefab = InventoryBase_Object.MainGunPrefab.MagazinPrefab;
        else
            mag_prefab = InventoryBase_Object.SubGunPrefab.MagazinPrefab;
        var mag = VRCInstantiate(mag_prefab);
        mag.GetComponent<Transform>().position = pos;*/
    }
}
