using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All particle system controls for Brewing For Science game.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PartMan : MonoBehaviour {

    public LidMovement Lid;

    public int PartsPerState;
    public int StartSpeed;
    public float SpeedScale;
    public int MaxSpeed;
    public int MinSpeed;

    public ParticleSystem PartSys;
    private int partArrayLen;
    private ParticleSystem.Particle[][] partArrays;
    public float CurrSpeed;


    // Use this for initialization
    void Start () {
        PartSys = GetComponent<ParticleSystem>();
        partArrays = new ParticleSystem.Particle[PartSys.maxParticles][];
        PartSys.startSpeed = StartSpeed;
        CurrSpeed = StartSpeed;
    }
	
	// Update is called once per frame
	void Update () {
	
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
    public void DeleteParts()
    {
        deleteMultiple(PartsPerState);   
    }

    /// <summary>
    /// Helper function for DeletePart
    /// </summary>
    /// <param name="count">Amount of particles to delete.</param>
    private void deleteMultiple(int count)
    {
        partArrays = new ParticleSystem.Particle[PartSys.maxParticles];
        partArrayLen = PartSys.GetParticles(partArrays);
        //Edge Case: get told to delete more than exists.
        count = partArrayLen < count ? partArrayLen : count;
        for(int x = 0; x < count; x++) {
            //Yes, the same particle may be marked for deletion multiple times.
            //Unless I find a simple way to guarantee different random integers, this will suffice.
            partArrays[Random.Range(0, partArrayLen)].lifetime = 0;
            PartSys.maxParticles--; 
        }
        PartSys.SetParticles(partArrays, partArrayLen);
        if(PartSys.maxParticles <= 0) {
            PartSys.Clear();
        }
    }

    public void AddParts()
    {
        PartSys.maxParticles += PartsPerState;
        PartSys.Emit(PartsPerState);
    }

    public void IncreaseSpeed()
    {
        if (CurrSpeed + SpeedScale > MaxSpeed)
            return;

        partArrays = new ParticleSystem.Particle[PartSys.maxParticles];
        partArrayLen = PartSys.GetParticles(partArrays);
        CurrSpeed += SpeedScale;
        for (int x = 0; x < partArrayLen; x++) {
            //Yes, the same particle may be marked for deletion multiple times.
            //Unless I find a simple way to guarantee different random integers, this will suffice.
            Vector3 temp = partArrays[x].velocity.normalized;
            partArrays[x].velocity = new Vector3(temp.x *CurrSpeed, temp.y * CurrSpeed, -1f);
        }
        PartSys.SetParticles(partArrays, partArrayLen);
    }

    public void DecreaseSpeed()
    {
        if (CurrSpeed - SpeedScale < MinSpeed)
            return;

        partArrays = new ParticleSystem.Particle[PartSys.maxParticles];
        partArrayLen = PartSys.GetParticles(partArrays);
        CurrSpeed -= SpeedScale;
        for (int x = 0; x < partArrayLen; x++) {
            //Yes, the same particle may be marked for deletion multiple times.
            //Unless I find a simple way to guarantee different random integers, this will suffice.
            Vector3 temp = partArrays[x].velocity.normalized;
            partArrays[x].velocity = new Vector3(temp.x * CurrSpeed, temp.y * CurrSpeed, -1f);
        }
        PartSys.SetParticles(partArrays, partArrayLen);
    }
}
