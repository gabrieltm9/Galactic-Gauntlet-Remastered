using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damage = 5; //How much damage it does on impact
    public int speed = 20; //How fast the bullet moves

    public void SetupBullet(int dmg)
    {
        damage = dmg;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    public void SetupBullet(int dmg, int speed)
    {
        damage = dmg;
        this.speed = speed;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}
