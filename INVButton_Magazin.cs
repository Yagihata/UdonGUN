
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class INVButton_Magazin : UdonSharpBehaviour
{
    [SerializeField]
    public InventoryBase InventoryBase_Object;
    void Start()
    {

    }

    public override void Interact()
    {
        if (InventoryBase_Object.MagazinPrefab != null)
        {
            var pos = InventoryBase_Object.MagazinSpawner.GetComponent<Transform>().position;
            var mag = VRCInstantiate(InventoryBase_Object.MagazinPrefab.gameObject);
            mag.GetComponent<Transform>().position = pos;
            mag.name = "RELMAG";
            InventoryBase_Object.SpawnedMagazin = mag;
            InventoryBase_Object.SetMagazinPos = false;
            Networking.SetOwner(Networking.LocalPlayer, mag);

        }
    }
}
