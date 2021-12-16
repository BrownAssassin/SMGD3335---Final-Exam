using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject MenuPanel;
    public GameObject RoomPanel;

    public InputField createInputField;
    public InputField joinInputField;
    public Text error;

    public Text roomName;
    public Text playerCount;

    public GameObject playerListing;
    public Transform playerListContent;

    public Button startButton;
    public Transform buttonOrganizer;

    void Start()
    {
        MenuPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createInputField.text))
            return;
        
        PhotonNetwork.CreateRoom(createInputField.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInputField.text);
    }

    public override void OnJoinedRoom()
    {
        MenuPanel.SetActive(false);
        RoomPanel.SetActive(true);

        roomName.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        playerCount.text = "" + players.Length;

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListing, playerListContent).GetComponent<PlayerListing>().SetPlayerInfo(players[i]);

            if (i == 0)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        error.text = message;
        Debug.Log("Error creating room! " + message);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Loading");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListing, playerListContent).GetComponent<PlayerListing>().SetPlayerInfo(newPlayer);
    }

    public void OnClickStartGame()
    {
        PhotonNetwork.LoadLevel("Arena");
    }
}
