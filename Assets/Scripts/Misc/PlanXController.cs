using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanXController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
            Destroy(other.gameObject);
    }
}
