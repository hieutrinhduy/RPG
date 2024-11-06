using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Collider Collider;
    [SerializeField] private float DelayTime;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Collider = GetComponent<Collider>();
        yield return new WaitForSeconds(DelayTime);
        Collider.enabled = true;
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
