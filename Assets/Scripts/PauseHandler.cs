using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LoLSDK;

public class PauseHandler : MonoBehaviour {

    public ParticleSystem EvapGas;
    public ParticleSystem PartSys;
    public PartMan PartM;
    public GameControls GCont;
    public GameObject UIOverlay;

    //TODO: PAUSE AUDIO


    // Use this for initialization
    void Start() {
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
                GCont.IsPaused = true;
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
                GCont.IsPaused = false;
                //enable all buttons
                UIOverlay.SetActive(false);
                //TODO: UNPAUSE AUDIO
                break;
            default:
                CBUG.SrsError("BAD GAMESTATE GIVEN!: " + (newState.ToString()));
                break;
        }

    }
}
