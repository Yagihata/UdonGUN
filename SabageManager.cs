
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class SabageManager : UdonSharpBehaviour
{
    [SerializeField]
    public GameObject[] SpawnPos;
    [SerializeField]
    public GameObject DeathPos;
    [SerializeField]
    public GameObject WorldSpawnPos;
    [SerializeField]
    public StayArea PlayerStayArea;
    [SerializeField]
    public PlayerHUDBase HUD;
    [SerializeField]
    public InventoryBase Inventory;
    [SerializeField]
    public LocalPlayerManager GamePlayer;
    [SerializeField]
    public GameObject StartButton;
    [SerializeField]
    public GameObject GameField;
    [SerializeField]
    public Text WinnerText;

    public int[] Player_IDs = null;
    private bool Player_IDs_Init = false;
    private int GameStartTime = -1;
    public bool InStayArea = false;

    [UdonSynced(UdonSyncMode.None)]
    public int LastStandingID = -1;
    private int SetLastStandingValue = -1;
    private int SetLastStandingStatus = -1;

    [UdonSynced(UdonSyncMode.None)]
    public int PosStartIndex = -1;
    private int SetPosStartIndexValue = -1;
    private int SetPosStartIndexStatus = -1;

    private bool NowGaming = false;

    private int UpdateWinnerTimer = -1;
    private int UpdateLivingStatusTimer = -1;
    public void DisableButton()
    {
        StartButton.SetActive(false);
    }

    void Update()
    {
        if(NowGaming)
        {
            if(GamePlayer.HP <= 0)
            {
                GamePlayer.FullHP();
                Death();
            }
        }
        if(GameStartTime > 0)
        {
            --GameStartTime;
        }
        else if(GameStartTime == 0)
        {
            GameStartTime = -1;
            SendCustomNetworkEvent(NetworkEventTarget.All, "JoinGame");
        }

        if (SetPosStartIndexStatus > 30)
        {
            if (Networking.GetOwner(this.gameObject).playerId == Networking.LocalPlayer.playerId)
            {
                PosStartIndex = SetPosStartIndexValue;
                SetPosStartIndexStatus = -1;
            }
            else
            {
                SetPosStartIndexStatus = 10;
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

        }
        else if (SetPosStartIndexStatus >= 0)
            ++SetPosStartIndexStatus;

        if (SetLastStandingStatus > 30)
        {
            if (Networking.GetOwner(this.gameObject).playerId == Networking.LocalPlayer.playerId)
            {
                LastStandingID = SetLastStandingValue;
                Debug.Log("UDGLOG - SENDLASTSTANDID:" + LastStandingID.ToString());
                SetLastStandingStatus = -1;
                SendCustomNetworkEvent(NetworkEventTarget.All, "EndGame");
            }
            else
            {
                SetLastStandingStatus = 10;
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }

        }
        else if (SetLastStandingStatus >= 0)
            ++SetLastStandingStatus;
        if (UpdateWinnerTimer >= 0)
        {
            if (UpdateWinnerTimer == 0)
            {
                var player = VRCPlayerApi.GetPlayerById(LastStandingID);
                if (player != null)
                    WinnerText.text = "WINNER\n\n" + player.displayName;
                UpdateWinnerTimer = -1;
            }
            else
                --UpdateWinnerTimer;
        }
        if (UpdateLivingStatusTimer >= 0)
        {
            if (UpdateLivingStatusTimer == 0)
            {
                var distX = 0f;
                var distZ = 0f;
                VRCPlayerApi livingLast = null;
                VRCPlayerApi player = null;
                Vector3 playerPos = new Vector3();
                Vector3 fieldPos = GameField.GetComponent<Transform>().position;
                Vector3 fieldSize = GameField.GetComponent<Transform>().localScale / 2f;
                Debug.Log("UDGLOG-FIELDPOS:" + fieldPos.ToString());
                Debug.Log("UDGLOG-FIELDSIZE:" + fieldSize.ToString());
                var id = -1;
                for (int i = 0; i < Player_IDs.Length; ++i)
                {
                    id = Player_IDs[i];
                    if (id != -1)
                    {
                        player = VRCPlayerApi.GetPlayerById(id);
                        playerPos = player.GetPosition();
                        distX = Mathf.Abs(playerPos.x - fieldPos.x);
                        distZ = Mathf.Abs(playerPos.z - fieldPos.z);
                        Debug.Log("UDGLOG-PLAYERPOS:" + playerPos.ToString());
                        Debug.Log("UDGLOG-DISTX:" + distX.ToString());
                        Debug.Log("UDGLOG-DISTZ:" + distZ.ToString());
                        if (distX <= fieldSize.x && distZ <= fieldSize.z)
                        {
                            Debug.Log("UDGLOG - LIVING:" + player.displayName);
                            if (livingLast != null)
                                return;
                            livingLast = player;
                        }
                    }
                }
                if (livingLast != null)
                {
                    Debug.Log("UDGLOG - TRYSENDID:" + livingLast.playerId.ToString());
                    SetLastStandingValue = livingLast.playerId;
                    SetLastStandingStatus = 0;
                }
                UpdateLivingStatusTimer = -1;
            }
            else
                --UpdateLivingStatusTimer;
        }
    }
    public void EndGame()
    {
        StartButton.SetActive(true);
        GamePlayer.FullHP();
        NowGaming = false;
        Networking.LocalPlayer.TeleportTo(WorldSpawnPos.GetComponent<Transform>().position, Networking.LocalPlayer.GetRotation());
        UpdateWinnerTimer = 30;
    }
    public void NotifyDeath()
    {
        if(Networking.IsOwner(gameObject))
        {
            UpdateLivingStatusTimer = 60;
            Debug.Log("UDGLOG-RECEIVE DEATH MENTION");
        }
    }
    public void BeginGame()
    {
        if (!NowGaming)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "DisableButton");
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            GameStartTime = 60 * 3;
            SetPosStartIndexValue = Random.Range(0, SpawnPos.Length - 1);
            SetPosStartIndexStatus = 0;
        }
    }
    public void JoinGame()
    {
        if (InStayArea)
        {
            if (Inventory.MainGun != null)
            {
                Inventory.MainGun.SetActivate(false);
            }
            if (Inventory.SubGun != null)
            {
                Inventory.SubGun.SetActivate(false);
            }
            var self_id = Networking.LocalPlayer.playerId;
            var inc = 0;
            for(int i = 0; i < Player_IDs.Length; ++i)
            {
                var id = Player_IDs[i];
                if (id == self_id)
                {
                    Networking.LocalPlayer.TeleportTo(SpawnPos[(PosStartIndex + inc) % SpawnPos.Length].GetComponent<Transform>().position, Networking.LocalPlayer.GetRotation());
                    break;
                }
                else if (id != -1)
                    ++inc;
            }
            GamePlayer.FullHP();
            NowGaming = true;
            HUD.gameObject.SetActive(true);
            Inventory.gameObject.SetActive(true);
            if (Inventory.MainGun != null)
            {
                Inventory.MainGun.RestoreMagazin();
                Inventory.MainGun.AmmoCocking();
            }
            if (Inventory.SubGun != null)
            {
                Inventory.SubGun.RestoreMagazin();
                Inventory.SubGun.AmmoCocking();
            }
        }
    }
    public void SortPlayerID()
    {
        int left, right; 
        int ptr = 0;
        int[] leftStack = new int[20];
        int[] rightStack = new int[20];
        leftStack[0] = 0;
        rightStack[0] = Player_IDs.Length - 1;
        ptr++;
        while (ptr-- > 0)
        {
            int pleft = left = leftStack[ptr];
            int pright = right = rightStack[ptr];
            int x = Player_IDs[(pleft + pright) / 2];
            int x1 = Player_IDs[pleft];
            int x2 = Player_IDs[pright];
            if ((x <= x1 && x1 <= x2) || (x2 <= x1 && x1 <= x)) x = x1;
            if ((x <= x2 && x2 <= x1) || (x1 <= x2 && x2 <= x)) x = x2;
            do
            {
                while (Player_IDs[pleft] < x) pleft++;
                while (Player_IDs[pright] > x) pright--;
                if (pleft <= pright)
                {
                    var v = Player_IDs[pleft];
                    Player_IDs[pleft] = Player_IDs[pright];
                    Player_IDs[pright] = v;
                    pleft++; pright--;
                }
            } while (pleft <= pright);
            if (left < pright)
            {
                leftStack[ptr] = left; rightStack[ptr] = pright;
                ptr++;
            }
            if (pleft < right)
            {
                leftStack[ptr] = pleft; rightStack[ptr] = right;
                ptr++;
            }
        }
    }
    public void Death()
    {
        NowGaming = false;
        Networking.LocalPlayer.TeleportTo(DeathPos.GetComponent<Transform>().position, Networking.LocalPlayer.GetRotation());
        SendCustomNetworkEvent(NetworkEventTarget.Owner, "NotifyDeath");
    }

    private void InitializeIdsIfNull()
    {
        if (!Player_IDs_Init)
        {
            Player_IDs = new int[80];
            for (int i = 0; i < Player_IDs.Length; i++)
            {
                Player_IDs[i] = -1;
            }
            Player_IDs_Init = true;
        }
    }
    public void RegisterPlayerID(int playerID)
    {
        InitializeIdsIfNull();

        for (int i = 0; i < Player_IDs.Length; i++)
        {
            if (Player_IDs[i] == -1)
            {
                Player_IDs[i] = playerID;
                break;
            }
        }
    }
    public void UnregisterPlayerID(int playerID)
    {
        InitializeIdsIfNull();

        for (int i = 0; i < Player_IDs.Length; i++)
        {
            if (Player_IDs[i] == playerID)
            {
                Player_IDs[i] = -1;
                break;
            }
        }
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RegisterPlayerID(player.playerId);
        SortPlayerID();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        UnregisterPlayerID(player.playerId);
        NotifyDeath();
        SortPlayerID();
    }
}
