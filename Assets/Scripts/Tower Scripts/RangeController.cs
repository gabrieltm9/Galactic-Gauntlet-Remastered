using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : MonoBehaviour
{
    public TowerController tc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if(!tc.enemiesInRange.Contains(other.gameObject))
            {
                tc.enemiesInRange.Add(other.gameObject);
                tc.UpdateTarget();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Enemy")
        {
            if (!tc.enemiesInRange.Contains(other.gameObject))
            {
                tc.enemiesInRange.Remove(other.gameObject);
                tc.UpdateTarget();
            }
        }
    }
}
