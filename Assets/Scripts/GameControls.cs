using UnityEngine;
using System.Collections;

/// <summary>
/// Manages all the more specific classes
/// </summary>
public class GameControls : MonoBehaviour {

    public LidMovement Lid;
    public PartMan Parts;

    private int frameCt;

    public ParticleSystem Smoke;
    /// <summary>
    /// Amount that's deleted every evaporation frame.
    /// </summary>
    private int evapoMount;
    /// <summary>
    /// Speed at which the lid top decreases.
    /// </summary>
    public float evapoVolSpeed;

	// Use this for initialization
	void Start () {
        Lid.SetFillState(LidMovement.FillStates.Empty);
        Parts.ClearParts();
        evapoMount = (int)(evapoVolSpeed / Lid.MovIncrement) + 1;
    }
	
	// Update is called once per frame
	void Update () {

        frameCt++;

        if (Parts.IsBoiling && !Smoke.isPlaying) {
            Smoke.Play();
        }else if (!Parts.IsBoiling && Smoke.isPlaying) {
            Smoke.Stop();
        }
        if (Parts.IsBoiling) {
            if (frameCt % Parts.EvapoRate == 0) {
                //Incremental parts deletion
                Parts.DeleteParts(evapoMount);
                //Incremental volume lowering
                Lid.SmoothEmpty(evapoVolSpeed);
            }
        }
    }

    /// <summary>
    /// Iterates through state towards target fill state.
    /// Possible edge case includes that the state changes require a frame between them.
    /// </summary>
    /// <param name="TargetFillState"></param>
    public void SetCoffeeState(LidMovement.FillStates TargetFillState)
    {
        if (TargetFillState == Lid.CurrFill)
            return;
        if(TargetFillState < Lid.CurrFill) { 
            while (Lid.CurrFill != TargetFillState) {
                DecreaseCoffee();
            }
        }else {
            while (Lid.CurrFill != TargetFillState) {
                IncreaseCoffee();
            }
        }   
    }

    public void ClearCoffee()
    {
        Parts.ClearParts();
        Lid.EmptyAll();
    }

    public void IncreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Large)
            return;
        Parts.AddParts();
        Lid.FillSome();
    }

    public void DecreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Empty)
            return;
        Parts.DeleteParts(0);
        Lid.EmptySome( );
    }

    public void IncreaseHeat()
    {
        Parts.IncreaseRate();
    }

    public void DecreaseHeat()
    {
        Parts.DecreaseRate();
    }
}
