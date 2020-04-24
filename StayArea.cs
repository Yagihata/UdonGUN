
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class StayArea : UdonSharpBehaviour
{
    [SerializeField]
    public SabageManager GameManager;
    [SerializeField]
    public Text PlayerListText;
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
    void Update()
    {
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
                GameManager.InStayArea = true;
            }
        }
    }
    void OnTriggerExit(object collider)
    {
        if (collider != null)
        {
            if (collider.GetType().ToString() == "UnityEngine.CharacterController")
            {
                GameManager.InStayArea = false;
            }
        }
    }
}