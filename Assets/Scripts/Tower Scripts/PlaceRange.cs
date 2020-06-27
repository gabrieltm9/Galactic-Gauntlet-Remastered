using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRange : MonoBehaviour
{
    public TowerController tc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlaceRange>() || other.tag == "Road")
            tc.UpdateObjectsInSpace(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlaceRange>() || other.tag == "Road")
            tc.UpdateObjectsInSpace(other.gameObject, false);
    }
}
