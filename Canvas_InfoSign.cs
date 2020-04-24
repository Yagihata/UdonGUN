
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Canvas_InfoSign : UdonSharpBehaviour
{
    [SerializeField]
    public Toggle Toggle_RotateMode;
    [SerializeField]
    public Toggle Toggle_LeftHanded;
    [SerializeField]
    public InventoryBase Inventory;
    [SerializeField]
    public GameObject[] TogglePosObjects;
    void Start()
    {
        
    }

    public void ToggleRotateMode()
    {
        Inventory.UsePlayerRotate = Toggle_RotateMode.isOn;
    }
    public void ToggleLeftMode()
    {
        for(int i = 0; i < TogglePosObjects.Length; ++i)
        {
            var obj = TogglePosObjects[i];
            var pos = obj.GetComponent<Transform>().localPosition;
            obj.GetComponent<Transform>().localPosition = new Vector3(pos.x * -1f, pos.y, pos.z);
        }
    }
}
