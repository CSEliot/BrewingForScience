using UnityEngine;
using System.Collections;

public class ToggleTargetActive : MonoBehaviour {

    public GameObject Target;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Toggle()
    {
        Target.SetActive(!Target.activeSelf);  
    }
}
