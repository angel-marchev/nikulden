using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityStandardAssets.Network;

public class GameManager : NetworkBehaviour
{
    static public GameManager s_Instance;

    //this is static so tank can be added even withotu the scene loaded (i.e. from lobby)
    static public List<TankManager> m_Tanks = new List<TankManager>();             // A collection of managers for enabling and disabling different aspects of the tanks.

    [SyncVar]
    public int m_NumRoundsToWin = 10;          // The number of rounds a single player has to win to win the game.
    [SyncVar]
    public CameraControl m_CameraControl;     // Reference to the CameraControl script for control during different phases.

    public Canvas m_GameCanvas;           // Reference to the prefab the players will control.
    public Text[] m_ATexts;
    public InputField[] m_AInputFields;
    public Toggle m_AToggle;
    public Text[] m_BTexts;
    public InputField[] m_BInputFields;
    public Toggle m_BToggle;
    public Text[] m_CTexts;
    public InputField[] m_CInputFields;
    public Toggle m_CToggle;
    public Text m_TimerDisplay;

    public Stopwatch m_stopwatch = new Stopwatch();
    private TimeSpan m_maxTime = new TimeSpan(0, 0, 0, 10);
    public bool m_submitting;
    public bool m_submitted;

    private int m_RoundNumber;                  // Which round the game is currently on.

    void Awake()
    {
        s_Instance = this;
    }

    [ServerCallback]
    private void Start()
    {
        // Once the tanks have been created and the camera is using them as targets, start the game.
        StartCoroutine(GameLoop());
    }

    /// <summary>
    /// Add a tank from the lobby hook
    /// </summary>
    /// <param name="tank">The actual GameObject instantiated by the lobby, which is a NetworkBehaviour</param>
    /// <param name="playerNum">The number of the player (based on their slot position in the lobby)</param>
    /// <param name="c">The color of the player, choosen in the lobby</param>
    /// <param name="name">The name of the Player, choosen in the lobby</param>
    /// <param name="localID">The localID. e.g. if 2 player are on the same machine this will be 1 & 2</param>
    static public void AddTank(GameObject tank, int playerNum, Color c, string name, int localID)
    {
        TankManager tmp = new TankManager();
        tmp.m_Instance = tank;
        tmp.m_PlayerNumber = playerNum;
        tmp.m_PlayerColor = c;
        tmp.m_PlayerName = name;
        tmp.m_LocalPlayerID = localID;
        tmp.Setup();

        m_Tanks.Add(tmp);
    }

    public void RemoveTank(GameObject tank)
    {
        TankManager toRemove = null;
        foreach (var tmp in m_Tanks)
        {
            if (tmp.m_Instance == tank)
            {
                toRemove = tmp;
                break;
            }
        }

        if (toRemove != null)
            m_Tanks.Remove(toRemove);
    }

    // This is called from start and will run each phase of the game one after another. ONLY ON SERVER (as Start is only called on server)
    private IEnumerator GameLoop()
    {
        while (m_Tanks.Count < 2)
            yield return null;

        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        StartCoroutine(GameLoop());
    }


    private IEnumerator RoundStarting()
    {
        UnityEngine.Debug.Log("starting?");
        //we notify all clients that the round is starting
        RpcRoundStarting();

        yield return new WaitForSeconds(0.5f);
    }

    [ClientRpc]
    void RpcRoundStarting()
    {
        m_GameCanvas.gameObject.SetActive(true);

        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        m_CameraControl.SetAppropriatePositionAndSize();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;
        m_submitting = false;
        m_submitted = false;
    }

    private IEnumerator RoundPlaying()
    {
        //notify clients that the round is now started, they should allow player to move.
        RpcRoundPlaying();

        foreach (var fish in m_Tanks.Select(x => x.m_Movement))
        {
            if (m_RoundNumber <= 1)
            {
                fish.RpcSetResultsData(0, true, 10, 0, 0, 0, false, 0, 0, 0, 0, 10);
                fish.RpcSetResultsData(1, false, 0, 0, 0, 0, false, 0, 0, 0, 0, 0);
                fish.RpcSetResultsData(2, false, 0, 0, 0, 0, false, 0, 0, 0, 0, 0);
            }
            else
            {
                fish.RpcEvolveData();
            }

            fish.RpcReset();
        }

        yield return new WaitForSeconds(0.5f);

        // While there is not one tank left...
        while (m_stopwatch.Elapsed <= m_maxTime)
        {
            // ... return on the next frame.
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    [ClientRpc]
    void RpcRoundPlaying()
    {
        m_stopwatch.Reset();
        m_stopwatch.Start();
    }

    public void Submit()
    {
        m_submitting = true;
    }
}