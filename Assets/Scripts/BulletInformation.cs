using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInformation : MonoBehaviour
{
    GameObject attackerObject;
    float lifeTime = 0f;
    //Vector3 direction = Vector3.zero;
    public void InitiateBullet(float bulletDamage, float speed, Vector3 direction, GameObject attacker, float bulletLifeTime)
    {
        damage = bulletDamage;
        lifeTime = bulletLifeTime;
        transform.parent = GameObject.Find("Bullets").transform;
        attackerObject = attacker;
        StartCoroutine(ProjectileDestruction(direction, speed));
    }

    int shotgunBulletAmount = 6;
    private float damage = 0f;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    bool hit = false;
    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject target = col.gameObject;
        if (!target.tag.Equals("Bullet"))
        {
            PlayerBehaviour player = target.GetComponent<PlayerBehaviour>();

            if (target.tag.Equals("Player") && player != null && target.name != attackerObject.name)
            {
                player.TakeDamage(damage);
                print("Damage");
            }
            hit = true;
        }
    }

    IEnumerator ProjectileDestruction(Vector3 direction, float bulletSpeed)
    {
        float time = 0f;
        float step = 0.01f;

        while (time < lifeTime && !hit)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, step * bulletSpeed);
            time += step;
            yield return new WaitForSeconds(step);
        }
        
        Destroy(gameObject);
    }
}
