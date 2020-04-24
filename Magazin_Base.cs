
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Magazin_Base : UdonSharpBehaviour
{
    public bool Enabled = false;
    public int CountNow = 0;
    public bool Interacted = false;
    public GameObject LockPosObject = null;
    void Start()
    {

    }
    void Update()
    {
        if ((name == "DROPPED_Magazin" || name == "RELMAG") && !Enabled)
            Enabled = true;
        if (Enabled)
        {
            if (CountNow >= 60 * 10)
            {
                Destroy(this.gameObject);
            }
            if(LockPosObject != null && !Interacted)
            {
                this.GetComponent<Transform>().position = LockPosObject.GetComponent<Transform>().position;
                this.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, 0);
            }
            ++CountNow;
        }
    }
    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Interacted = true;
    }
}
