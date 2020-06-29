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

    public List<GameObject> towerPrefabs; //0 = machine gun
    public Transform towersParent;
    public List<GameObject> enemyPrefabs;
    public Transform enemiesParent;
    public Transform generatedPrefabsParent;

    public bool isPlacingTower;

    //Wave vars
    public List<Enemy> enemiesToSpawn = new List<Enemy>();

    private void Awake()
    {
        GeneratePrefabs();
        //GenerateSampleXMLs();
    }

    private void Start()
    {
        LoadWave("test");
        StartCoroutine(RunWave());
    }

    void GeneratePrefabs() //Spawns all tower and enemy prefabs so Awake methods in their scripts can set variables, then replaces their existing prefabs with these spawned versions
    {
        //Generate Towers
        for(int i = 0; i < towerPrefabs.Count; i++)
        {
            GameObject newTower = Instantiate(towerPrefabs[i], new Vector3(0, 0, -15), transform.rotation, generatedPrefabsParent);
            towerPrefabs.Remove(towerPrefabs[i]);
            towerPrefabs.Add(newTower);
        }
        generatedPrefabsParent.gameObject.SetActive(false);
    }

    public void SelectTower(TowerController tc)
    {
        if(selectedTower != null)
            selectedTower.ToggleUI();
        selectedTower = tc;
        mUI.EnableUpgradeMenu(tc);
    }

    public void DeselectTower(TowerController tc)
    {
        selectedTower = null;
        mUI.DisableUpgradeMenu();
    }

    public void UpgradeTower()
    {
        TowerController tc = selectedTower;
        TowerData td = tc.td;
        if(money >= td.upgradeLevels[tc.level - 1].cost && td.upgradeLevels.Length >= tc.level) //If player can afford upgrade
        {
            tc.UpdateData(td.upgradeLevels[tc.level - 1]);
            tc.level++;
            mUI.UpdateUpgradeUI(tc);
        }
    }

    IEnumerator InfiniteWave()
    {
        while (spawnEnemies)
        {
            yield return new WaitForSeconds(enemySpawnDelay);
            Instantiate(enemyPrefabs[0], new Vector3(100, 0, 0), transform.rotation, enemiesParent);
        }
    }

    IEnumerator RunWave()
    {
        if (spawnEnemies) //If spawning is enabled (change to while to make looping waves)s
        {
            int enemyIndexToSpawn = 0; //The index in enemiesToSpawn that will be targetted
            while(enemyIndexToSpawn < enemiesToSpawn.Count) //While there are more enemy types to be spawned
            {
                int currentEnemyCount = 0; //How many of the current enemy type has been spawned
                while(currentEnemyCount < enemiesToSpawn[enemyIndexToSpawn].count) //While there are more of this enemy type to spawn
                {
                    yield return new WaitForSeconds(enemySpawnDelay); //Wait the spawn delay
                    Instantiate(enemyPrefabs[enemiesToSpawn[enemyIndexToSpawn].id], new Vector3(100, 0, 0), transform.rotation, enemiesParent); //Spawn enemy
                    currentEnemyCount++;
                }
                enemyIndexToSpawn++;
            }
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

    // --- Wave Systems ---

    void LoadWave(string xml)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>("Waves/" + xml);
        Wave w = XMLOp.DeserializeXMLTextAsset<Wave>(xmlAsset);

        // \/ Line below will load xml from path instead
        //Wave w = XMLOp.Deserialize<Wave>("D:\\Game Stuff\\Game Making\\GGRemastered\\Galactic-Gauntlet-Remastered\\Assets\\Resources\\Waves\\test.xml");

        foreach (Enemy e in w.Enemies)
            enemiesToSpawn.Add(e);
    }

    void GenerateSampleXMLs()
    {
        GenerateSampleWave(Application.dataPath + "\\SampleXMLs\\Wave.xml");
        GenerateSampleTowerData(Application.dataPath + "\\SampleXMLs\\TowerData.xml");
    }

    void GenerateSampleWave(string outPath) //Makes a basic wave xml file which can be edited to make new waves
    {
        Wave w = new Wave();
        w.Enemies = new Enemy[enemyPrefabs.Count];
        int index = 0;
        foreach(GameObject enemyPrefab in enemyPrefabs)
        {
            EnemyController ec = enemyPrefab.GetComponent<EnemyController>();
            Enemy e = new Enemy(ec.id, ec.name, 0, index + 1);
            w.Enemies[index] = e;
            index++;
        }
        XMLOp.Serialize(w, outPath);
    }

    void GenerateSampleTowerData(string outPath)
    {
        TowerData td = new TowerData();
        td.upgradeLevels = new UpgradeLevel[3];
        for(int i = 0; i < 3; i++)
            td.upgradeLevels[i] = new UpgradeLevel();
        XMLOp.Serialize(td, outPath);
    }
}