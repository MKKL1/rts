using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SelectionTool selectionTool;
    public GameMain gameMain;
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
        if (gameMain == null) gameMain = GameMain.instance;

        selectionTool.selectionChangeEvent += updateList;
        gameMain.playerListChangeEvent += updatePlayerList;

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
        foreach (var player in gameMain.playerList)
        {
            text += player.playerName + "\n";
            //Debug.Log(player.playerName);
        }
        playerlist.SetText(text);
    }
}
