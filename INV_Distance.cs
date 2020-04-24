
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class INV_Distance : UdonSharpBehaviour
{
    [SerializeField]
    public Scrollbar ScrollBar_Src;
    [SerializeField]
    public Text Text_Dest;
    void Start()
    {
        
    }
    void Update()
    {
        if (Text_Dest.text != ScrollBar_Src.value.ToString("F1"))
            Text_Dest.text = ScrollBar_Src.value.ToString("F1");
    }
}
