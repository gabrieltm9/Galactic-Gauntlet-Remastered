using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int bulletDmg = 5;
    public float bulletSpeed = 3f; //How fast the bullet moves
    
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
    }
}
