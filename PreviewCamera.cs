
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PreviewCamera : UdonSharpBehaviour
{
    [SerializeField]
    public GameObject CameraDisplay;
    [SerializeField]
    public GameObject PreviewCameraObject;
    [SerializeField]
    public Text CurrentUserText;
    [SerializeField]
    public SabageManager Sabage;
    [SerializeField]
    public Text ButtonText;
    private bool CurrentActiveStatus = false;
    [UdonSynced(UdonSyncMode.None)]
    public int TrackingUserID = -1;
    private int UpdateTextTimer = -1;
    void Start()
    {
        
    }
    public void Update()
    {
        if (CurrentActiveStatus)
        {
            if (TrackingUserID != -1)
            {
                var player = VRCPlayerApi.GetPlayerById(TrackingUserID);
                if (player != null)
                {
                    PreviewCameraObject.gameObject.GetComponent<Transform>().position = player.GetPosition();
                    PreviewCameraObject.gameObject.GetComponent<Transform>().rotation = player.GetRotation();
                }
                else if (Networking.IsOwner(this.gameObject))
                    PreviewNext();
            }
        }
        if (UpdateTextTimer != -1)
        {
            if (UpdateTextTimer > 0)
            {
                --UpdateTextTimer;
            }
            else
            {
                var player = VRCPlayerApi.GetPlayerById(TrackingUserID);
                if (player != null)
                    CurrentUserText.text = player.displayName;
            }
        }
    }
    public void PreviewNext()
    {
        if(Networking.IsOwner(this.gameObject))
        {
            var distX = 0f;
            var distZ = 0f;
            VRCPlayerApi player = null;
            Vector3 playerPos = new Vector3();
            Vector3 fieldPos = Sabage.GameField.GetComponent<Transform>().position;
            Vector3 fieldSize = Sabage.GameField.GetComponent<Transform>().localScale / 2f;
            var beforeID = -1;
            var firstID = -1;
            var id = -1;
            var trackingID = -1;
            for (int i = 0; i < Sabage.Player_IDs.Length; ++i)
            {
                id = Sabage.Player_IDs[i];
                if (id != -1)
                {
                    player = VRCPlayerApi.GetPlayerById(id);
                    playerPos = player.GetPosition();
                    distX = Mathf.Abs(playerPos.x - fieldPos.x);
                    distZ = Mathf.Abs(playerPos.z - fieldPos.z);
                    if (distX <= fieldSize.x && distZ <= fieldSize.z)
                    {
                        if (firstID == -1)
                            firstID = player.playerId;
                        if(beforeID == player.playerId)
                        {
                            trackingID = player.playerId;
                            break;
                        }
                        beforeID = player.playerId;
                    }
                }
            }
            if (trackingID != -1)
                TrackingUserID = trackingID;
            else if (firstID != -1)
                TrackingUserID = firstID;
            else
                TrackingUserID = Networking.LocalPlayer.playerId;
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UpdateText");
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "PreviewNext");
        }
    }
    public void UpdateText()
    {
        UpdateTextTimer = 30;
    }
    public void ToggleCamera()
    {
        CameraDisplay.gameObject.SetActive(!CameraDisplay.gameObject.activeSelf);
        CurrentActiveStatus = CameraDisplay.gameObject.activeSelf;
        if (CameraDisplay.gameObject.activeSelf)
            ButtonText.text = "Disable Camera";
        else
            ButtonText.text = "Enable Camera";
        if (CurrentActiveStatus)
        {
            if (TrackingUserID == -1)
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "PreviewNext");
        }
    }
}
