using UnityEngine;
using System.Collections;

public class ArrayTargetActive : MonoBehaviour {

    public int StartActive;
    public bool LoopArray;

    public GameObject[] Targets;

    private int currentActive;

	// Use this for initialization
	void Start () {
        currentActive = StartActive;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActivateNext()
    {
        currentActive = currentActive + 1 >= Targets.Length? 0 : currentActive + 1;
        _activate(currentActive);
    }

    public void ActivatePrevious()
    {
        currentActive = currentActive - 1 < 0 ? Targets.Length - 1 : currentActive - 1;
        _activate(currentActive);
    }

    public void Activate(int targetNum)
    {
        _activate(targetNum);
    }

    private void _activate(int targetNum)
    {
        for(int x = 0; x < Targets.Length; x++) {
            Targets[x].SetActive(false);
        }
        if (!LoopArray && currentActive == Targets.Length - 1)
            return;
        Targets[targetNum].SetActive(true);
    }
}
