using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainUIController : MonoBehaviour
{
    public GameController gc;

    public GameObject mainUI;
    public GameObject buyMenu;
    public TMP_Text moneyTxt;
    public TMP_Text waveCountTxt;
    public List<TMP_Text> towerPrices;
    public KeyCode toggleMainUIKey;

    public GameObject towerBeingPlaced;

    [Header("Upgrade Menu")]
    public GameObject upgradeMenu;
    public TMP_Text towerNameTxt;
    public TMP_Text rangeTxt;
    public TMP_Text damageTxt;
    public TMP_Text fireRateTxt;
    public TMP_Text levelTxt;
    public TMP_Text upgradeDescriptionTxt;
    public TMP_Text upgradePriceTxt;

    public void PlaceTower(int id) //Places a tower based on it's id
    {
        int price = gc.towerPrefabs[id].GetComponent<TowerController>().price;
        if(gc.money >= price && !gc.isPlacingTower)
        {
            gc.isPlacingTower = true;
            buyMenu.SetActive(false);
            gc.UpdateMoney(-price);
            towerBeingPlaced = Instantiate(gc.towerPrefabs[id], transform.position, transform.rotation, gc.towersParent);
            if (gc.selectedTower != null)
                gc.selectedTower.ToggleUI();
            gc.selectedTower = towerBeingPlaced.GetComponent<TowerController>();
        }
    }

    private void Update()
    {
        if(gc.isPlacingTower)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8.33f));
            towerBeingPlaced.transform.position = new Vector3(mousePos.x, 0f, mousePos.z);

            if(Input.GetMouseButtonDown(0) && towerBeingPlaced.GetComponent<TowerController>().objectsInSpace.Count == 0) //If left click to place tower & no obstructions in that place
            {
                gc.isPlacingTower = false;
                buyMenu.SetActive(true);
                EnableUpgradeMenu(towerBeingPlaced.GetComponent<TowerController>());
                towerBeingPlaced.GetComponent<TowerController>().isActive = true;
            }

            if(Input.GetKeyDown(KeyCode.Escape)) //Cancel place
            {
                gc.UpdateMoney(towerBeingPlaced.GetComponent<TowerController>().price);
                gc.selectedTower = null;
                gc.isPlacingTower = false;
                buyMenu.SetActive(true);
                Destroy(towerBeingPlaced);
            }
        }

        if (Input.GetKeyDown(toggleMainUIKey))
            ToggleMainUI();
    }

    public void EnableUpgradeMenu(TowerController tc)
    {
        upgradeMenu.SetActive(true);
        UpdateUpgradeUI(tc);
    }

    public void DisableUpgradeMenu()
    {
        upgradeMenu.SetActive(false);
    }

    public void UpdateUpgradeUI(TowerController tc)
    {
        towerNameTxt.text = tc.name;
        rangeTxt.text = "Range: " + tc.range;
        damageTxt.text = "Damage: " + tc.damage;
        fireRateTxt.text = "Fire Rate: " + tc.fireRate; //In shoot delay, lower number = faster. This makes bigger number = faster
        levelTxt.text = "Level: " + tc.level;
        if (tc.level <= tc.td.upgradeLevels.Length)
        {
            upgradeDescriptionTxt.text = tc.td.upgradeLevels[tc.level - 1].description;
            upgradePriceTxt.text = "$" + tc.td.upgradeLevels[tc.level - 1].cost;
        }
        else
        {
            upgradeDescriptionTxt.text = "Fully Upgraded";
            upgradePriceTxt.text = "";
        }
    }

    public void UpdateWaveCount(int currentWave, int maxWaves)
    {
        waveCountTxt.text = "Wave: " + currentWave + "/" + maxWaves;
    }

    void ToggleMainUI()
    {
        mainUI.SetActive(!mainUI.activeSelf);
    }
}