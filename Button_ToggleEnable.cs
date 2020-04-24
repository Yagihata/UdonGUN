
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Button_ToggleEnable : UdonSharpBehaviour
{
    [SerializeField]
    public InventoryBase Inventory;
    [SerializeField]
    public Text ButtonText;
    void Start()
    {
        
    }
    public void ToggleInventory()
    {
        if(Inventory != null)
        {
            Inventory.gameObject.SetActive(!Inventory.gameObject.activeSelf);
            if (Inventory.gameObject.activeSelf)
                ButtonText.text = "Disable Inventory";
            else
                ButtonText.text = "Enable Inventory";
        }
    }
}
