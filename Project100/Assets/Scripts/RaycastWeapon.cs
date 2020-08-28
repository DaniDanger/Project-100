using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public int bounce;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    public ActiveWeapon.WeaponSlot weaponSlot;

    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastOrigin;
    public Transform raycastDestination;
    public WeaponRecoil recoil;
    public GameObject magazine;
    public TrailRenderer tracerEffect;
    

    public int ammoCount;
    public int clipSize;
    public int damage;
    public int damageGain;
    public int enemyKilled;
    public bool isFiring = false;
    public float fireRate = 25;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public int maxBounces = 0;
    public string weaponName;

    Ray ray;
    RaycastHit hitInfo;

    float accumaltedTime;
    float maxLifeTime = 3.0f;
    List<Bullet> bullets = new List<Bullet>();

    private void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
        
    }
    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        bullet.bounce = maxBounces;
        Color color = Random.ColorHSV(0.46f, 0.61f);
        float intensity = 20.0f;
        Color rgb = new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a * intensity);
        bullet.tracer.material.SetColor("_EmissionColor", rgb);
        return bullet;

    }
    public void StartFiring()
    {
        isFiring = true;
        accumaltedTime = 0.0f;
        recoil.Reset();
        //FireBullet();
    }

    public void UpdateWeapon(float deltaTime)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartFiring();
        }
        if (isFiring)
        {
            UpdateFiring(deltaTime);
        }
        UpdateBullet(deltaTime);
        if (Input.GetButtonUp("Fire1"))
        {
            StopFiring();
        }
    }
    public void UpdateFiring(float deltaTime)
    {
        accumaltedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while(accumaltedTime >= 0.0f)
        {
            FireBullet();
            accumaltedTime -= fireInterval;
        }
    }

    public void UpdateBullet(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>{
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        Color debugColor = Color.green;

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            if(hitInfo.transform.gameObject.CompareTag("Enemy"))
            {
                if(hitInfo.collider != null)
                {
                    hitInfo.collider.gameObject.GetComponent<Enemy>().Damage(damage);
                    Debug.Log("skrt2);");
                }
            }
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.time = maxLifeTime;
            end = hitInfo.point;
            debugColor = Color.red;

            if(bullet.bounce > 0)
            {
                bullet.time = 0;
                bullet.initialPosition = hitInfo.point;
                bullet.initialVelocity = Vector3.Reflect(bullet.initialVelocity, hitInfo.normal);
                bullet.bounce--;
            }

            var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d)
            {
                rb2d.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
            }
        } 

        bullet.tracer.transform.position = end;
        
    }

    public void EnemyKilled()
    {
        enemyKilled++;
    }
    private void FireBullet()
    {
        if (ammoCount <= 0)
        {
            return;
        }
        ammoCount--;
        foreach (var particlesystem in muzzleFlash)
        {
            particlesystem.Emit(1);
        }

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);
        recoil.GenerateRecoil(weaponName);
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
