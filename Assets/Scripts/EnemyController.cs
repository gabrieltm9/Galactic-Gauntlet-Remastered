using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health = 50;
    public int moneyDrop = 50; //How much money to drop when killed
    public int damage = 1; //How much health to remove from the player

    public bool isDying;

    private void Awake()
    {
        GetComponent<PathCreation.Examples.PathFollower>().pathCreator = GameObject.FindObjectOfType<PathCreation.PathCreator>();
    }

    void LateUpdate()
    {
        if (isDying)
            Die();
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
            PreDeath();
    }

    void PreDeath()
    {
        GameObject.FindObjectOfType<GameController>().money += moneyDrop;
        isDying = true;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            TakeDamage(other.GetComponent<BulletController>().bulletDmg);
            Destroy(other.gameObject);
        }
    }
}
