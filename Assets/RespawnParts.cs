using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RespawnParts : MonoBehaviour {

    private ParticleSystem partSys;
    private List<ParticleSystem.Particle> outParts;
    private int totalExitted;

	// Use this for initialization
	void Awake () {
        partSys = GetComponent<ParticleSystem>();
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

        partSys.Emit(totalExitted);

        Debug.Log("Emitting total new parts: " + totalExitted);

        partSys.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, outParts);
    }
}
