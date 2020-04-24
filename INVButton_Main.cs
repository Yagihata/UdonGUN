
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class INVButton_Main : UdonSharpBehaviour
{
    [SerializeField]
    public InventoryBase InventoryBase_Object;
    void Start()
    {

    }
    public override void Interact()
    {
        if(InventoryBase_Object.MainGun != null)
        {
            if (InventoryBase_Object.SubGun != null)
                InventoryBase_Object.SubGun.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Deactivate");
            InventoryBase_Object.MainGun.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Activate");
            InventoryBase_Object.MainGun.SetProgramVariable("Interacted", "false");
            var obj = (UdonBehaviour)InventoryBase_Object.MainGun.gameObject.GetComponent(typeof(UdonBehaviour));
            InventoryBase_Object.MagazinPrefab = (GameObject)obj.GetProgramVariable("MagazinPrefab");
            InventoryBase_Object.UsingMainGun = true;
        }
    }
}
