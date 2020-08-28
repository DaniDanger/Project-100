using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int healthAmount = 100;
    public int damageGain = 10;
    RaycastWeapon raycastWeapon;

    private void Start()
    {
        raycastWeapon = GameObject.FindObjectOfType<RaycastWeapon>();
    }
    public void Damage(int damage)
    {
        healthAmount -= damage;
    }

    private void Update()
    {
        if(healthAmount < 1)
        {
            EnemyDeath();
        }
    }
    void EnemyDeath()
    {
        raycastWeapon.GetComponent<RaycastWeapon>().EnemyKilled();
        Destroy(gameObject);
    }

}
