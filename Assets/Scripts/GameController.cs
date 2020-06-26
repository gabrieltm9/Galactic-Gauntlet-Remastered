using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int health = 20;
    public int money = 500;
    
    public TowerController selectedTower;

    public bool spawnEnemies;
    public float enemySpawnDelay; //How long to wait between enemy spawns
    public List<GameObject> enemyPrefabs;
    public Transform enemiesParent;

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    public void SelectTower(TowerController tc)
    {
        if(selectedTower != null)
            selectedTower.ToggleUI();
        selectedTower = tc;
    }

    IEnumerator SpawnEnemy()
    {
        while (spawnEnemies)
        {
            yield return new WaitForSeconds(enemySpawnDelay);
            Instantiate(enemyPrefabs[0], new Vector3(100, 0, 0), transform.rotation, enemiesParent);
        }
    }
}
