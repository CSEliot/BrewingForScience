using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RespawnParts : MonoBehaviour {

    private ParticleSystem partSys;
    private List<ParticleSystem.Particle> exitParts;
    private int totalExitted;
    private PartMan parts;
    public Transform RespawnLoc;

    float speedVal;

	// Use this for initialization
	void Awake () {
        totalExitted = 0;
        partSys = GetComponent<ParticleSystem>();
        parts = GetComponent<PartMan>();
        exitParts = new List<ParticleSystem.Particle>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnParticleTrigger()
    {
        totalExitted = partSys.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, exitParts);

        if (totalExitted == 0)
            return;

        speedVal = Mathf.Sqrt(parts.SqrHighestSpd);
        for (int x = 0; x < totalExitted; x++)
        {
            ParticleSystem.Particle p = exitParts[x];
            p.position = RespawnLoc.position;
            p.velocity = new Vector3(speedVal, -speedVal, 0f);
            exitParts[x] = p;
        }

        ////var pt = new ParticleSystem.EmitParams();
        ////pt.velocity = new Vector3(xVal, -xVal - 0.1f, 0f);
        ////partSys.Emit(pt, totalExitted + 1); //Crashes on this line. 
        ////ParticleSystem.Particle part = new ParticleSystem.Particle();
        ////part.velocity = new Vector3(xVal, -xVal - 0.1f, 0f);
        ////partSys.Emit(part);
        partSys.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, exitParts);
        CBUG.Log("Total Current Parts: " + partSys.particleCount);
    }
}
