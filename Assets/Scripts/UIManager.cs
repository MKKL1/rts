using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SelectionTool selectionTool;
    public RTSNetworkManager rtsNetworkManager;
    public TMP_Text list;
    public TMP_Text playerlist;

    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (selectionTool == null) selectionTool = GameMain.instance.localSelectionTool;
        if (rtsNetworkManager == null) rtsNetworkManager = RTSNetworkManager.instance;

        selectionTool.selectionChangeEvent += updateList;
        rtsNetworkManager.playerListChangeEvent.AddListener(updatePlayerList);

    }

    public void updateList()
    {
        string text = "Lista: \n";
        foreach(Transform transform in selectionTool.selected)
        {
            text += transform.GetComponent<Entity>().entityName + "\n";
        }
        list.SetText(text);
    }

    public void updatePlayerList()
    {
        Debug.Log("HERE ?????");
        string text = "Gracze: \n";
        foreach (PlayerScript player in rtsNetworkManager.playerList)
        {
            text += player.playerName + "\n";
            Debug.Log(player.playerName);
        }
        playerlist.SetText(text);
    }
}
