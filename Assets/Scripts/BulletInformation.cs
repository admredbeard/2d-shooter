using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInformation : MonoBehaviour
{
    public void InitiateBullet(float bulletDamage, float speed, Vector3 direction){
        damage = bulletDamage;
        Rigidbody2D rbody = GetComponent<Rigidbody2D> ();
        rbody.AddForce(speed * direction);
        transform.parent = GameObject.Find("Bullets").transform;
        StartCoroutine(ProjectileDestruction());
    }

    private float damage = 0f;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject target = col.gameObject;
        PlayerBehaviour player = target.GetComponent<PlayerBehaviour>();
        print("Bullet destroyed");

        if (target.tag.Equals("Player") && player != null)
        {
            player.TakeDamage(damage);
            print("Damage");
        }

        Destroy(gameObject);
    }

    IEnumerator ProjectileDestruction()
    {
        float time = 0f;

        while (time < 1.5f)
        {
            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }
}
