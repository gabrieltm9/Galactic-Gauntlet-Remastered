using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoERadius : MonoBehaviour
{
    //Used to set parent vars in BulletController based on child trigger collider for AoE bullet effects
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            switch (transform.parent.GetComponent<BulletController>().type)
            {
                case 1: //Explosive
                    transform.parent.GetComponent<BulletController>().aoeTargets.Add(other.gameObject);
                    break;
            }
        }
    }
}
