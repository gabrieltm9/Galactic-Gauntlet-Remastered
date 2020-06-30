using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject aoeChild; //The child gameobject that handles the projectile's aoe range

    public int damage = 0; //How much damage it does on impact
    public int speed = 0; //How fast the bullet moves

    public bool explosive;
    public List<GameObject> aoeTargets;
    public float aoeRadius = 0; //The radius affected by the bullet's aoe effects. If 0, aoe effects are disabled

    public bool isPrimed = false;

    public GameObject impactParticlesParent; //The gameobject that holds particle systems that can be played on bullet impact; Chosen in SetupBullet and others are destroyed
    public ParticleSystem impactParticleSystem = null; //This particle system will be played on bullet impact

    public GameObject modelsParent; //The gameobject that holds potential bullet models; Model is chosen in SetupBullet and others are destroyed
    public GameObject modelChild;

    public void SetupBullet(TowerData td) //Sets the bullet's stats, but doesnt prime it or set velocity
    {
        BulletData bd = td.bulletData;
        damage = td.damage;
        speed = bd.bulletSpeed;
        explosive = bd.explosive;
        aoeRadius = bd.aoeRadius;

        //Set model, destroy other model options
        modelChild = modelsParent.transform.GetChild(bd.model).gameObject;

        //If this bullet has an AoE effect, set the aoe radius. Otherwise, destroy aoe child
        if (aoeRadius == 0)
            Destroy(aoeChild);
        else
            aoeChild.GetComponent<AoERadius>().SetRadius(aoeRadius);

        //Set impact particles
        if(bd.overrideImpactParticles < 0) //Try to detect proper impact particles
        {
            if (bd.explosive)
                impactParticleSystem = impactParticlesParent.transform.GetChild(0).GetComponent<ParticleSystem>(); //Medium explosion
            else
                impactParticleSystem = null; //No / default impact particles
        }
        else if(bd.overrideImpactParticles > 0) //Override particles with options from list
            impactParticleSystem = impactParticlesParent.transform.GetChild(bd.overrideImpactParticles - 1).GetComponent<ParticleSystem>(); //-1 to override int because 0 = no impact, so list starts at 1

        DestroyUnusedChildren();
    }

    public void RunBullet()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        isPrimed = true;
    }

    //Called from ImpactDetection
    public void OnImpact(GameObject collidedObj)
    {
        if(isPrimed)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (explosive)
                Explode();
            else
                NormalBulletImpact(collidedObj);
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
        impactParticleSystem.Play(); //Play impact particles
        yield return new WaitForSeconds(impactParticleSystem.main.duration); //Wait for impact particles
        Destroy(gameObject); //Destroy bullet
    }

    void DisableBulletFunctions()
    {
        modelChild.SetActive(false);
        if (aoeChild != null)
            aoeChild.SetActive(false); //Disables AOE child (if this bullet has one)
        isPrimed = false; //Prevents any further damage code from running
    }

    void DestroyUnusedChildren() //Will destroy unused model & particle system children
    {
        //Destroy ununsed bullet models
        foreach (Transform child in modelsParent.transform)
        {
            if (child.gameObject != modelChild)
                Destroy(child.gameObject);
        }
        if (modelChild == null)
            Destroy(modelsParent);

        //Destroy unused impact particle systems
        foreach (Transform child in impactParticlesParent.transform)
        {
            if (child.GetComponent<ParticleSystem>() != impactParticleSystem)
                Destroy(child.gameObject);
        }
        if (impactParticleSystem == null)
            Destroy(impactParticlesParent);
    }
}
