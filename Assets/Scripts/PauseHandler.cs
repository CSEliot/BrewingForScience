using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class PauseHandler : MonoBehaviour {

    public ParticleSystem EvapGas;
    public ParticleSystem PartSys;
    public PartMan PartM;
    private GameControls gCont;
    public GameObject UIOverlay;

    //TODO: PAUSE AUDIO


    // Use this for initialization
    void Start() {

        gCont = GameControls.GetSelf();

        LOLSDK.Instance.GameStateChanged += new GameStateChangedHandler(this.OnPauseStateChange);
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPauseStateChange(GameState newState)
    {
        switch(newState){
            case GameState.Paused:
                //Pause particle FX
                PartSys.Pause();
                EvapGas.Pause();
                //Pause heatup
                PartM.IsPaused = true;
                gCont.IsPaused = true;
                //disable all buttons
                UIOverlay.SetActive(true);
                //LOLSDK.Instance.
                //TODO: PAUSE AUDIO
                break;
            case GameState.Resumed:
                //Unpause particle FX
                PartSys.Play();
                EvapGas.Play();
                //unpause heatup
                PartM.IsPaused = false;
                gCont.IsPaused = false;
                //enable all buttons
                UIOverlay.SetActive(false);
                //TODO: UNPAUSE AUDIO
                break;
            default:
                CBUG.SrsError("BAD GAMESTATE GIVEN!: " + (newState.ToString()));
                break;
        }

    }

    public void _PauseGame()
    {
        OnPauseStateChange(GameState.Paused);
    }

    public static void PauseGame()
    {
        GameObject.FindGameObjectWithTag("Pause").GetComponent<PauseHandler>()._PauseGame();
    }

    public void _UnpauseGame()
    {
        OnPauseStateChange(GameState.Resumed);
    }

    public static void UnpauseGame()
    {
        GameObject.FindGameObjectWithTag("Pause").GetComponent<PauseHandler>()._UnpauseGame();
    }

}
