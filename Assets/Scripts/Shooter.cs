using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this script is just for playing shooting game
// you can shoot bullet toward the mouse pointer
public class Shooter : MonoBehaviour
{
    public GameObject prefab;
    public GameObject TowerTop;
    public Transform firePos;

    private float nextFire;
    private float fireRate;

    private void Awake () 
    {
        fireRate = 0.75f;
        nextFire = 0f;
    }

    private void Update() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("hit: " + hit.collider.gameObject.name);
        }

        // tower rotation on y axis
        Quaternion towerHeading = Quaternion.LookRotation(hit.point);
        towerHeading.x = 0f;
        towerHeading.z = 0f;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, towerHeading, 0.1f);

        // tower top rotation on x axis
        Quaternion towerTopHeading = Quaternion.LookRotation(hit.point);
        towerHeading.y = 0f;
        towerHeading.z = 0f;
        TowerTop.transform.rotation = Quaternion.Lerp(TowerTop.transform.rotation, towerTopHeading, 0.1f);


        if (Input.GetMouseButton(0) && Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(prefab, firePos.position, Quaternion.Euler(-10f, 0f, 0f)) as GameObject;
            // bullet.Get Component<Bullet>().Fire(hit.point.normalized);
        }

    }   
}
