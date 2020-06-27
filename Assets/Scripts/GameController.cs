using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int health = 20;
    public int money = 500;
    
    public MainUIController mUI;
    public TowerController selectedTower;

    public KeyCode sellTowerKey;

    public bool spawnEnemies;
    public float enemySpawnDelay; //How long to wait between enemy spawns
    public List<GameObject> enemyPrefabs;
    public Transform enemiesParent;

    public bool isPlacingTower;

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

    public void UpdateMoney(int m, bool add) //Add/remove money; add true = add, add false = remove
    {
        if (add)
            money += m;
        if (!add)
            money -= m;
        mUI.moneyTxt.text = "Money: $" + money;
    }
}
