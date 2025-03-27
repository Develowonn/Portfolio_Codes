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

public class PhotonLobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PhotonRoomManager roomManager;
    [SerializeField]
    private TransitionSettings   circleWipe;
    [SerializeField]
    private TransitionSettings   circleWipeOut;

    [Header("Panel")]
    [SerializeField]
    private GameObject           lobbyPanel;
    [SerializeField]
    private GameObject[]         roomPanel;

    [Header("InputField")]
    [SerializeField]
    private TMP_InputField       roomCodeInputField;
    [SerializeField]
    private TMP_InputField       nickNameInputField;

    [Header("Text")]
    [SerializeField]
    private TMP_Text             loginButtonText;

    [Header("Button")]
    [SerializeField]
    private Button               roomLoginButton;

    private bool                 isReadyCreateRoom = true;
    private bool                 isTransitionEnd   = true;

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

        Debug.Log("클릭");

        if (nickNameInputField.text == string.Empty)
        {
            ErrorManager.Instance.InvokeErrorMessage("Nickname Null !!!!");
            return;
        }

        // 닉네임 설정
        PhotonNetwork.NickName = nickNameInputField.text;
        
        // 룸 코드를 입력하지 않았을 때 방생성
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
        // 룸 코드를 입력했을 때 방 가입 
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

    public override void OnJoinedLobby()
    {
        Debug.Log("[Speak Speak] 로비 입장 성공");
    }

    public override void OnJoinedRoom()
    {
        if(isTransitionEnd)
        {
            TransitionManager.Instance().Transition(circleWipeOut, 0);
            TransitionManager.Instance().onTransitionCutPointReached += SetActiveRoomPanel;
        }

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
            Debug.Log("[Speak Speak] 방 입장 실패: 방이 꽉 찼습니다.");
        }
        else
        {
            ErrorManager.Instance.InvokeErrorMessage("Room Code ERROR !!!!");
            Debug.Log("[Speak Speak] 방 입장 실패");
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

