using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks 
{
    public TMP_InputField userNameText;
    public TMP_InputField roomNameText;
    public TMP_InputField maxPlayer;

    public GameObject PlayerNamePanel;
    public GameObject connectingPanel;
    public GameObject LobbyPanel;
    public GameObject RoomCreatePanel;
    public GameObject RoomListPanel;

    private Dictionary<string, RoomInfo> roomListData;

    public GameObject roomListPrefab;
    public GameObject roomListParent;

    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> playerListGameObject;

    [Header("inside room panel")]
    public GameObject InsideRoomPanel;
    public GameObject playerListItemPrefab;
    public GameObject playerListItemParent;
    public GameObject PlayButton;

    #region UnityMethods

    // Start is called before the first frame update
    void Start()
    {
        ActivateMyPanel(PlayerNamePanel.name);
        roomListData = new Dictionary<string, RoomInfo>();
        roomListGameObject = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Network state : " + PhotonNetwork.NetworkClientState);
    }

    #endregion

    #region UIMethods

    public void OnLoginClick()
    {
        string name = userNameText.text;
        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActivateMyPanel(connectingPanel.name);
        }
        else
        {
            Debug.Log("name is empty ra erripuka");
        }
    }

    public void OnClickRoomCreate()
    {
        string roomName = roomNameText.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = roomName + Random.Range(0, 1000);
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) int.Parse(maxPlayer.text);
        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }

    public void OnCancelClick()
    {
        ActivateMyPanel(LobbyPanel.name);  
    }

    public void RoomListBtnClick()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivateMyPanel(RoomListPanel.name);
    }

    public void BackFromRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("Leave lobby");
            PhotonNetwork.LeaveLobby();
        }
        ActivateMyPanel(LobbyPanel.name);
    }

    public void BackFromPlayerList()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leave room");
            PhotonNetwork.LeaveRoom();
        }
        ActivateMyPanel(LobbyPanel.name);
    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
    #endregion

    #region PHOTON_CALLBACKS

    public override void OnConnected()
    {
        Debug.Log("Connected to internet");
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName  + " is connected to photon ra lambdi.....");
        ActivateMyPanel(LobbyPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + "Room is Created !");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is joined in" + PhotonNetwork.CurrentRoom.Name + "Room is Created !");
        ActivateMyPanel(InsideRoomPanel.name);

        if(playerListGameObject == null)
        {
            playerListGameObject = new Dictionary<int, GameObject>();
        }

        if(PhotonNetwork.IsMasterClient)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(playerListItemPrefab);
            playerListItem.transform.SetParent(playerListItemParent.transform);
            playerListItem.transform.localScale = Vector3.one;

            playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = p.NickName;
            if(p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(false);
            }

            playerListGameObject.Add(p.ActorNumber, playerListItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(playerListItemPrefab);
        playerListItem.transform.SetParent(playerListItemParent.transform);
        playerListItem.transform.localScale = Vector3.one;

        playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(false);
        }

        playerListGameObject.Add(newPlayer.ActorNumber, playerListItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("actor number remove");
        Destroy(playerListGameObject[otherPlayer.ActorNumber]);
        playerListGameObject.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.IsMasterClient)
        {
            PlayButton.SetActive(true);
        }
        else
        {
            PlayButton.SetActive(false);
        }
    }

    public override void OnLeftRoom()
    {
        ActivateMyPanel(LobbyPanel.name);

        if (playerListGameObject != null)
        {
            foreach (GameObject obj in playerListGameObject.Values)
            {
                Destroy(obj);
            }
            playerListGameObject.Clear();
        }

        Debug.Log("Successfully left the room!");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //Clear
        ClearRoomList();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log("rooms name : "+room.Name);
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (roomListData.ContainsKey(room.Name))
                {
                    roomListData.Remove(room.Name);
                }
            }
            else
            {
                if(roomListData.ContainsKey(room.Name))
                {
                    roomListData[room.Name] = room;
                }
                else
                {
                    roomListData.Add(room.Name, room);
                }
            }
            
        }

        //Generate list item

        foreach(RoomInfo roomitem in roomListData.Values)
        {
            GameObject roomListItemObject = Instantiate(roomListPrefab);
            roomListItemObject.transform.SetParent(roomListParent.transform);
            roomListItemObject.transform.localScale = Vector3.one;

            roomListItemObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = roomitem.Name;
            roomListItemObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roomitem.PlayerCount + "/" + roomitem.MaxPlayers;
            roomListItemObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomitem.Name));
            roomListGameObject.Add(roomitem.Name, roomListItemObject);
        }

    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left the lobby, clearing room list...");
        ClearRoomList();
        roomListData.Clear();
    }
    #endregion

    #region Public_Methods

    public void RoomJoinFromList(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    public void ClearRoomList()
    {
        if(roomListGameObject.Count > 0)
        {
            foreach (var v in roomListGameObject.Values)
            {
                Destroy(v);
            }
            roomListGameObject.Clear();
        }
        
    }

    public void ActivateMyPanel(string panelName)
    {
        LobbyPanel.SetActive(panelName.Equals(LobbyPanel.name));
        PlayerNamePanel.SetActive(panelName.Equals(PlayerNamePanel.name));
        RoomCreatePanel.SetActive(panelName.Equals(RoomCreatePanel.name));
        connectingPanel.SetActive(panelName.Equals(connectingPanel.name));
        RoomListPanel.SetActive(panelName.Equals(RoomListPanel.name));
        InsideRoomPanel.SetActive(panelName.Equals(InsideRoomPanel.name));
    }

    #endregion
}
