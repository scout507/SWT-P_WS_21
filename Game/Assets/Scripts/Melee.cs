using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : ShootGun
{
    // Start is called before the first frame update
    void Start()
    {
        //this.gunDamage = 100;
        this.fireRate = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer) 
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate;
            Shoot();  
        }
    }

    public override void Shoot()
    {
        throw new System.NotImplementedException();
    }
}
