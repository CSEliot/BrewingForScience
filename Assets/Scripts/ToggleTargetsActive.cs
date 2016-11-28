using UnityEngine;
using System.Collections;

public class ToggleTargetsActive : MonoBehaviour {

    public GameObject[] Targets;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle()
    {
        for(int x = 0; x < Targets.Length; x++) {
            Targets[x].SetActive(!Targets[x].activeSelf);    
        }
    }
}
