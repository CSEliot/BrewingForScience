using UnityEngine;
using UnityEngine.UI;
using LoLSDK;
using System.Collections;

public class GetSprite : MonoBehaviour {

    public Image spr;
    private GameControls cont;

    public AudioManager Audio;

	// Use this for initialization
	void Start () {
        cont = GameControls.GetSelf();
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
            //Audio.MusicPlayer.Stop();
            //Audio.MusicPlayer.clip = Audio.Musics[cont.CurrentDay + 1];
            LOLSDK.Instance.StopSound(AudioManager.GetMscName(cont.CurrentDay));
            LOLSDK.Instance.PlaySound(AudioManager.GetMscName(cont.CurrentDay + 1), true, true);
            //Audio.MusicPlayer.Play();
        } else {
            cont.SpawnNPC();
        }
    }

    public void SetSpawnNPC2()
    {
        cont.SpawnNPC();
    }
}
