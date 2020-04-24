
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class LocalPlayerManager : UdonSharpBehaviour
{
    [SerializeField]
    public Text HPText;
    public int HP = 100;
    private bool Initialized = false;
    public bool IsVR = false;
    public int MagazinSpawnCooldown = -1;
    public int GameTime = 0;
    public void InitializeVRState()
    {
        if(!Initialized)
        {
            object val = Networking.LocalPlayer.IsUserInVR();
            if(val != null && val.GetType().ToString() == "System.Boolean")
            {
                IsVR = (bool)val;
                Initialized = true;
            }
        }
    }
    void Start()
    {
        
    }
    public void FullHP()
    {
        HP = 100;
    }
    public void Update()
    {
        var text = "HP:" + HP.ToString();
        if (HPText.text != text)
            HPText.text = text;
        if (MagazinSpawnCooldown >= 0)
            --MagazinSpawnCooldown;
        ++GameTime;
    }
}
