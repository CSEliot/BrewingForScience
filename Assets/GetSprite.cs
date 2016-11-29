using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetSprite : MonoBehaviour {

    public Image spr;
    public GameControls cont;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Get(float spriteNum)
    {
        spr.sprite = cont.GetCurrentCharSprite(spriteNum);
    }

    public void SetRequest()
    {
        cont.SetRequest();
    }

    public void EndRequest()
    {
        cont.EndRequest();
    }

    public void SetFadeAnim()
    {
        cont.FadeInGameAnim();
    }

    public void SetSpawnNPC()
    {
        cont.SpawnNPC();
    }
}
