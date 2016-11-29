using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GetSprite : MonoBehaviour {

    public Image spr;
    public GameControls cont;

    public AudioManager Audio;

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
        if(cont.CurrentNPC + 1 >= cont.TotalCharactersPerDay && cont.CurrentDay < 2) {
            cont.DayPanels[cont.CurrentDay + 1].SetActive(true);
            Audio.MusicPlayer.Stop();
            Audio.MusicPlayer.clip = Audio.Musics[cont.CurrentDay + 1];
            Audio.MusicPlayer.Play();
        } else {
            cont.SpawnNPC();
        }
    }

    public void SetSpawnNPC2()
    {
        cont.SpawnNPC();
    }
}
