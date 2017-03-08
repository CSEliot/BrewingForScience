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
    private WaitForSeconds boilBuffer;

    #region Controls behavior
    [Tooltip("How many particles per volume of coffee")]
    public int PartsPerState;
    [Tooltip("Multiplicative speed applied to particles every DeltaHeatUpRate")]
    public float SpeedScale;
    [Tooltip("If all particles are above this point, boiling occurs")]
    public float BoilPoint;
    [Tooltip("# of frames that an evaporation will occur")]
    public float EvapoRate;
    [Tooltip("The change the heatup rate experiences on +/-'ing the heatup rate")]
    public float DeltaHeatUpRate;
    public float MaxSecondsPerHeatUp;
    public bool IsBoiling;
    public bool CanBoil;
    [Tooltip("Buffer time is how long at Max Temp before Boiling starts.")]
    public float BoilBufferTime;
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
    private ParticleSystem.MainModule mainPartSys;
    #endregion

    #region Private Pre-Boil Buffer Vars
    private bool isBuffered;
    #endregion

    #region Particle State Sampling Vars
    private float prevTime;
    private float sqrAvgSpd = 0f;
    private float lowestSpd = 0f;
    private float totalSpd = 0f;
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

        PartSys = GetComponent<ParticleSystem>();
        partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
        mainPartSys = PartSys.main;
        heatUpRate = MaxSecondsPerHeatUp;
        minHeatUpRate = MaxSecondsPerHeatUp - (DeltaHeatUpRate * (float)(heatUpStates));
        if (minHeatUpRate < 0) {
            CBUG.SrsError("MIN HEAT RATE TOO LOW, LOWER DELTA or RAISE MAXSECONDS: " + minHeatUpRate);
        }
        sqrAvgSpd = mainPartSys.startSpeed.constant * mainPartSys.startSpeed.constant;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsPaused)
            return;
        //&& heatUpRate < MaxSecondsPerHeatUp
        if (Time.time - prevTime >= (heatUpRate + HeatUpRateMod + HeatRateModStatic) ) {
            IncreaseSpeed();
            prevTime = Time.time;
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
        partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
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

    public void IncreaseSpeed()
    {
        //if (CurrSpeed + SpeedScale > MaxSpeed)
        //    return;
        //partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
        //partArrayLen = PartSys.GetParticles(partArray);
        UpdateAvgSpd();

        CanBoil = !(PartSys.main.maxParticles == 0);

        for (int x = 0; x < partArrayLen; x++)
        {
            if(partArray[x].velocity.sqrMagnitude > sqrAvgSpd)
            {
                partArray[x].velocity = new Vector3(
                    partArray[x].velocity.x / SpeedScale,
                    partArray[x].velocity.y / SpeedScale,
                    0f
                );
            }
            else
            {
                partArray[x].velocity = new Vector3(
                    partArray[x].velocity.x * SpeedScale,
                    partArray[x].velocity.y * SpeedScale,
                    0f
                );
            }
        }
        if (sqrAvgSpd < BoilPoint)
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
        partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
        partArrayLen = PartSys.GetParticles(partArray);
        totalSpd = 0;
        lowestSpd = 10000f;

        if(partArrayLen == 0) {
            sqrAvgSpd = 0;
            return;
        }

        for (int x = 0; x < partArrayLen; x++) {
            tempSpd = partArray[x].velocity.sqrMagnitude;
            totalSpd += tempSpd;
            if (lowestSpd > tempSpd)
                lowestSpd = tempSpd;
        }
        sqrAvgSpd = (totalSpd / partArrayLen) + targetSqrAvgSpd;
        //for (int x = 0; x < partArrayLen; x++) {
        //if (IsBoiling)
        //    partArray[x].lifetime = partArray[x].startLifetime / 2;
        //else {
        //    partArray[x].lifetime = partArray[x].startLifetime;
        //}
        //    //Lifetime used here to change sprite in particle.
        //}
        //PartSys.SetParticles(partArray, partArrayLen);
    }
    
    /// <summary>
    /// Cooling Down
    /// </summary>
    public void IncreaseRate()
    {
        if (heatUpRate > MaxSecondsPerHeatUp)
            return;
        heatUpRate += DeltaHeatUpRate;
        targetSqrAvgSpd--;
    }

    /// <summary>
    /// Heating Up
    /// </summary>
    public void DecreaseRate()
    {
        if (heatUpRate < minHeatUpRate)
            return;
        heatUpRate -= DeltaHeatUpRate;
        targetSqrAvgSpd++;
    }

    public void DecreaseSpeed()
    {
        partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
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
            return heatUpRate;
        }
    }

    public float MinHeatUpRate {
        get {
            return minHeatUpRate;
        }
    }

    public float SqrAvgSpd {
        get {
            return sqrAvgSpd;
        }
    }

    public void CalculateAvgSpeed()
    {
        partArray = new ParticleSystem.Particle[PartSys.main.maxParticles];
        partArrayLen = PartSys.GetParticles(partArray);
        //IsBoiling = !(PartSys.main.maxParticles == 0);
        totalSpd = 0;
        for (int x = 0; x < partArrayLen; x++) {
            totalSpd += partArray[x].velocity.sqrMagnitude;
        }
        //if (avgSpd < BoilPoint) {
        //    IsBoiling = false;
        //}
        sqrAvgSpd = totalSpd / partArrayLen;
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
