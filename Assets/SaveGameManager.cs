using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instance;

    public ObjectCollector playerCollector;
    public int currentLevel = 1;
    public Vector3 cpPos = Vector3.zero;
    public Quaternion cpRot = Quaternion.identity;

    private List<string> lvl1_melons = new List<string>();
    private List<string> lvl1_crates = new List<string>();

    private List<string> lvl2_melons = new List<string>();
    private List<string> lvl2_crates = new List<string>();

    private int melonCount = 0;
    private int idGen = 0;
    private GameUIController gameUIController;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);
    }

    // Player respawn
    public void SetCurrentCheckpoint(Vector3 cpPos, Quaternion cpRot)
    {
        this.cpPos = cpPos;
        this.cpRot = cpRot;
    }

    // ===== MELONS ======
    public void IWasLooted_melon(string ID)
    {
        if (currentLevel == 1)
            lvl1_melons.Add(ID);
        else if (currentLevel == 2)
            lvl2_melons.Add(ID);
    }
    public bool WasILootedAlready_melon(string ID)
    {
        if (currentLevel == 1)
        {
            if (lvl1_melons.Contains(ID))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (lvl2_melons.Contains(ID))
            {
                return true;
            }
            return false;
        }
    }
    public void AddMelon()
    {
        if (gameUIController == null)
        {
            gameUIController = GameObject.Find("Canvas").GetComponentInChildren<GameUIController>();
        }
        melonCount++;
        gameUIController?.SetMelonCount(melonCount);
    }


    // ====== CRATES ======
    public void IWasBroken_box(string ID)
    {
        if (currentLevel == 1)
            lvl1_crates.Add(ID);
        else if (currentLevel == 2)
            lvl2_crates.Add(ID);
    }
    public bool WasIBrokenAlready(string ID)
    {
        if (currentLevel == 1)
        {
            if (lvl1_crates.Contains(ID))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (lvl2_crates.Contains(ID))
            {
                return true;
            }
            return false;
        }
    }
    public ObjectCollector GetCollector()
    {
        if (playerCollector == null)
        {
            playerCollector = GameObject.Find("Player").GetComponent<ObjectCollector>();
        }
        return playerCollector;
    }
}
