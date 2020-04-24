
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StartButton : UdonSharpBehaviour
{
    [SerializeField]
    public SabageManager GameManager;
    void Start()
    {
        
    }

    public override void Interact()
    {
        GameManager.BeginGame();
    }
}
