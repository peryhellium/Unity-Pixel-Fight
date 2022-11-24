using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private List<GameObject> availableRooms = new List<GameObject>();
    private string selectedRoom;
    public GameObject buttonPrefab;
    public TMP_InputField nickname;
    public TMP_InputField roomName;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject rb in availableRooms)
        {
            Destroy(rb);
        }

        availableRooms.Clear();


        for (int i = 0; i < roomList.Count; i++)
        {
            string roomName = roomList[i].Name;
            GameObject newButton = Instantiate(buttonPrefab, buttonPrefab.transform.parent);
            newButton.GetComponent<Button>().onClick.AddListener(() => roomSelector(roomName));
            newButton.GetComponentInChildren<TMP_Text>().text = roomName;
            newButton.SetActive(true);

            availableRooms.Add(newButton);
        }
    }

    public void roomSelector(string roomName)
    {
        selectedRoom = roomName;
    }

    public async void CreateRoom()
    {
        PhotonNetwork.NickName = nickname.text;
        PhotonNetwork.CreateRoom(roomName.text);

        await Task.Delay(2000);

        PhotonNetwork.LoadLevel("fps");
    }

    public void JoinRoom()
    {
        PhotonNetwork.NickName = nickname.text;
        PhotonNetwork.JoinRoom(selectedRoom);
    }
}
