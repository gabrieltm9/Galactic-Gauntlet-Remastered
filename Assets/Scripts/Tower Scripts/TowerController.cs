using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    public TowerData td; //An xml file containing name, type, price, upgrade data, etc, for this tower
    public TextAsset towerDataAsset; //The actual xml file for the TowerData above
    public string name;
    public int type; //Type of tower; 0 = machine gun
    public int price;
    public float range;
    public int damage;
    public float shootDelay; //How long between each shot

    public GameController gc;

    public GameObject modelRotateChild; //The gameobject to rotate to face enemies
    public Image rangeImg; //The tower's range UI child

    public bool isActive = false; //If false, tower wont shoot, rotate, etc

    public List<GameObject> enemiesInRange;
    public GameObject target;

    public float rotateStrength = 1;

    public bool uiEnabled;

    public GameObject bulletPrefab;
    public bool isShooting;

    public List<GameObject> objectsInSpace; //While being placed, any towers/objects intersecting with this tower will be added to this list; If this list is empty, tower can be placed
    public List<GameObject> towersInSynergyDistance; //Towers within the distance needed to synergise with this tower

    private void Awake()
    {
        gc = GameObject.FindObjectOfType<GameController>();
        UpdateData();
    }

    public void Update()
    {
        if(target != null && target.GetComponent<EnemyController>().isDying)
        {
            enemiesInRange.Remove(target);
            UpdateTarget();
        }

        if ((enemiesInRange.Count > 0) && (target == null || enemiesInRange[0] == null))
            UpdateTarget();

        if(Input.GetKeyDown(KeyCode.Escape) && uiEnabled && isActive)
            ToggleUI();

        if(Input.GetKeyDown(gc.sellTowerKey) && uiEnabled && isActive)
            SellTower(0.5f);
    }

    void UpdateData()
    {
        td = XMLOp.DeserializeXMLTextAsset<TowerData>(towerDataAsset); //Deserialize tower data xml

        price = td.price;
        range = td.range;
        damage = td.damage;
        shootDelay = td.shootDelay;

        rangeImg.transform.localScale = new Vector3(range, range, range);
    }

    void SellTower(float priceMultiplier)
    {
        gc.UpdateMoney((int)(price * priceMultiplier), true); //Give back money equal to the price of this tower times a multiplier (ex. get half money back vs full refund, etc)
        gc.DeselectTower(this);

        foreach (GameObject tower in towersInSynergyDistance) //Remove this tower from others' synergy lists
            tower.GetComponent<TowerController>().UpdateSynergyList(gameObject, false);

        Destroy(gameObject);
    }

    void LateUpdate() //Runs at the end of every frame
    {
        if(target != null) //If there is a target, rotate to face it. In late update so the target can move in this frame before the tower rotates to face it.; prevents delay
            Rotate();
    }

    void Rotate() //Rotates tower to face target
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - modelRotateChild.transform.position);
        targetRotation.Set(0, targetRotation.y, targetRotation.z, targetRotation.w);
        float str = Mathf.Min(rotateStrength * Time.deltaTime, 1);
        modelRotateChild.transform.rotation = Quaternion.Lerp(modelRotateChild.transform.rotation, targetRotation, str);
    }

    private void OnMouseDown() //When tower's box collider is clicked on
    {
        if(isActive && !gc.isPlacingTower)
            ToggleUI();
    }

    public void ToggleUI()
    {
        uiEnabled = !uiEnabled;
        if (uiEnabled)
            EnableUI();
        else
            DisableUI();
    }

    void EnableUI()
    {
        gc.SelectTower(this);
        rangeImg.enabled = true;
    }

    void DisableUI()
    {
        gc.DeselectTower(this);
        rangeImg.enabled = false;
    }

    IEnumerator ShootTimer()
    {
        isShooting = true;
        while (target != null)
        {
            yield return new WaitForSeconds(shootDelay);
            if (target == null)
                break;
            Shoot();
        }
        isShooting = false;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, modelRotateChild.transform.position, modelRotateChild.transform.rotation, transform);
        bullet.GetComponent<BulletController>().SetupBullet(damage);
    }

    public void UpdateTarget() //Called whenever the tower's available targets changes
    {
        if(isActive)
        {
            if (enemiesInRange.Count > 0)
            {
                //Target latest enemy
                while (enemiesInRange[0] == null)
                    enemiesInRange.RemoveAt(0);
                if (enemiesInRange.Count > 0) //If there is an enemy to target
                {
                    target = enemiesInRange[0];
                    if (!isShooting)
                        StartCoroutine(ShootTimer());
                }
                else
                    target = null;
            }
        }
    }

    public void UpdateObjectsInSpace(GameObject obstructor, bool addToRange) //A list of objects in this tower's space (meaning this tower cant be placed there)
    {
        if(addToRange)
            objectsInSpace.Add(obstructor);
        else
            objectsInSpace.Remove(obstructor);
        UpdateRangeTint();
    }

    public void UpdateSynergyList(GameObject tower, bool addToRange) //The list of towers within this tower's synergy distance
    {
        if (addToRange)
            towersInSynergyDistance.Add(tower);
        else
            towersInSynergyDistance.Remove(tower);
        UpdateRangeTint();
    }

    public void UpdateRangeTint()
    {
        if (objectsInSpace.Count > 0)
            rangeImg.color = Color.red;
        else if (towersInSynergyDistance.Count > 0)
            rangeImg.color = Color.blue;
        else
            rangeImg.color = Color.white;
    }
}