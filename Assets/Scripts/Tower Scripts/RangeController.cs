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
            GameObject enemyParent = other.gameObject;
            if (!tc.enemiesInRange.Contains(enemyParent))
            {
                tc.enemiesInRange.Add(enemyParent);
                tc.UpdateTarget();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Enemy")
        {
            GameObject enemyParent = other.gameObject;
            if (tc.enemiesInRange.Contains(enemyParent))
            {
                tc.enemiesInRange.Remove(enemyParent);
                if (tc.target == enemyParent)
                    tc.target = null;
                tc.UpdateTarget();
            }
        }
    }
}