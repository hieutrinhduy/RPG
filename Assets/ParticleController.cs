using System.Collections;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private float time = 0.15f;
    public ParticleSystem particleSystem;

    void Start()
    {
        // Start the coroutine to automatically pause the particle system
        //StartCoroutine(PauseAfterTime(time));
    }

    private IEnumerator PauseAfterTime(float time)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(time);

        // Pause the Particle System
        particleSystem.Pause();
    }
    private void OnEnable()
    {
        StartCoroutine(PauseAfterTime(time));
    }
}
