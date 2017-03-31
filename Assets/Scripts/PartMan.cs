using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// All particle system controls for Brewing For Science game.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PartMan : MonoBehaviour
{

    public LidMovement Lid;
    public TempReader _TempReader;
    private WaitForSeconds boilBuffer;


    #region Controls behavior
    [Tooltip("How many particles per volume of coffee")]
    public int PartsPerState;
    [Tooltip("Multiplicative speed applied to particles every DeltaHeatUpRate")]
    public float SpeedScale;
    [Tooltip("If all particles are above this point, boiling occurs")]
    public float SqrBoilPoint;
    public float SqrFreezePoint;
    [Tooltip("# of frames that an evaporation will occur")]
    public float EvaporateWaitTime;
    public float HeatUpMaxWaitTime;
    public bool HeatUpEnabled;
    public bool IsBoiling;
    public bool CanBoil;
    [Tooltip("Buffer time is how long at Max Temp before Boiling starts.")]
    public float BoilBufferTime;
    [Tooltip("Alters heatup rate.")]
    public float HeatChangeWaitTime;
    [Tooltip("Alters cooldown rate.")]
    public float CoolDownRateMod;
    #endregion

    #region Per-particle management
    public ParticleSystem PartSys;
    [Tooltip("The change the heatup rate experiences on +/-'ing the heatup rate")]
    private float deltaHeatUpWaitTime;
    private int partArrayLen;
    private ParticleSystem.Particle[] partArray;
    private float heatUpWaitTime;
    private float minHeatUpWaitTime;
    private const int heatUpStates = 5;
    private ParticleSystem.MainModule mainPartSys;
    public float SqrHighestSpd;
    #endregion

    #region Private Pre-Boil Buffer Vars
    private bool isBuffered;
    #endregion

    #region Particle State Sampling Vars
    private float prevTime;
    private float sqrAvgSpd = 0f;
    private float sqrLowestSpd = 0f;
    private float sqrTotalSpd = 0f;
    private float totalSamples = 0f;
    private float tempSpd = 0f;
    #endregion

    public bool IsPaused;

    //Testing ...
    private int targetSqrAvgSpd;
    public float HeatRateModStatic; //Static is a word which here means, unchanged by code.

    // Use this for initialization
    void Start()
    {
        CanBoil = false;
        boilBuffer = new WaitForSeconds(BoilBufferTime);

        targetSqrAvgSpd = 0;

        IsPaused = false;

        HeatUpEnabled = false;

        PartSys = GetComponent<ParticleSystem>();
        partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
        mainPartSys = PartSys.main;
        heatUpWaitTime = HeatUpMaxWaitTime;
        deltaHeatUpWaitTime = HeatUpMaxWaitTime / heatUpStates;
        minHeatUpWaitTime = HeatUpMaxWaitTime - (deltaHeatUpWaitTime * (float)(heatUpStates));
        if (minHeatUpWaitTime < 0) {
            CBUG.SrsError("MIN HEAT RATE TOO LOW, LOWER DELTA or RAISE MAXSECONDS: " + minHeatUpWaitTime);
        }
        sqrAvgSpd = mainPartSys.startSpeed.constant * mainPartSys.startSpeed.constant;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsPaused)
            return;

        if (HeatUpEnabled)
        {
            //&& heatUpRate < MaxSecondsPerHeatUp
            if (Time.time - prevTime >= (heatUpWaitTime + HeatChangeWaitTime + HeatRateModStatic) ) {
                IncreaseHeat();
                prevTime = Time.time;
            }

        }
        else
        {
            if (Time.time - prevTime >= (CoolDownRateMod + HeatChangeWaitTime))
            {
                DecreaseHeat();
                prevTime = Time.time;
            }
        }


        if (CanBoil && !isBuffered && !IsBoiling)
        {
            StartCoroutine(StartBoilBuffer());
        }

        //if (frameCt % 3 == 0) {
        //    partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
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
    }

    /// <summary>
    /// Delete PartsToDelete particles from the system.
    /// </summary>
    public void DeleteParts(int amt)
    {
        int tempAmt = amt <= 0 ? PartsPerState - 1 : amt;
        deleteMultiple(tempAmt);
    }

    /// <summary>
    /// Helper function for DeletePart
    /// </summary>
    /// <param name="count">Amount of particles to delete.</param>
    private void deleteMultiple(int count)
    {
        partArrayLen = PartSys.GetParticles(partArray);
        //Edge Case: get told to delete more than exists.
        count = partArrayLen < count ? partArrayLen : count;
        for (int x = 0; x < count; x++) {
            //Yes, the same particle may be marked for deletion multiple times.
            //Unless I find a simple way to guarantee different random integers, this will suffice.
            partArray[Random.Range(0, partArrayLen)].remainingLifetime = 0;
        }
        PartSys.SetParticles(partArray, partArrayLen);
        if (PartSys.main.maxParticles <= 0) {
            PartSys.Clear();
        }
    }

    public void AddParts()
    {
        int tempAmt = PartSys.main.maxParticles - (PartSys.main.maxParticles / PartsPerState) * PartsPerState;
        tempAmt = PartsPerState - tempAmt;
        PartSys.Emit(tempAmt);
    }

    public void FixParts()
    {

    }

    public void IncreaseHeat()
    {
        partArrayLen = PartSys.GetParticles(partArray);
        UpdateAvgSpd();

        CanBoil = true;

        for (int x = 0; x < partArrayLen; x++)
        {
            Vector3 tempV = Vector3.Scale(partArray[x].velocity, new Vector3(SpeedScale, SpeedScale));
            if(tempV.sqrMagnitude < SqrBoilPoint + 1f)
                partArray[x].velocity = tempV;
        }

        if (sqrAvgSpd < SqrBoilPoint)
        {
            CanBoil = false;
        }
        PartSys.SetParticles(partArray, partArrayLen);
    }

    public void DecreaseHeat()
    {
        //if (CurrSpeed + SpeedScale > MaxSpeed)
        //    return;
        //partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
        partArrayLen = PartSys.GetParticles(partArray);
        UpdateAvgSpd();

        if (sqrAvgSpd <= SqrFreezePoint)
            return;

        CanBoil = true;

        for (int x = 0; x < partArrayLen; x++)
        {
            partArray[x].velocity.Scale(Vector3.one / SpeedScale);
        }

        if (sqrAvgSpd < SqrBoilPoint)
        {
            CanBoil = false;
        }



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

    public void UpdateAvgSpd()
    {
        partArrayLen = PartSys.GetParticles(partArray);
        sqrTotalSpd = 0;
        sqrLowestSpd = 10000f;
        SqrHighestSpd = 0f;

        if(partArrayLen == 0) {
            sqrAvgSpd = 0;
            return;
        }

        for (int x = 0; x < partArrayLen; x++) {
            tempSpd = partArray[x].velocity.sqrMagnitude;
            sqrTotalSpd += tempSpd;
            if (sqrLowestSpd > tempSpd)
                sqrLowestSpd = tempSpd;
            if (SqrHighestSpd < tempSpd)
                SqrHighestSpd = tempSpd;
        }
        sqrAvgSpd = (sqrTotalSpd / partArrayLen);
        _TempReader.SetReadingTemp(sqrAvgSpd);
        
    }
    
    /// <summary>
    /// Cooling Down
    /// </summary>
    public void SlowdownHeatup()
    {
        if (heatUpWaitTime > HeatUpMaxWaitTime)
            return;
        heatUpWaitTime += deltaHeatUpWaitTime;
        targetSqrAvgSpd--;
        if (targetSqrAvgSpd == 0)
        {
            targetSqrAvgSpd = -1;
        }

    }

    /// <summary>
    /// Heating Up
    /// </summary>
    public void SpeedupHeatup()
    {
        if (heatUpWaitTime < minHeatUpWaitTime)
            return;
        heatUpWaitTime -= deltaHeatUpWaitTime;
        if(targetSqrAvgSpd == -1)
        {
            targetSqrAvgSpd = 0;
        }
        targetSqrAvgSpd++;
    }

    public void DecreaseSpeed()
    {
        partArrayLen = PartSys.GetParticles(partArray);
        for (int x = 0; x < partArrayLen; x++)
        {
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
            return heatUpWaitTime;
        }
    }

    public float MinHeatUpRate {
        get {
            return minHeatUpWaitTime;
        }
    }

    public float SqrAvgSpd {
        get {
            return sqrAvgSpd;
        }
    }

    public void CalculateAvgSpeed()
    {
        partArrayLen = PartSys.GetParticles(partArray);
        //IsBoiling = !(PartSys.main.maxParticles == 0);
        sqrTotalSpd = 0;
        for (int x = 0; x < partArrayLen; x++) {
            sqrTotalSpd += partArray[x].velocity.sqrMagnitude;
        }
        //if (avgSpd < BoilPoint) {
        //    IsBoiling = false;
        //}
        sqrAvgSpd = sqrTotalSpd / partArrayLen;
    }

    #region Helper Functions
    private IEnumerator StartBoilBuffer()
    {
        CBUG.Do("STARTING BOIL!!");
        isBuffered = true;
        yield return boilBuffer;
        if (CanBoil && !GameControls.IsQuizTime())
        {
            IsBoiling = true;
            Tips.Spawn(0);
        }
        isBuffered = false;
    }
    #endregion
}
