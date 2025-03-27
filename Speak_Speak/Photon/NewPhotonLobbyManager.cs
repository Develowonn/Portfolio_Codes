// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using UnityEngine.UI;

// # Project
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using EasyTransition;

public class NewPhotonLobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private NewPhotonRoomManager roomManager;
    [SerializeField]
    private TransitionSettings   circleWipe;
    [SerializeField]
    private TransitionSettings   circleWipeOut;

    [Header("Panel")]
    [SerializeField]
    private GameObject      lobbyPanel;
    [SerializeField]
    private GameObject[]    roomPanel;

    [Header("InputField")]
    [SerializeField]
    private TMP_InputField  roomCodeInputField;
    [SerializeField]
    private TMP_InputField  nickNameInputField;

    [Header("Text")]
    [SerializeField]
    private TMP_Text        loginButtonText;

    [Header("Button")]
    [SerializeField]
    private Button          roomLoginButton;

    private List<RoomInfo>  roomList          = new List<RoomInfo>();

    private bool            isReadyCreateRoom = true;
    public bool            isTransitionEnd   = true;

    private void Start()
    {
        isReadyCreateRoom = true;

        AddListener();

        SetLobbyPanelActive(true);
        SetRoomPanelActive(false);
    }

    private void Update()
    {
        if (roomCodeInputField.text == string.Empty)
        {
            loginButtonText.text = "Create";
        }
        else
        {
            loginButtonText.text = "Join";
        }
    }

    private void AddListener()
    {
        roomLoginButton.onClick.AddListener(() => OnRoomCreateOrJoinButton());
    }

    private void OnRoomCreateOrJoinButton()
    {
        if (!isReadyCreateRoom) return;

        Debug.Log("Ŭ��");

        if (nickNameInputField.text == string.Empty)
        {
            ErrorManager.Instance.InvokeErrorMessage("Nickname Null !!!!");
            return;
        }

        // �г��� ����
        PhotonNetwork.NickName = nickNameInputField.text;
        
        // �� �ڵ带 �Է����� �ʾ��� �� �����
        if(roomCodeInputField.text == string.Empty)
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = Constants.MaxPlayerInRoom,
                IsVisible  = true,
                IsOpen     = true,
            };

            string roomCode = RoomCode.GenerateRoomCode(Constants.RoomCodeLength);

            isTransitionEnd = false;

            TransitionManager.Instance().Transition(circleWipe, 0);
            TransitionManager.Instance().onTransitionCutPointReached += SetActiveRoomPanel;
            TransitionManager.Instance().onTransitionEnd += IsTranstionEnd;

            PhotonNetwork.CreateRoom(roomCode, roomOptions);

            isReadyCreateRoom = false;
        }
        // �� �ڵ带 �Է����� �� �� ���� 
        else
        {
            string roomCode = roomCodeInputField.text;

            PhotonNetwork.JoinRoom(roomCode);

            isReadyCreateRoom = false;
        }

        StartCoroutine(ReadyCreateRoom());
    }

    public IEnumerator ReadyCreateRoom()
    {
        roomLoginButton.interactable = false;

        yield return new WaitForSeconds(1.0f);

        roomLoginButton.interactable = true;

        isReadyCreateRoom = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[Speak Speak] �κ� ���� ����");
    }

    public override void OnJoinedRoom()
    {
        if(isTransitionEnd)
        {
            TransitionManager.Instance().Transition(circleWipeOut, 0);
            TransitionManager.Instance().onTransitionCutPointReached += SetActiveRoomPanel;
        }

        //SetLobbyPanelActive(false);
        //SetRoomPanelActive(true);

        roomManager.SetUpRoomCode(PhotonNetwork.CurrentRoom.Name);
    }

    private void IsTranstionEnd()
    {
        isTransitionEnd = true;
        TransitionManager.Instance().onTransitionEnd -= IsTranstionEnd;
    }

    private void SetActiveRoomPanel()
    {
        SetLobbyPanelActive(false);
        SetRoomPanelActive(true);

        TransitionManager.Instance().onTransitionCutPointReached -= SetActiveRoomPanel;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == (short)Photon.Realtime.ErrorCode.GameFull)
        {
            ErrorManager.Instance.InvokeErrorMessage("Room Full !!!!");
            Debug.Log("[Speak Speak] �� ���� ����: ���� �� á���ϴ�.");
        }
        else
        {
            ErrorManager.Instance.InvokeErrorMessage("Room Code ERROR !!!!");
            Debug.Log("[Speak Speak] �� ���� ����");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorManager.Instance.InvokeErrorMessage("Room Create ERROR !!!!");
    }

    public override void OnLeftRoom()
    {
        SetLobbyPanelActive(true);
        SetRoomPanelActive(false);
    }
     
    public void SetLobbyPanelActive(bool isActive)
    {
        lobbyPanel?.gameObject.SetActive(isActive);
    }

    public void SetRoomPanelActive(bool isActive)
    {
        for ( int index = 0; index < roomPanel.Length; index++ )
        {
            roomPanel[index].SetActive(isActive);
        }
    }
}

