using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RespawnParts : MonoBehaviour {

    private ParticleSystem partSys;
    private List<ParticleSystem.Particle> outParts;
    private int totalExitted;
    private PartMan parts;

    float xVal;

	// Use this for initialization
	void Awake () {
        partSys = GetComponent<ParticleSystem>();
        parts = GetComponent<PartMan>();
        outParts = new List<ParticleSystem.Particle>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnParticleTrigger()
    {
        totalExitted = partSys.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, outParts);

        if (totalExitted == 0)
            return;

        for(int x = 0; x < totalExitted; x++) {
            ParticleSystem.Particle p = new ParticleSystem.Particle();
            p.remainingLifetime = -1;
            outParts[x] = p;
        }
        
        ParticleSystem.EmitParams pt = new ParticleSystem.EmitParams();
        xVal = parts.SqrAvgSpd / 2.82842712f;
        pt.velocity = new Vector3(xVal, -xVal - 0.1f, 0f);
        partSys.Emit(pt, totalExitted + 2);

        //Debug.Log("Emitting total new parts: " + totalExitted);

        partSys.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, outParts);
    }
}
