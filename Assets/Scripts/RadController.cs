using UnityEngine;
using System.Collections;

public class RadController : MonoBehaviour {

    public PartMan Parts;

    public SpriteRenderer mySprite;

    private float currentHeatUpRate = 0.0f;
	
    void Start()
    {
        ResetRad();
    }

	// Update is called once per frame
	void Update () {
	
        //if(currentHeatUpRate != Parts.HeatUpRate) {
        //    currentHeatUpRate = Parts.HeatUpRate;
        //    mySprite.color = new Color(
        //        1f, 0f, 0f,
        //        (Parts.MinHeatUpRate/Parts.HeatUpRate )
        //    );
        //    if(currentHeatUpRate >= Parts.MaxSecondsPerHeatUp) {
        //        mySprite.color = new Color(1f, 0f, 0f, 0f);
        //    }
        //}
	}

    public void IncreaseRad()
    {
        if (currentHeatUpRate >= 1.0f)
            return;
        currentHeatUpRate += 0.20f;
        mySprite.color = new Color(
                1f, 0f, 0f,
                currentHeatUpRate
                );
    }

    public void DecreaseRad()
    {
        if (currentHeatUpRate <= 0.0f)
            return;
        currentHeatUpRate -= 0.20f;
        mySprite.color = new Color(
                1f, 0f, 0f,
                currentHeatUpRate
                );
    }

    public void ResetRad()
    {
        currentHeatUpRate = 0.0f;
        mySprite.color = new Color(
                1f, 0f, 0f,
                currentHeatUpRate
                );
    }
}
