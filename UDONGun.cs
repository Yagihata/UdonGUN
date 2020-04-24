
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class UDONGun : UdonSharpBehaviour
{
    [SerializeField]
    GameObject BulletPrefab;
    [SerializeField]
    GameObject MuzzleTip;
    [SerializeField]
    GameObject MuzzleFlashPrefab;
    [SerializeField]
    GameObject FireSoundPrefab;
    [SerializeField]
    GameObject CockingSoundPrefab;
    [SerializeField]
    public GameObject MagazinPrefab;
    [SerializeField]
    GameObject VisualizedMagazin;
    [SerializeField]
    GameObject ReloadButton;
    [SerializeField]
    GameObject DropMagButton;
    [SerializeField]
    public Text GunGUI;
    [SerializeField]
    public bool FlipSpeed = false;
    [SerializeField]
    public int AmmoMax = 10;
    [SerializeField]
    public int Damage = 1;
    public int AmmoNow = 10;
    [SerializeField]
    int FireRate = -1;
    [SerializeField]
    Vector3 MuzzleFlashScale;
    [SerializeField]
    public InventoryBase PlayerInventory = null;
    [SerializeField]
    public bool IsSideArm = false;
    [SerializeField]
    public LocalPlayerManager PlayerManager = null;
    [SerializeField]
    public float WalkSpeed = 4;

    public float BaseWalkSpeed = 4;
    public float SprintMpy = 1.75f;
    public float ReloadMpy = 0.5f;


    public bool ShowAmmoCount = false;
    bool Firing = false;
    int FireTimer = -1;
    GameObject MuzzleFlash = null;
    GameObject FireSound = null;
    GameObject CockingSound = null;
    public bool Reloading = false;
    public bool Interacted = false;
    public int ReloadTimer = -1;
    [UdonSynced(UdonSyncMode.None)]
    public Vector3 StartPosition = new Vector3();
    [UdonSynced(UdonSyncMode.None)]
    public Quaternion StartRotation = new Quaternion();
    [UdonSynced(UdonSyncMode.None)]
    public bool PosRegistered = false;
    private int LastUseTime = 0;
    private bool Equipping = false;
    private 
    void Start()
    {
        if (!PosRegistered && Networking.IsOwner(gameObject))
        {
            StartPosition = GetComponent<Transform>().position;
            StartRotation = GetComponent<Transform>().rotation;
            PosRegistered = true;
        }
        AmmoNow = AmmoMax;
        RefreshGUI(); 
    }
    public void AmmoReload()
    {
        AmmoNow = 0;
        SendCustomNetworkEvent(NetworkEventTarget.All, "RestoreMagazin");
        Reloading = true;
        RefreshGUI();
    }
    public void Activate()
    {
        this.gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
    public void Equip()
    {
        Equipping = true;
    }
    public void Unequip()
    {
        Equipping = false;
    }
    public void NotifyfyUse()
    {
        LastUseTime = PlayerManager.GameTime;
    }
    public void SetActivate(bool status)
    {
        if(status)
            SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
        else
            SendCustomNetworkEvent(NetworkEventTarget.All, "Deactivate");
    }
    public void AmmoCocking()
    {
        if (CockingSoundPrefab != null)
        {
            if (CockingSound == null)
            {
                CockingSound = VRCInstantiate(CockingSoundPrefab);
                CockingSound.GetComponent<Transform>().position = GetComponent<Transform>().position;
                CockingSound.GetComponent<AudioSource>().volume = 1;
                CockingSound.GetComponent<AudioSource>().Play();
            }
            else
            {
                CockingSound.GetComponent<Transform>().position = GetComponent<Transform>().position;
                CockingSound.GetComponent<AudioSource>().Play();
            }
        }
        AmmoNow = AmmoMax;
        Reloading = false;
        RefreshGUI();
        ReloadButton.SetActive(false);
        DropMagButton.SetActive(true);
    }

    public void RefreshGUI()
    {
        if (ShowAmmoCount && !Reloading && PlayerManager.IsVR)
            GunGUI.text = "AMMO:" + AmmoNow.ToString() + "/" + AmmoMax.ToString();
        else if (ShowAmmoCount && !Reloading && !PlayerManager.IsVR)
            GunGUI.text = "AMMO:" + AmmoNow.ToString() + "/" + AmmoMax.ToString() + " *";
        else if (ShowAmmoCount && Reloading)
            GunGUI.text = "RELOADING";
        else
            GunGUI.text = "";

    }
    void Update()
    {
        if(ShowAmmoCount)
        {
            if (Firing)
            {
                if (FireTimer == 0)
                {
                    if (AmmoNow > 0 && !Reloading)
                    {
                        SendCustomNetworkEvent(NetworkEventTarget.All, "Fire");
                        --AmmoNow;
                        RefreshGUI();
                    }
                    else
                    {
                        if (AmmoNow == 0 && !PlayerManager.IsVR && !Reloading && ReloadTimer == -1)
                        {
                            Networking.LocalPlayer.SetWalkSpeed(WalkSpeed * ReloadMpy);
                            Networking.LocalPlayer.SetRunSpeed(WalkSpeed * SprintMpy * ReloadMpy);
                            AmmoNow = 0;
                            ReloadTimer = 60 * 4;
                            DropMagazin();
                            Reloading = true;
                            RefreshGUI();
                        }
                        FireTimer = -1;
                        Firing = false;
                    }
                }
                FireTimer = (FireTimer + 1) % FireRate;
            }
        }
        if(Equipping)
            LastUseTime = PlayerManager.GameTime;
        else if (PlayerManager.GameTime % 600 == 0)
        {
            if(PlayerManager.GameTime - LastUseTime > 600 && Networking.IsOwner(gameObject))
            {
                Respawn();
            }
        }
    }
    public void DropMagazin()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "DropEmptyMagazin");
    }
    public void DropEmptyMagazin()
    {
        AmmoNow = 0;
        RefreshGUI();
        if (VisualizedMagazin != null)
        {
            VisualizedMagazin.SetActive(false);
            if (MagazinPrefab != null && PlayerManager.MagazinSpawnCooldown == -1)
            {
                var obj = VRCInstantiate(MagazinPrefab.gameObject);
                if (obj != null)
                {
                    obj.name = "DROPPED_Magazin";
                    obj.GetComponent<Transform>().position = VisualizedMagazin.GetComponent<Transform>().position;
                }
                PlayerManager.MagazinSpawnCooldown = 30;
            }
            if (DropMagButton != null)
                DropMagButton.SetActive(false);
        }
    }
    public void RestoreMagazin()
    {
        AmmoNow = AmmoMax;
        ReloadButton.SetActive(true);
        VisualizedMagazin.SetActive(true);
    }
    public void Fire()
    {
        var player = Networking.LocalPlayer;
        var obj = VRCInstantiate(BulletPrefab);
        Networking.SetOwner(Networking.GetOwner(this.gameObject), obj);
        obj.name = "SHOOTED_Bullet_" + Damage.ToString();
        if (MuzzleTip != null)
            obj.GetComponent<Transform>().position = MuzzleTip.GetComponent<Transform>().position;
        else
            obj.GetComponent<Transform>().position = GetComponent<Transform>().position;
        obj.GetComponent<Rigidbody>().velocity = 30f * GetComponent<Transform>().forward * (FlipSpeed ? -1f : 1f);
        obj.GetComponent<Transform>().rotation = GetComponent<Transform>().rotation;
        if (MuzzleFlashPrefab != null)
        {
            var flash = VRCInstantiate(MuzzleFlashPrefab);
            flash.GetComponent<Transform>().SetParent(gameObject.transform);
            flash.GetComponent<Transform>().localScale = MuzzleFlashScale;
            flash.GetComponent<Transform>().position = MuzzleTip.GetComponent<Transform>().position;
            flash.GetComponent<Transform>().rotation = MuzzleTip.GetComponent<Transform>().rotation;
        }
        if (FireSoundPrefab != null)
        {
            if (FireSound == null)
            {
                FireSound = VRCInstantiate(FireSoundPrefab);
                FireSound.GetComponent<Transform>().position = GetComponent<Transform>().position;
                FireSound.GetComponent<AudioSource>().volume = 1;
                FireSound.GetComponent<AudioSource>().Play();
            }
            else
            {
                FireSound.GetComponent<Transform>().position = GetComponent<Transform>().position;
                FireSound.GetComponent<AudioSource>().Play();
            }
        }
        if (AmmoNow == 0 && !Reloading && VisualizedMagazin.activeSelf)
        {
            DropMagazin();
        }
    }
    public override void OnPickupUseDown()
    {
        if (FireRate == -1)
        {
            if (AmmoNow > 0 && !Reloading)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "Fire");
                --AmmoNow;
                RefreshGUI();
            }
            else if (AmmoNow == 0 && !PlayerManager.IsVR && !Reloading && ReloadTimer == -1)
            {
                Networking.LocalPlayer.SetWalkSpeed(WalkSpeed * ReloadMpy);
                Networking.LocalPlayer.SetRunSpeed(WalkSpeed * SprintMpy * ReloadMpy);
                AmmoNow = 0;
                ReloadTimer = 60 * 4;
                DropMagazin();
                Reloading = true;
                RefreshGUI();
            }
        }
        else
        {
            if (AmmoNow > 0)
            {
                FireTimer = 0;
                Firing = true;
            }
        }
    }
    public override void OnPickupUseUp()
    {
        if (FireRate != -1)
        {
            FireTimer = -1;
            Firing = false;
        }
    }
    public void Respawn()
    {
        if (Networking.IsOwner(gameObject))
        {
            this.GetComponent<Transform>().position = StartPosition;
            this.GetComponent<Transform>().rotation = StartRotation;
            this.GetComponent<Rigidbody>().velocity = new Vector3();
            SendCustomNetworkEvent(NetworkEventTarget.All, "NotifyUse");
        }
        SetActivate(true);
    }
    public override void OnPickup()
    {
        Networking.LocalPlayer.SetWalkSpeed(WalkSpeed);
        Networking.LocalPlayer.SetRunSpeed(WalkSpeed * SprintMpy);
        SendCustomNetworkEvent(NetworkEventTarget.All, "Equip");
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PlayerManager.InitializeVRState();
        Interacted = true;
        if (PlayerInventory != null)
        {
            PlayerInventory.HoldingGun = this;
            if (IsSideArm)
            {
                PlayerInventory.SubGun = this;
            }
            else
            {
                PlayerInventory.MainGun = this;
            }
            PlayerInventory.MagazinPrefab = MagazinPrefab;
        }
        if (VisualizedMagazin.activeSelf)
            DropMagButton.SetActive(true);
        if(!PlayerManager.IsVR)
        {
            var rot = Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.Head);
            if (!PlayerInventory.UsePlayerRotate)
            {
                var rawrot = rot.eulerAngles;
                rot = Quaternion.Euler(rawrot.x, Networking.LocalPlayer.GetRotation().eulerAngles.y, rawrot.z);
            }
            this.gameObject.GetComponent<Transform>().rotation = Quaternion.Euler(-rot.eulerAngles.x, rot.eulerAngles.y + (FlipSpeed ? 180 : 0), rot.eulerAngles.z);
        }
        GetComponent<Rigidbody>().useGravity = true;
        ShowAmmoCount = true;
        RefreshGUI();
    }
    public override void OnDrop()
    {
        Networking.LocalPlayer.SetWalkSpeed(BaseWalkSpeed);
        Networking.LocalPlayer.SetRunSpeed(BaseWalkSpeed * SprintMpy);
        PlayerInventory.HoldingGun = null;
        SendCustomNetworkEvent(NetworkEventTarget.All, "Unequip");
        DropMagButton.SetActive(false);
        ShowAmmoCount = false;
        RefreshGUI();
    }
}
