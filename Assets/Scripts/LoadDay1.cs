using UnityEngine;
using System.Collections;

public class LoadDay1 : MonoBehaviour {

    private int dayNum = -1;

    public GameControls game;

	// Use this for initialization
	void Awake () {
        dayNum++;
        game.SpawnNPC();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
