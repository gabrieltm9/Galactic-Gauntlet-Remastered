using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactDetection : MonoBehaviour
{
    public BulletController bc;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Obstruction")
            bc.OnImpact(other.gameObject);
    }
}
