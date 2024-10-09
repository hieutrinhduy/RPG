using System.Collections;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem particleSystem;

    void Start()
    {
        // Start the coroutine to automatically pause the particle system
        StartCoroutine(PauseAfterTime(0.15f));
    }

    void Update()
    {
        // Press the "P" key to pause the Particle System
        if (Input.GetKeyDown(KeyCode.P))
        {
            particleSystem.Pause();
        }

        // Press the "R" key to resume the Particle System
        if (Input.GetKeyDown(KeyCode.R))
        {
            particleSystem.Play();
        }
    }

    private IEnumerator PauseAfterTime(float time)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(time);

        // Pause the Particle System
        particleSystem.Pause();
    }
}
