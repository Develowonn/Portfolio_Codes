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
        isConnected         = true; // ���� ���¸� true�� ����

        userNickname        = PhotonNetwork.LocalPlayer.NickName;

        chatInputField.text = string.Empty;
        chatText.text       = string.Empty;
        currentChat         = string.Empty;

        chatClient          = new ChatClient(this);

        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userNickname));

        Debug.Log("ä�� ���� ���� �õ� ��....");
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
    /// <summary>����� �޽����� ó���մϴ�.</summary>
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    /// <summary>ä�� ���°� ����� �� ȣ��˴ϴ�.</summary>
    public void OnChatStateChange(ChatState state)
    {
    }

    /// <summary>������ ���������� ����Ǿ��� �� ȣ��˴ϴ�.</summary>
    public void OnConnected()
    {
        Debug.Log("ä�� ������ ����Ǿ����ϴ�.");
        isConnected = true; // ���� ���¸� true�� ����

        // ���ο� ���� �̸����� ä�� ���� (���� �޽��� �ҷ����� ����)
        chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name }, 0);

        // ���ο� �濡 �������Ƿ�, ä�� �ؽ�Ʈ�� �ʱ�ȭ
        chatText.text = string.Empty;
        currentChat   = string.Empty;

        // �濡 ���Դٴ� �޽����� ���� (�ڽ� ����)
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, $"<color=#0000FF>{userNickname}���� �濡 �����ϼ̽��ϴ�!!");
    }

    /// <summary>�������� ������ �������� �� ȣ��˴ϴ�.</summary>
    public void OnDisconnected()
    {
        chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, $"<color=#FF0000>{PhotonNetwork.NickName}���� ���� �����̽��ϴ�.");

        chatClient.Unsubscribe(new string[] { PhotonNetwork.CurrentRoom.Name });

        chatText.text = string.Empty;

        isConnected = false;

        Debug.Log("ä�� ������ ������ ���������ϴ�.");
    }

    /// <summary>������ ä�ο��� �޽����� �������� �� ȣ��˴ϴ�.</summary>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        chatAlarm.gameObject.SetActive(true);

        for (int i = 0; i < senders.Length; i++)
        {
            string message = messages[i].ToString();

            // �߽��� �̸��� �������� �ʰ� �޽��� ���븸 ǥ��
            chatText.text += $"<color=#000000>\n{message}";
        }
    }

    /// <summary>���� �޽����� �������� �� ȣ��˴ϴ�.</summary>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    /// <summary>�ٸ� ������� ���°� ������Ʈ�Ǿ��� �� ȣ��˴ϴ�.</summary>
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    /// <summary>ä�� ������ �������� �� ȣ��˴ϴ�.</summary>
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("ä�� ���� ���ӿ� �����ϼ̽��ϴ�.");
    }

    /// <summary>ä�� ������ ������� �� ȣ��˴ϴ�.</summary>
    public void OnUnsubscribed(string[] channels)
    {
    }

    /// <summary>�ٸ� ����ڰ� ä�ο� �������� �� ȣ��˴ϴ�.</summary>
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
