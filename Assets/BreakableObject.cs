using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyMe()
    {
        ParticleEffects.instance.PlayParticle(Particles.CRATE_BREAK, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
