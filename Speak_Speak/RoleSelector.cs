// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Project
using Photon.Pun;
using TMPro;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoleSelector : MonoBehaviour
{
    [SerializeField]
    [SerializeField]
    private LayerMask                       layer;
    [SerializeField]
    private GameObject                      roomPanel;
    [SerializeField]
    private GameObject[]                    lights;
    [SerializeField]
    private Animator[]                      characters;
    [SerializeField]
    private TMP_Text[]                      nickNameTexts;

    private MyRole                          myRole;
    private PhotonView                      photonView;

    private int                             myRoleID;
    private Dictionary<int, bool>           isRoleUsage;

    private new Camera                      camera;

    public Dictionary<int, bool> IsRoleUsage { get => isRoleUsage; }

    private void Start()
    {
        camera              = Camera.main;
        photonView          = GetComponent<PhotonView>();

        myRoleID            = -1;

        InitalizeDictionary();

        if (myRole == null)
        {
            myRole = GameObject.Find("MyRole").GetComponent<MyRole>();
        }
    }

    private void Update()
    {
        if (photonRoomManager.IsChatting) return;

        if(roomPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            SelectRole();
        }
    }

    private void OnApplicationQuit()
    {
        if(myRoleID != -1)
        {
            UpdateRole(myRoleID, false);
        }
    }

    private void InitalizeDictionary()
    {
        isRoleUsage = new Dictionary<int, bool>();

        for(int index = 0; index < Constants.MaxPlayerInRoom; index++)
        {
            isRoleUsage[index] = false;
        }
    }

    private void SelectRole()
    {
        Ray         ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit  hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))    
        {
            if (!hit.collider.CompareTag("CharacterTouch")) return;

            int roleID = hit.collider.GetComponent<Role>().RoleID;

            // ���� ��� 
            if(myRoleID == roleID)
            {
                UpdateNickname(roleID, string.Empty);

                characters[roleID].SetBool("isSelect", false);

                myRoleID      = -1;
                myRole.RoleID = -1;

                UpdateRole(roleID, false);

                return;
            }

            // ���� ���� �� �ٸ� ���ҷ� �ٲ� ��
            if(myRoleID != -1 && !isRoleUsage[roleID])
            {
                characters[myRoleID].SetBool("isSelect", false);

                UpdateRole(myRoleID, false);

                UpdateNickname(myRoleID, string.Empty);
                UpdateNickname(roleID, PhotonNetwork.NickName);

                myRoleID            = roleID;
                myRole.RoleID       = roleID;

                characters[myRoleID].SetBool("isSelect", true);

                UpdateRole(roleID, true);
            }
            // ������ ó�� ������ �� 
            else if (!isRoleUsage[roleID])
            {
                UpdateNickname(roleID, PhotonNetwork.NickName);

                myRoleID = roleID;
                myRole.RoleID       = roleID;

                characters[myRoleID].SetBool("isSelect", true);

                UpdateRole(roleID, true);
            }
            else
            {
                Debug.Log("�� ������ �̹� ������Դϴ�");
            }
        }
    }

    public void UpdateRoleMyRole(bool isUsing)
    {
        if (myRoleID == -1) return;

        photonView.RPC(nameof(UpdateRoleRpc), RpcTarget.AllBuffered, myRoleID, isUsing);
    }

    public void UpdateRole(int roleID, bool isUsing)
    {
        photonView.RPC(nameof(UpdateRoleRpc), RpcTarget.AllBuffered, roleID, isUsing);
    }

    public void UpdateNickname(int roleID, string nickname)
    {
        photonView.RPC(nameof(UpdateNicknameRpc), RpcTarget.AllBuffered, roleID, nickname);
    }

    [PunRPC]
    private void UpdateRoleRpc(int roleID, bool isUsing)
    {
        isRoleUsage[roleID] = isUsing;

        if (isUsing == false) UpdateNicknameRpc(roleID, string.Empty);

        UpdateLighting(roleID, isUsing);

        photonRoomManager.UpdateGameReady();
    }

    private void UpdateLighting(int roleID, bool isUsing)
    {
        lights[roleID].SetActive(isUsing);
    }

    [PunRPC]
    private void UpdateNicknameRpc(int roleID, string nickname)
    {
        nickNameTexts[roleID].text = nickname;
    }
}
