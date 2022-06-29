using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float cooldownInterval;
    public bool isRanged;
    public float lifeTime;
    public float attacksLeft;
    public GameObject bulletPrefab;

    public float cooldown;
    bool onCoolDown;
    private void Start()
    {
        onCoolDown = false;
    }
    public void attack(Vector3 position, Vector3 forward)
    {
        //Debug.Log("attack clicked");
        if (onCoolDown)
        {
            //Debug.Log("you cant attack right now!");
            return;
        }

        if (!isRanged)
        {
            melee(position, forward);
        }
        else
        {
            ranged(position, forward);
        }

        attacksLeft--;
        cooldown = cooldownInterval;
        onCoolDown = true; 
    }

    void melee(Vector3 position, Vector3 forward)
    {
        // to be done
    }

    void ranged(Vector3 position, Vector3 forward)
    {
        GameObject prefab = Instantiate(bulletPrefab);
        prefab.transform.position = position + forward;
        prefab.transform.forward = forward;
    }

    private void Update()
    {
        if (lifeTime <= 0 || attacksLeft <= 0)
        {
            Destroy(gameObject);
        }

        if (onCoolDown &&  cooldown <= 0)
        {
            onCoolDown = false;
            cooldown = 0;
        }

        cooldown -= Time.deltaTime;
        lifeTime -= Time.deltaTime;       
    }

}
