// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

// # Project
using Photon.Pun;
using TMPro;

public class RoleSelector : MonoBehaviour
{
    [SerializeField]
    private PhotonRoomManager               photonRoomManager;

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

            // 역할 취소
            if(myRoleID == roleID)
            {
                UpdateNickname(roleID, string.Empty);

                characters[roleID].SetBool("isSelect", false);

                myRoleID      = -1;
                myRole.RoleID = -1;

                UpdateRole(roleID, false);

                return;
            }

            // 역할 선택 후 다른 역할로 바꿀 때
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
            // 역할을 처음 선택할 떄 
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
                Debug.Log("이 역할은 이미 사용중입니다");
            }
        }
    }

    public void UpdateMyRole(bool isUsing)
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

    [PunRPC]
    private void UpdateNicknameRpc(int roleID, string nickname)
    {
        nickNameTexts[roleID].text = nickname;
    }

    private void UpdateLighting(int roleID, bool isUsing)
    {
        lights[roleID].SetActive(isUsing);
    }
}
