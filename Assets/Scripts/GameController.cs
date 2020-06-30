using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int health = 20;
    public int money = 500;
    
    public MainUIController mUI;
    public TowerController selectedTower;

    public KeyCode sellTowerKey;

    public Vector3 levelID; //x = planet, y = level, z = wave
    public int maxWaves = 20; //How many waves need to be cleared to consider the level "beaten"; This int will likely be changed at runtime in the future instead of being hardcoded
    public bool spawnEnemies;
    public bool runWaveSystem;
    public bool isSpawningWave;
    public float enemySpawnDelay; //How long to wait between enemy spawns

    public List<GameObject> towerPrefabs; //0 = machine gun
    public Transform towersParent;
    public List<GameObject> enemyPrefabs;
    public Transform enemiesParent;
    public GameObject bulletPrefab; //Is modified at runtime per tower to fit the TowerData's BulletData
    public Transform generatedPrefabsParent;

    public bool isPlacingTower;

    //Wave vars
    public List<Enemy> enemiesToSpawn = new List<Enemy>();

    private void Start()
    {
#if UNITY_EDITOR
        GenerateSampleXMLs();
#endif
        GeneratePrefabs();
        SetupWaveSystem(levelID); //Eventually, this levelID parameter will likely be passed from a level select
    }

    private void Update()
    {
        if (runWaveSystem)
            WaveController();

        GCode();
    }

    void GeneratePrefabs() //Spawns all tower and enemy prefabs so Awake methods in their scripts can set variables, then replaces their existing prefabs with these spawned versions
    {
        //Generate Towers
        List<GameObject> tempTowers = new List<GameObject>();
        for (int i = 0; i < towerPrefabs.Count; i++)
        {
            GameObject newTower = Instantiate(towerPrefabs[i], new Vector3(0, 0, -15), transform.rotation, generatedPrefabsParent);
            newTower.GetComponent<TowerController>().SetupTower();
            tempTowers.Add(newTower);
        }
        towerPrefabs = tempTowers;
    }

    public void UpdateTowerPrice(int id, int price)
    {
        mUI.towerPrices[id].text = "$" + price;
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
        if(td.upgradeLevels.Length >= tc.level && money >= td.upgradeLevels[tc.level - 1].cost) //If player can afford upgrade
        {
            UpdateMoney(-td.upgradeLevels[tc.level - 1].cost);
            tc.UpdateData(td.upgradeLevels[tc.level - 1]);
            tc.level++;
            mUI.UpdateUpgradeUI(tc);
        }
    }
    
    public void SetupWaveSystem(Vector3 levelID)
    {
        this.levelID = levelID;
        runWaveSystem = true;
        spawnEnemies = true;
        WaveController();
    }

    void WaveController()
    {
        if (enemiesParent.childCount == 0 && !isSpawningWave && levelID.z < maxWaves) //If there is no wave currently running, all enemies are dead, and there are more waves to be spawned
        {
            levelID.z++;
            string path = "Waves\\Planet" + levelID.x + "\\Level" + levelID.y + "\\Wave" + levelID.z;
            LoadWave(path);
            StartCoroutine(RunWave());
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
        if (spawnEnemies) //If spawning is enabled (change to while to make looping waves)
        {
            isSpawningWave = true;
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
        isSpawningWave = false;
    }

    public void UpdateMoney(int m) //Add/remove money & update UI value
    {
        money += m;
        mUI.moneyTxt.text = "Money: $" + money;
    }

    // --- Wave Systems ---

    void LoadWave(string resourcesPath)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>(resourcesPath);
        Wave w = XMLOp.DeserializeXMLTextAsset<Wave>(xmlAsset);

        // \/ Line below will load xml from path instead
        //Wave w = XMLOp.Deserialize<Wave>("D:\\Game Stuff\\Game Making\\GGRemastered\\Galactic-Gauntlet-Remastered\\Assets\\Resources\\Waves\\test.xml");

        enemiesToSpawn.Clear();
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
            Enemy e = new Enemy(ec.id, ec.name, 0);
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
        td.bulletData = new BulletData();
        XMLOp.Serialize(td, outPath);
    }

    void GCode()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            UpdateMoney(100);
    }
}