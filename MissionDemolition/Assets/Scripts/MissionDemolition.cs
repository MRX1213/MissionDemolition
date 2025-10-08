using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static public MissionDemolition S;

    [Header("Inscribed")]
    public TextMeshProUGUI uitLevel;
    public TextMeshProUGUI uitShots;
    public Vector3 castlePos;
    public GameObject[] castles;

    [Header("Dynamic")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameMode mode = GameMode.idle;
    public GameObject castle;
    public string showing = "Show SLingshot";

    // Start is called before the first frame update
    void Start()
    {
        S = this;
        level = 0;
        shotsTaken = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        castle = Instantiate(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;
        Goal.goalMet = false;
        UpdateGUI();
        mode = GameMode.playing;
    }


    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateGUI();

        if ((mode == GameMode.playing) && (Goal.goalMet))
        {
            mode = GameMode.levelEnd;
            Invoke("NextLevel", 2f);
        }
    }

    public void ShotFired()
    {
        shotsTaken++;
    }

    void NextLevel()
    {
        level++;

        if(level == levelMax)
        {
            level = 0;
            shotsTaken = 0;
        }
        StartLevel();
    }
}