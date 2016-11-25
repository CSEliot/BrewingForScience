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
    
    [Tooltip("Amount that's deleted every evaporation frame")]
    public int EvapoMount;
    /// <summary>
    /// Speed at which the lid top decreases.
    /// </summary>
    public float EvapoVolSpeed;
    /// <summary>
    /// More liquid in the cup means it takes longer to heat up.
    /// </summary>
    public float HeatUpVolMod;

	// Use this for initialization
	void Start () {
        Lid.SetFillState(LidMovement.FillStates.Empty);
        Parts.ClearParts();
        //EvapoMount = (int)(/*evapoVolSpeed / Lid.MovIncrement) + 1*/;
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
                Parts.DeleteParts(EvapoMount);
                //Incremental volume lowering
                if (Lid.SmoothEmpty(EvapoVolSpeed)) {
                    Parts.IsBoiling = false;
                }
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
        Parts.IsBoiling = false;
        Parts.HeatUpRateMod += HeatUpVolMod;
        Lid.FillSome();
    }

    public void DecreaseCoffee()
    {
        if (Lid.CurrFill == LidMovement.FillStates.Empty)
            return;
        Parts.DeleteParts(0);
        Parts.HeatUpRateMod -= HeatUpVolMod;
        if(Lid.EmptySome()){
            Parts.IsBoiling = false;
        }
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
