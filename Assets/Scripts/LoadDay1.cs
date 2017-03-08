using UnityEngine;
using LoLSDK;
using System.Collections;

public class LoadDay1 : MonoBehaviour {

    private int dayNum = -1;

    public GameObject DayPanel;
    public AudioManager MyAudio;

	// Use this for initialization
	void Awake () {
        dayNum++;
        DayPanel.SetActive(true);
        //MyAudio.MusicPlayer.Stop();
        //MyAudio.MusicPlayer.clip = MyAudio.Musics[0];
        //MyAudio.MusicPlayer.Play();
        LOLSDK.Instance.PlaySound(AudioManager.GetMscName(0), true, true);
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
