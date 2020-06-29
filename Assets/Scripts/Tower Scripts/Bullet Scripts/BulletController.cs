using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int type; //0 = normal bullet; 1 = grenade / explodes on impact
    public int damage = 5; //How much damage it does on impact
    public int speed = 20; //How fast the bullet moves

    public List<GameObject> aoeTargets;
    public float aoeRadius = 4;

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

    public void OnImpact(GameObject collidedObj)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        switch (type)
        {
            case 0: //Normal
                NormalBulletImpact(collidedObj);
                break;
            case 1: //Explosive
                StartCoroutine(Explode());
                break;
        }
    }

    void NormalBulletImpact(GameObject obj)
    {
        if (obj.tag == "Enemy")
            obj.GetComponent<EnemyController>().TakeDamage(damage);
        Destroy(gameObject);
    }

    IEnumerator Explode()
    {
        foreach(GameObject target in aoeTargets)
        {
            if (target.tag == "Enemy")
                target.GetComponent<EnemyController>().TakeDamage(damage);
        }

        //Explosion Animation
        yield return new WaitForSeconds(0.1f); //Wait for anim
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Obstruction")
            OnImpact(other.gameObject);
    }
}
