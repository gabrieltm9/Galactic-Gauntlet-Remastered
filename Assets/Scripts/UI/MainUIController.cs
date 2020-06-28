﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainUIController : MonoBehaviour
{
    public GameController gc;

    public GameObject buyMenu;
    public TMP_Text moneyTxt;

    public List<GameObject> towerPrefabs; //0 = machine gun
    public Transform towersParent;

    public GameObject towerBeingPlaced;

    [Header("Upgrade Menu")]
    public GameObject upgradeMenu;
    public TMP_Text towerNameTxt;
    public TMP_Text rangeTxt;
    public TMP_Text damageTxt;
    public TMP_Text rateOfFireTxt;

    public void PlaceTower(int id) //Places a tower based on it's id
    {
        int price = towerPrefabs[id].GetComponent<TowerController>().price;
        if(gc.money >= price && !gc.isPlacingTower)
        {
            gc.isPlacingTower = true;
            buyMenu.SetActive(false);
            gc.UpdateMoney(price, false);
            towerBeingPlaced = Instantiate(towerPrefabs[id], transform.position, transform.rotation, towersParent);
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
            Debug.Log(mousePos);
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
                gc.UpdateMoney(towerBeingPlaced.GetComponent<TowerController>().price, true);
                gc.selectedTower = null;
                gc.isPlacingTower = false;
                buyMenu.SetActive(true);
                Destroy(towerBeingPlaced);
            }
        }
    }

    public void EnableUpgradeMenu(TowerController tc)
    {
        upgradeMenu.SetActive(true);
        towerNameTxt.text = tc.name;
        rangeTxt.text = "Range: " + tc.range;
    }

    public void DisableUpgradeMenu()
    {
        upgradeMenu.SetActive(false);
    }
}
