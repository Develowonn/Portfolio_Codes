// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;
using TMPro;

// # Photon 
using Photon.Chat;
using Photon.Pun;

using ExitGames.Client.Photon;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public static PhotonChatManager instance { get; private set; }

    [SerializeField]
    private PhotonView      photonView;

    [Header("Chat Panel")]
    [SerializeField]
    private GameObject      chatPanel;
    [SerializeField]
    private GameObject      chatAlarm;
    [SerializeField]
    private TextMeshProUGUI chatText;
    [SerializeField]
    private TMP_InputField  chatInputField;

    private string          userNickname;
    private string          currentChat;

    private bool            isConnected = false;
    private ChatClient      chatClient;

    public TextMeshProUGUI ChatText
    {
        get => chatText;
        set => chatText = value;
    }

    public GameObject      ChatAlarm  => chatAlarm;
    public ChatClient      ChatClient => chatClient;

    private void Awake()
    {
        instance = this;
    }

    public void Initialize()
    {
        isConnected         = true; // 연결 상태를 true로 설정

        userNickname        = PhotonNetwork.LocalPlayer.NickName;

        chatInputField.text = string.Empty;
        chatText.text       = string.Empty;
        currentChat         = string.Empty;

        chatClient          = new ChatClient(this);

        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userNickname));

        Debug.Log("채팅 서버 연결 시도 중....");
    }

    private void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }

        if (chatPanel.activeSelf == false) return;

        if (chatInputField.text != string.Empty && Input.GetKeyDown(KeyCode.Return))
        {
            SubmitPublicChatOnClick();
        }
    }

    #region Photon Chat Methods
    /// <summary>디버그 메시지를 처리합니다.</summary>
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    /// <summary>채팅 상태가 변경될 때 호출됩니다.</summary>
    public void OnChatStateChange(ChatState state)
    {
    }

    /// <summary>서버에 성공적으로 연결되었을 때 호출됩니다.</summary>
    public void OnConnected()
    {
        Debug.Log("채팅 서버에 연결되었습니다.");
        isConnected = true; // 연결 상태를 true로 설정

        // 새로운 방의 이름으로 채널 구독 (이전 메시지 불러오지 않음)
        chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name }, 0);

        // 새로운 방에 들어왔으므로, 채팅 텍스트를 초기화
        chatText.text = string.Empty;
        currentChat   = string.Empty;

        // 방에 들어왔다는 메시지를 전송 (자신 포함)
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, $"<color=#0000FF>{userNickname}님이 방에 참가하셨습니다!!");
    }

    /// <summary>서버와의 연결이 끊어졌을 때 호출됩니다.</summary>
    public void OnDisconnected()
    {
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, $"<color=#FF0000>{PhotonNetwork.NickName}님이 방을 나가셨습니다.");

        chatClient.Unsubscribe(new string[] { PhotonNetwork.CurrentRoom.Name });

        chatText.text = string.Empty;

        isConnected = false;

        Debug.Log("채팅 서버에 연결이 끊어졌습니다.");
    }

    /// <summary>지정된 채널에서 메시지를 수신했을 때 호출됩니다.</summary>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        chatAlarm.gameObject.SetActive(true);

        for (int i = 0; i < senders.Length; i++)
        {
            string message = messages[i].ToString();

            // 발신자 이름을 포함하지 않고 메시지 내용만 표시
            chatText.text += $"<color=#000000>\n{message}";
        }
    }

    /// <summary>개인 메시지를 수신했을 때 호출됩니다.</summary>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    /// <summary>다른 사용자의 상태가 업데이트되었을 때 호출됩니다.</summary>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    /// <summary>채널 구독에 성공했을 때 호출됩니다.</summary>
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("채팅 서버 접속에 성공하셨습니다.");
    }

    /// <summary>채널 구독을 취소했을 때 호출됩니다.</summary>
    public void OnUnsubscribed(string[] channels)
    {
    }

    /// <summary>다른 사용자가 채널에 구독했을 때 호출됩니다.</summary>
    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    #endregion

    public void SubmitPublicChatOnClick()
    {
        if (chatInputField.text != string.Empty)
        {
            chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, $"<color=#FFFFFF>{userNickname} : {chatInputField.text}");
            chatInputField.text = string.Empty;
        }
    }
}
