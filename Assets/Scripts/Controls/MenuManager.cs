using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject playerprefab;
    public GameObject playerListObject;
    public GameObject roomObject;
    public GameObject mainMenuObject;
    public GameObject loadingObject;
    public NetworkManager networkManager;
    int count = 0;
    void Start()
    {
        //networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        GameMain.instance.playerListChangeEvent += updatePlayerList;
    }
    public void connect()
    {
        loadingObject.SetActive(true);

        networkManager.StartClient();

        mainMenuObject.SetActive(false);
        loadingObject.SetActive(false);
        roomObject.SetActive(true);
        //SceneManager.LoadScene(1);
    }

    public void host()
    {
        networkManager.StartHost();

        mainMenuObject.SetActive(false);
        roomObject.SetActive(true);
        //SceneManager.LoadScene(1);
    }

    public void updatePlayerList()
    {
        count = 0;
        foreach (Transform child in playerListObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var player in GameMain.instance.playerList)
        {
            GameObject c = Instantiate(playerprefab, playerListObject.transform);
            c.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, count*120);
            c.transform.Find("PlayerText").GetComponent<TMP_Text>().text = player.playerName;
            count++;
        }
        
    }

    public void startButton()
    {
        NetworkMenuManager.instance.cmdStartGame();
    }

}
