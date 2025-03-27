// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Project
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;
using UnityEngine.SceneManagement;


// Todo. ��ũ��Ʈ Title Scene���� �̵���Ű�� 

public class NewPhotonNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform logo;
    [SerializeField]
    private GameObject loadingText;

    private void Awake()
    {
        Application.runInBackground = true;

        // ���� �� �ڵ� ����ȭ
        PhotonNetwork.AutomaticallySyncScene = true;

        Screen.SetResolution(1280, 720, false);

        PlayerPrefs.SetFloat("HighTime", 6000);
    }

    private void Start()
    {
        Invoke();

        StartCoroutine(nameof(OnLoadingText));
    }

    private void LoadLobbyScene()
    {
        // ���� ���� ����
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[SPEAK SPEAK] ���� ���� �Ϸ�");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[SPEAK SPEAK] �κ� ���� �Ϸ�");

        StopCoroutine(nameof(OnLoadingText));

        SceneManager.LoadScene("1. Maintitle");
    }

    private void Invoke()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(logo.transform.DOMoveY(2.0f, 0.5f));
        sequence.Append(logo.DOScale(1.3f, 0.6f));
        sequence.Append(logo.DOScale(1.0f, 0.6f));

        sequence.Play().OnComplete(LoadLobbyScene);
    }

    private IEnumerator OnLoadingText()
    {
        yield return new WaitForSeconds(3.0f);

        loadingText.SetActive(true);
    }

    public void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }
}
