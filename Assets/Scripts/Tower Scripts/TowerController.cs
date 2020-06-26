using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    public GameController gc;
    public int type; //Type of tower; 0 = machine gun

    public GameObject modelRotateChild; //The gameobject to rotate to face enemies
    public Image rangeImg; //The tower's range UI child

    public List<GameObject> enemiesInRange;
    public GameObject target;

    public float rotateStrength = 1;

    public bool uiEnabled;

    public float shootDelay; //How long between each shot
    public GameObject bulletPrefab;
    public bool isShooting;

    private void Awake()
    {
        gc = GameObject.FindObjectOfType<GameController>();
    }

    public void Update()
    {
        if(target != null && target.GetComponent<EnemyController>().isDying)
        {
            enemiesInRange.Remove(target);
            UpdateTarget();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && uiEnabled)
            ToggleUI();
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
        gc.selectedTower = null;
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
        Instantiate(bulletPrefab, modelRotateChild.transform.position, modelRotateChild.transform.rotation, transform); 
    }

    public void UpdateTarget() //Called whenever the tower's available targets changes
    {
        bool shouldStartShooting = false;
        if (target == null)
            shouldStartShooting = true;

        if (enemiesInRange.Count > 0)
        {
            //Target latest enemy
            while (enemiesInRange[0] == null || enemiesInRange[0].GetComponent<EnemyController>().isDying)
                enemiesInRange.RemoveAt(0);
            target = enemiesInRange[0];
        }

        if(shouldStartShooting)
            StartCoroutine(ShootTimer());
    }
}
