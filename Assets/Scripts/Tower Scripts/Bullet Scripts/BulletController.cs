using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject aoeChild; //The child gameobject that handles the projectile's aoe range

    public int type; //0 = normal bullet; 1 = grenade / explodes on impact
    public int damage = 5; //How much damage it does on impact
    public int speed = 20; //How fast the bullet moves

    public List<GameObject> aoeTargets;
    public float aoeRadius = 1;

    public bool isPrimed = false;

    public ParticleSystem impactParticles; //This particle system will be played on bullet impact

    public void SetupBullet(int type, int dmg, float aoeRadius, int speed) //Sets the bullet's stats, but doesnt prime it or set velocity
    {
        this.type = type;
        damage = dmg;
        this.speed = speed;
        this.aoeRadius = aoeRadius;

        if (aoeChild != null)
            aoeChild.GetComponent<AoERadius>().SetRadius(aoeRadius);
    }

    public void RunBullet()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        isPrimed = true;
    }

    public void OnImpact(GameObject collidedObj)
    {
        if(isPrimed)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            switch (type)
            {
                case 0: //Normal
                    NormalBulletImpact(collidedObj);
                    break;
                case 1: //Explosive
                    Explode();
                    break;
            }
        }
    }

    void NormalBulletImpact(GameObject obj)
    {
        if (obj.tag == "Enemy")
            obj.GetComponent<EnemyController>().TakeDamage(damage);
        Destroy(gameObject);
    }

    void Explode()
    {
        foreach(GameObject target in aoeTargets) //Damage all targets in aoe radius
        {
            if (target != null && target.GetComponent<EnemyController>())
                target.GetComponent<EnemyController>().TakeDamage(damage);
        }
        DisableBulletFunctions(); //Disable renderer, aoe child
        StartCoroutine(ImpactEffects());
    }

    IEnumerator ImpactEffects()
    {
        impactParticles.Play(); //Play impact particles
        yield return new WaitForSeconds(impactParticles.main.duration); //Wait for impact particles
        Destroy(gameObject); //Destroy bullet
    }

    void DisableBulletFunctions()
    {
        GetComponent<MeshRenderer>().enabled = false; //Disables bullet model
        GetComponent<MeshCollider>().enabled = false; //Disables bullet collider
        if (aoeChild != null)
            aoeChild.SetActive(false); //Disables AOE child (if this bullet has one)
        isPrimed = false; //Prevents any further damage code from running
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Obstruction")
            OnImpact(other.gameObject);
    }
}
