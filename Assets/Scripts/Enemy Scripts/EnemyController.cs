using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public PathCreation.Examples.PathFollower pf;

    public int id;
    public new string name;
    public int health = 50;
    public int moneyDrop = 50; //How much money to drop when killed
    public int damage = 1; //How much health to remove from the player

    public bool isDying;

    void Start()
    {
        pf = GetComponent<PathCreation.Examples.PathFollower>();
        pf.pathCreator = GameObject.FindObjectOfType<PathCreation.PathCreator>();
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
        GameObject.FindObjectOfType<GameController>().UpdateMoney(moneyDrop);
        isDying = true;
        DisableEnemyFunctions();
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void DisableEnemyFunctions() //Disables the enemy model, movement, etc
    {
        pf.enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
}