using UnityEngine;
using System.Collections;

public class RadController : MonoBehaviour {

    public PartMan Parts;

    public SpriteRenderer mySprite;

    private float currentHeatUpRate;

	// Use this for initialization
	void Awake () {

        currentHeatUpRate = Parts.HeatUpRate;

	}
	
	// Update is called once per frame
	void Update () {
	
        if(currentHeatUpRate != Parts.HeatUpRate) {
            currentHeatUpRate = Parts.HeatUpRate;
            mySprite.color = new Color(
                1f, 0f, 0f,
                (Parts.MinHeatUpRate / Parts.HeatUpRate)
            );
            if(currentHeatUpRate >= Parts.MaxFramesPerHeatUp) {
                mySprite.color = new Color(1f, 0f, 0f, 0f);
            }
        }
        

	}
}
