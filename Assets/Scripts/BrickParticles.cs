using UnityEngine;

public class BrickParticles : MonoBehaviour
{
    void Start()
    {
        var particle = GetComponent<ParticleSystem>();
        Destroy(gameObject, particle.main.duration);
    }
}
