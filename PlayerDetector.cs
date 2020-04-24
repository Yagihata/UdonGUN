
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PlayerDetector : UdonSharpBehaviour
{
    [SerializeField]
    public Text UserDisplayText;
    [UdonSynced(UdonSyncMode.None)]
    public string CurrentUserName = "";
    private string SetValue = "";
    private int SetStatus = -1;
    private bool UpdatedStatus = false;
    private string BeforeUserName = "";
    public int[] ConvertArray(string rawStr)
    {
        if (!string.IsNullOrEmpty(rawStr))
        {
            var src = rawStr.Split(',');
            var ret = new int[src.Length];
            for (int i = 0; i < ret.Length; ++i)
            {
                ret[i] = int.Parse(src[i]);
            }
            return ret;
        }
        else return new int[0];
    }
    public string JoinArray(char split, int[] arr)
    {
        string str = "";
        for (int i = 0; i < arr.Length; ++i)
            str += arr[i].ToString() + split;
        return str.TrimEnd(split);
    }
    public string RestoreArray(int[] rarArr)
    {
        var ret = JoinArray(',', rarArr);
        return ret;
    }

    void Start()
    {

    }
    public void SetUsingName()
    {
        UpdatedStatus = true;
    }
    void Update()
    {
        if (SetStatus > 30)
        {
            if (Networking.GetOwner(this.gameObject).playerId == Networking.LocalPlayer.playerId)
            {
                CurrentUserName = SetValue;
                SetStatus = -1;
                SendCustomNetworkEvent(NetworkEventTarget.All, "SetUsingName");
            }
            else
            {
                SetStatus = 10;
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

        }
        else if (SetStatus >= 0)
            ++SetStatus;
        if(UpdatedStatus)
        {
            if (CurrentUserName != BeforeUserName)
            {
                UpdatedStatus = false;
                UserDisplayText.text = "Current User:" + CurrentUserName;
            }
            BeforeUserName = CurrentUserName;
        }
    }
    public int ContainsArray(int[] arr, int obj)
    {
        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i] == obj)
                return i;
        }
        return -1;
    }
    void OnTriggerEnter(object collider)
    {
        if (collider != null)
        {
            if (collider.GetType().ToString() == "UnityEngine.CharacterController")
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                SetValue = Networking.LocalPlayer.displayName;
                SetStatus = 0;
            }
        }
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if(player.displayName == CurrentUserName)
        {
            UserDisplayText.text = "Current User:";
        }
    }
}
