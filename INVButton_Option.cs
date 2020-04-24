
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class INVButton_Option : UdonSharpBehaviour
{
    [SerializeField]
    public GameObject OptionWindow;
    void Start()
    {

    }
    public override void Interact()
    {
        OptionWindow.SetActive(!OptionWindow.activeSelf);
    }
}
