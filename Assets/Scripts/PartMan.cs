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
    public int PartsPerState;
    public int StartSpeed;
    public float SpeedScale;
    public float BoilPoint;
    public float EvapoRate;
    public float DeltaHeatUpRate;
    public float MaxHeatUpRate;
    public float MinHeatUpRate;
    #endregion

    #region Per-particle management
    public ParticleSystem PartSys;
    private int partArrayLen;
    private ParticleSystem.Particle[] partArray;
    private bool isBoiling;
    private float heatUpRate;
    private float startingHeatUpRate;
    public float HeatUpRateMod;
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
        startingHeatUpRate = MaxHeatUpRate;
        heatUpRate = startingHeatUpRate;

    }

    // Update is called once per frame
    void Update()
    {
        frameCt++;

        if(frameCt % (heatUpRate + HeatUpRateMod) == 0 && heatUpRate != MaxHeatUpRate) {
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
        isBoiling = PartSys.maxParticles != 0;
        for (int x = 0; x < partArrayLen; x++) {
            if (partArray[x].velocity.sqrMagnitude < BoilPoint) {
                isBoiling = false;
            }
            partArray[x].velocity = new Vector3(
                partArray[x].velocity.x * SpeedScale,
                partArray[x].velocity.y * SpeedScale,
                0f
            );
        }
        for (int x = 0; x < partArrayLen; x++) {
            if (isBoiling)
                partArray[x].lifetime = partArray[x].startLifetime / 2;
            else {
                partArray[x].lifetime = partArray[x].startLifetime;
            }
            //Lifetime used here to change sprite in particle.
        }
        PartSys.SetParticles(partArray, partArrayLen);
    }

    public void IncreaseRate()
    {
        if (heatUpRate - DeltaHeatUpRate < MinHeatUpRate)
            return;
        heatUpRate -= DeltaHeatUpRate; 
    }

    public void DecreaseRate()
    {
        if (heatUpRate + DeltaHeatUpRate > MaxHeatUpRate)
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

    public bool IsBoiling {
        get {
            return isBoiling;
        }
    }

    public float HeatUpRate {
        get {
            return heatUpRate;
        }
    }
}
