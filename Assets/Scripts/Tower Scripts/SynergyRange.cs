using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyRange : MonoBehaviour
{
    public TowerController tc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SynergyRange>())
            tc.UpdateSynergyList(other.GetComponent<SynergyRange>().tc.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<SynergyRange>())
            tc.UpdateSynergyList(other.GetComponent<SynergyRange>().tc.gameObject, false);
    }
}
