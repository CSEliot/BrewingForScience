using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All particle system controls for Brewing For Science game.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PartMan : MonoBehaviour
{

    public LidMovement Lid;

    #region Controls behavior
    [Tooltip("How many particles per volume of coffee")]
    public int PartsPerState;
    [Tooltip("Starting speed of particles")]
    public int StartSpeed;
    [Tooltip("Multiplicative speed applied to particles every DeltaHeatUpRate")]
    public float SpeedScale;
    [Tooltip("If all particles are above this point, boiling occurs")]
    public float BoilPoint;
    [Tooltip("# of frames that an evaporation will occur")]
    public float EvapoRate;
    [Tooltip("The change the heatup rate experiences on +/-'ing the heatup rate")]
    public float DeltaHeatUpRate;
    public float MaxFramesPerHeatUp;
    public bool IsBoiling;
    [Tooltip("Alters heatup rate based on volume of liquid")]
    public float HeatUpRateMod;
    #endregion

    #region Per-particle management
    public ParticleSystem PartSys;
    private int partArrayLen;
    private ParticleSystem.Particle[] partArray;
    private float heatUpRate;
    private float minHeatUpRate;
    private const int heatUpStates = 5;
    #endregion

    #region Test VARS
    private float frameCt;
    private float avgSpd = 0f;
    private float totalSpd = 0f;
    private float totalSamples = 0f;
    #endregion
    
    // Use this for initialization
    void Start()
    {
        PartSys = GetComponent<ParticleSystem>();
        partArray = new ParticleSystem.Particle[PartSys.maxParticles];
        PartSys.startSpeed = StartSpeed;
        heatUpRate = MaxFramesPerHeatUp + 1;
        minHeatUpRate = MaxFramesPerHeatUp - (DeltaHeatUpRate * heatUpStates);
        if(minHeatUpRate <= 0) {
            CBUG.SrsError("MIN HEAT RATE TOO LOW, LOWER DELTA");
        }
    }

    // Update is called once per frame
    void Update()
    {
        frameCt++;

        if(frameCt % (heatUpRate + HeatUpRateMod) == 0 && heatUpRate < MaxFramesPerHeatUp) {
            IncreaseSpeed();
        }

        //if (frameCt % 3 == 0) {
        //    partArray = new ParticleSystem.Particle[PartSys.maxParticles];
        //    partArrayLen = PartSys.GetParticles(partArray);
        //    if (partArrayLen <= 0)
        //        return;
        //    totalSpd += partArray[Random.Range(0, partArrayLen)].velocity.magnitude;
        //    totalSamples++;
        //    avgSpd = totalSpd / totalSamples;
        //    CBUG.Do("Spd AVG: " + avgSpd);
        //}
    }

    /// <summary>
    /// Delete all particles from the system.
    /// </summary>
    public void ClearParts()
    {
        PartSys.Clear();
        PartSys.maxParticles = 0;
    }

    /// <summary>
    /// Delete PartsToDelete particles from the system.
    /// </summary>
    public void DeleteParts(int amt)
    {
        int tempAmt = amt <= 0 ? PartsPerState : amt;
        deleteMultiple(tempAmt);
    }

    /// <summary>
    /// Helper function for DeletePart
    /// </summary>
    /// <param name="count">Amount of particles to delete.</param>
    private void deleteMultiple(int count)
    {
        partArray = new ParticleSystem.Particle[PartSys.maxParticles];
        partArrayLen = PartSys.GetParticles(partArray);
        //Edge Case: get told to delete more than exists.
        count = partArrayLen < count ? partArrayLen : count;
        for (int x = 0; x < count; x++) {
            //Yes, the same particle may be marked for deletion multiple times.
            //Unless I find a simple way to guarantee different random integers, this will suffice.
            partArray[Random.Range(0, partArrayLen)].lifetime = 0;
            PartSys.maxParticles--;
        }
        PartSys.SetParticles(partArray, partArrayLen);
        if (PartSys.maxParticles <= 0) {
            PartSys.Clear();
        }
    }

    public void AddParts()
    {
        int tempAmt = PartSys.maxParticles - (PartSys.maxParticles / PartsPerState) * PartsPerState;
        tempAmt = PartsPerState - tempAmt;
        PartSys.maxParticles += tempAmt;
        PartSys.Emit(tempAmt);
    }

    public void IncreaseSpeed()
    {
        //if (CurrSpeed + SpeedScale > MaxSpeed)
        //    return;

        partArray = new ParticleSystem.Particle[PartSys.maxParticles];
        partArrayLen = PartSys.GetParticles(partArray);
        IsBoiling = !(PartSys.maxParticles == 0);
        totalSpd = 0;
        for (int x = 0; x < partArrayLen; x++) {
            if (partArray[x].velocity.sqrMagnitude < BoilPoint) {
                IsBoiling = false;
            }
            partArray[x].velocity = new Vector3(
                partArray[x].velocity.x * SpeedScale,
                partArray[x].velocity.y * SpeedScale,
                0f 
            );
            totalSpd += partArray[x].velocity.sqrMagnitude;
        }
        avgSpd = totalSpd / partArrayLen;
        //for (int x = 0; x < partArrayLen; x++) {
            //if (IsBoiling)
            //    partArray[x].lifetime = partArray[x].startLifetime / 2;
            //else {
            //    partArray[x].lifetime = partArray[x].startLifetime;
            //}
        //    //Lifetime used here to change sprite in particle.
        //}
        PartSys.SetParticles(partArray, partArrayLen);
    }

    public void IncreaseRate()
    {
        if (heatUpRate < minHeatUpRate)
            return;
        heatUpRate -= DeltaHeatUpRate; 
    }

    public void DecreaseRate()
    {
        if (heatUpRate > MaxFramesPerHeatUp)
            return;
        heatUpRate += DeltaHeatUpRate;
    }

    public void DecreaseSpeed()
    {
        partArray = new ParticleSystem.Particle[PartSys.maxParticles];
        partArrayLen = PartSys.GetParticles(partArray);
        for (int x = 0; x < partArrayLen; x++) {
            partArray[x].velocity = new Vector3(
                partArray[x].velocity.x / SpeedScale,
                partArray[x].velocity.y / SpeedScale,
                0f
            );  
        }
        PartSys.SetParticles(partArray, partArrayLen);
    }

    public float HeatUpRate {
        get {
            return heatUpRate;
        }
    }

    public float MinHeatUpRate {
        get {
            return minHeatUpRate;
        }
    }
}
