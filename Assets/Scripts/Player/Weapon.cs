using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;
    public bool isShooting;
    public bool readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    public int bulletPerBurst = 3;
    public int burstBulletsLeft;
    public float spread;

    public enum ShootingMode{
        Single, Burst, Auto
    }

    public ShootingMode shootingMode;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public float bulletVelocity = 30f;
    public float bulletLifeTime = 3f;

    SoundController soundController;

    void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletPerBurst;
        soundController = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundController>();
    }

    void Update()
    {
        if(shootingMode == ShootingMode.Auto){
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(shootingMode == ShootingMode.Single || shootingMode == ShootingMode.Burst){
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        if(readyToShoot && isShooting){
            burstBulletsLeft = bulletPerBurst;
            FireWeapon();
        }
    }

    public Vector3 CalculateDirectionAndSpread(){

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit)){
            targetPoint = hit.point;
        }
        else{
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        return direction + new Vector3(x, y, 0);
    }

    private void FireWeapon(){
        soundController.Play(soundController.shoot);
        readyToShoot = false;

        Vector3 shootDirection = CalculateDirectionAndSpread().normalized;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.transform.forward = shootDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootDirection * bulletVelocity, ForceMode.Impulse);
        Destroy(bullet, bulletLifeTime);

        if(allowReset){
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }
        if(shootingMode == ShootingMode.Burst && burstBulletsLeft > 1){
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        
        }
    }

    private void ResetShot(){
        readyToShoot = true;
        allowReset = true;
    }
}
