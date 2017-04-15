using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour {

    public GameObject[] MyTips;
    private bool[] myTipsEnabled;
    public GameObject CoverPanel;

	// Use this for initialization
	void Start () {
        myTipsEnabled = new bool[MyTips.Length];
        for(int x = 0; x < myTipsEnabled.Length; x++)
        {
            myTipsEnabled[x] = false;
            MyTips[x].SetActive(false);
        }
        CoverPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnAlways(int tipNum)
    {
        myTipsEnabled[tipNum] = false;
        _spawn(7);
    }

    #region Static Functions
    public static void Spawn(int tipNum)
    {
        GameObject.FindGameObjectWithTag("Tips").GetComponent<Tips>()._spawn(tipNum);
    }
    #endregion

    #region Helper Functions
    private void _spawn(int tipNum)
    {
        if (myTipsEnabled[tipNum])
            return;

        //This is the tooltip to explain tooltips.
        //if (!myTipsEnabled[5])
        //{
        //    myTipsEnabled[5] = true;
        //    MyTips[5].SetActive(true);
        //}
        myTipsEnabled[tipNum] = true;
        MyTips[tipNum].SetActive(true);
        //Pause Game
        PauseHandler.PauseGame();
        //Enable panel raycast.
        CoverPanel.SetActive(true);
    }
    #endregion
}
