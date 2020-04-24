
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class InventoryBase : UdonSharpBehaviour
{
    [SerializeField]
    public GameObject WeaponSpawner;
    [SerializeField]
    public GameObject MagazinSpawner;
    [SerializeField]
    public Scrollbar ScrollBar_Distance;
    [SerializeField]
    public Scrollbar ScrollBar_Height;
    [SerializeField]
    public INVButton_Main MainWeaponButton;
    [SerializeField]
    public INVButton_Sub SubWeaponButton;

    public UDONGun HoldingGun = null;
    public UdonBehaviour CurrentGunBehaviour = null;
    public GameObject MagazinPrefab = null;
    public GameObject SpawnedMagazin = null;
    public UDONGun MainGun = null;
    public UDONGun SubGun = null;
    public bool SetMagazinPos = false;
    public bool UsePlayerRotate = false;
    public bool UsingMainGun = true;
    void Start()
    {

    }
    void Update()
    {
        var player = Networking.LocalPlayer;
        var rot = player.GetBoneRotation(HumanBodyBones.Chest);
        if(!UsePlayerRotate)
        {
            var rawrot = rot.eulerAngles;
            rot = Quaternion.Euler(rawrot.x, player.GetRotation().eulerAngles.y, rawrot.z);
        }
        if (MainGun != null && MainGun.gameObject != null)
        {
            if (!MainGun.Interacted)
            {
                var pos = WeaponSpawner.GetComponent<Transform>().position;
                MainGun.gameObject.GetComponent<Transform>().position = pos;
                MainGun.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + (MainGun.FlipSpeed ? 180 : 0), rot.eulerAngles.z);
            }
        }
        if (SubGun != null && SubGun.gameObject != null)
        {
            if (!SubGun.Interacted)
            {
                var pos = WeaponSpawner.GetComponent<Transform>().position;
                SubGun.gameObject.GetComponent<Transform>().position = pos;
                SubGun.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + (SubGun.FlipSpeed ? 180 : 0), rot.eulerAngles.z);
            }
        }
        if(!SetMagazinPos && SpawnedMagazin != null)
        {
            var obj = (UdonBehaviour)SpawnedMagazin.gameObject.GetComponent(typeof(UdonBehaviour));
            if(obj != null)
            {
                obj.SetProgramVariable("LockPosObject", MagazinSpawner);
                object ret = obj.GetProgramVariable("LockPosObject");
                if (ret != null)
                    SetMagazinPos = true;
            }
        }

        var forward = rot * Vector3.forward;
        var basepos = player.GetBonePosition(HumanBodyBones.Chest);
        if(!UsePlayerRotate)
        {
            basepos.x = player.GetPosition().x;
            basepos.z = player.GetPosition().z;
        }
        this.gameObject.GetComponent<Transform>().position = basepos + (forward * ScrollBar_Distance.value) + new Vector3(0, ScrollBar_Height.value * 2f - 1f, 0);
        this.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);
        
        if(HoldingGun != null)
        {
            if (!HoldingGun.PlayerManager.IsVR)
            {
                if (HoldingGun.ReloadTimer == -1 && Input.GetKeyDown(KeyCode.R) && !HoldingGun.Reloading && HoldingGun.ShowAmmoCount)
                {
                    Networking.LocalPlayer.SetWalkSpeed(HoldingGun.WalkSpeed * HoldingGun.ReloadMpy);
                    Networking.LocalPlayer.SetRunSpeed(HoldingGun.WalkSpeed * HoldingGun.SprintMpy * HoldingGun.ReloadMpy);
                    HoldingGun.ReloadTimer = 60 * 4;
                    HoldingGun.DropMagazin();
                    HoldingGun.Reloading = true;
                    HoldingGun.RefreshGUI();
                }
                else if (HoldingGun.ReloadTimer == 0)
                {
                    Networking.LocalPlayer.SetWalkSpeed(HoldingGun.WalkSpeed);
                    Networking.LocalPlayer.SetRunSpeed(HoldingGun.WalkSpeed * HoldingGun.SprintMpy);
                    HoldingGun.SendCustomNetworkEvent(NetworkEventTarget.All, "RestoreMagazin");
                    HoldingGun.AmmoCocking();
                    HoldingGun.ReloadTimer = -1;
                }
                else if (HoldingGun.ReloadTimer > 0)
                    --HoldingGun.ReloadTimer;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && !UsingMainGun)
        {
            MainWeaponButton.Interact();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && UsingMainGun)
        {
            SubWeaponButton.Interact();
        }
    }
}
