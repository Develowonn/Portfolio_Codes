// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEngine.UI;

// # Project
using Photon.Pun;
using Photon.Realtime;

using EasyTransition;

using TMPro;

public class NewPhotonRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private NewPhotonLobbyManager lobbyManager;
    [SerializeField]
    private RoleSelector          roleSelector;
    [SerializeField]
    private TransitionSettings    transition;
    
    [Header("Panel")]
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private GameObject chatPanel;
    [SerializeField]
    private GameObject chat;

    [Header("Text")]
    [SerializeField]
    private TMP_Text roomCodeText;
    [SerializeField]
    private TMP_Text gameStartButtonText;

    [Header("Button")]
    [SerializeField]
    private Button gameStartButton;
    [SerializeField]
    private Button chatOpenButton;

    private new PhotonView photonView;

    private bool isGameReady = false;
    private bool isGameStart = false;
    private bool isChatting  = false;

    public GameObject ChatPanel  => chatPanel;
    public bool       IsChatting => isChatting;

    private void Start()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        photonView = GetComponent<PhotonView>();

        AddListener();
    }

    private void AddListener()
    {
        gameStartButton.onClick.AddListener(() => OnGameStartButton());
        chatOpenButton.onClick.AddListener(() => OnChatOpenButton());
    }

    private void Update()
    {
        if (roomPanel.activeSelf == false) return;

        if (chatPanel.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            ScaleEffect.Hide(chat.transform, chatPanel.transform);
            isChatting = false;
        }
        else if(!chatPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            ScaleEffect.Show(chat.transform, chatPanel.transform);
            isChatting = true;
        }

        if(PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= Constants.MaxPlayerInRoom && isGameReady)
            {
                gameStartButton.interactable = true;
                gameStartButtonText.color = new Color(
                   gameStartButtonText.color.r,
                   gameStartButtonText.color.g,
                   gameStartButtonText.color.b,
                   255.0f
               );
            }
            else
            {
                gameStartButton.interactable = false;
                gameStartButtonText.color = new Color(
                    gameStartButtonText.color.r,
                    gameStartButtonText.color.g,
                    gameStartButtonText.color.b,
                    125.0f
                );
            }
        }
    }

    public void SetUpRoomCode(string roomCode)
    {
        roomCodeText.text = string.Empty;
        roomCodeText.text = roomCode;
    }

    public void UpdateGameReady()
    {
        isGameReady = true;

        for(int i = 0; i < Constants.MaxPlayerInRoom; i++)
        {
            if(roleSelector.IsRoleUsage[i] == false)
            {
                isGameReady = false;
                return;
            }
        }
    }

    private void OnGameStartButton()
    {
        if (isGameStart) return;

        if(PhotonNetwork.IsMasterClient)
        {
            isGameStart = true;

            photonView.RPC(nameof(FadeInRpc), RpcTarget.All);
        }
    }

    [PunRPC]
    private void FadeInRpc()
    {
        TransitionManager.Instance().Transition(transition, 0);

        TransitionManager.Instance().onTransitionEnd += LoadLevel;
    }

    private void LoadLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("3. lnGame");
        }

        TransitionManager.Instance().onTransitionEnd -= LoadLevel;

    }

    private void OnChatOpenButton()
    {
        chatPanel.gameObject.SetActive(true);

        isChatting = true;
    }

    public override void OnJoinedRoom()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            gameStartButton.gameObject.SetActive(false);
        }

        PhotonChatManager.instance.Initialize();
    }

    public override void OnLeftRoom()
    {
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonChatManager.instance.ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, $"<color=#FF0000>{otherPlayer.NickName}¥‘¿Ã πÊ¿ª ≥™∞°ºÃΩ¿¥œ¥Ÿ.");

        int roleID = 0;

        if(otherPlayer.CustomProperties.TryGetValue("RoleID", out object roleIDObject))
        {
            roleID = (int)roleIDObject;
        }

        if(roleID != -1)
        {
            roleSelector.UpdateRole(roleID, false);
        }

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameStartButton.gameObject.SetActive(true);
        }
        else
        {
            gameStartButton.gameObject.SetActive(false);
        }
    }
}
