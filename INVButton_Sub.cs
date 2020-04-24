
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class INVButton_Sub : UdonSharpBehaviour
{
    [SerializeField]
    public InventoryBase InventoryBase_Object;
    void Start()
    {
        
    }
    public override void Interact()
    {
        if (InventoryBase_Object.SubGun != null)
        {
            if (InventoryBase_Object.MainGun != null)
                InventoryBase_Object.MainGun.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Deactivate");
            InventoryBase_Object.SubGun.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activate");
            InventoryBase_Object.SubGun.SetProgramVariable("Interacted", "false");
            var obj = (UdonBehaviour)InventoryBase_Object.SubGun.gameObject.GetComponent(typeof(UdonBehaviour));
            InventoryBase_Object.MagazinPrefab = (GameObject)obj.GetProgramVariable("MagazinPrefab");
            InventoryBase_Object.UsingMainGun = false;
        }
    }
}
