using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoERadius : MonoBehaviour
{
    //Used to set parent vars in BulletController based on child trigger collider for AoE bullet effects
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !transform.parent.GetComponent<BulletController>().aoeTargets.Contains(other.gameObject))
            transform.parent.GetComponent<BulletController>().aoeTargets.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && transform.parent.GetComponent<BulletController>().aoeTargets.Contains(other.gameObject))
            transform.parent.GetComponent<BulletController>().aoeTargets.Remove(other.gameObject);
    }

    public void SetRadius(float radius)
    {
        GetComponent<SphereCollider>().radius = radius;
    }
}
