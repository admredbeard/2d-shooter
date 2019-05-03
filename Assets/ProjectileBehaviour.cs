using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    public GameObject bullet;
    ProjectileBehaviour(float bulletDamage, float bulletDistance, float bulletSpeed, Vector3 startingPosition, Vector3 bulletDirection){
        damage = bulletDamage;
        distance = bulletDistance;
        speed = bulletSpeed;
        start = startingPosition;
        direction = bulletDirection;
    }
    
    void Start()
    {
        myBullet = Instantiate(bullet, start, Quaternion.identity);
        rbody = myBullet.GetComponent<Rigidbody> ();
        rbody.velocity = speed * direction;
        ProjectileDestruction();
    }

    GameObject myBullet;
    Vector3 direction;
    Vector3 start;
    Rigidbody rbody;
    private float damage = 0f;
    private float distance = 0f;
    private float fps = 30f;
    private float speed = 100f;

    void OnCollisionEnter(Collision2D col){
        GameObject target = col.gameObject;
        PlayerBehaviour player = target.GetComponent<PlayerBehaviour> ();

        if(player != null){
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
    IEnumerator ProjectileDestruction(){

        float time = 0f;
        while(time < 1.5f){

            time += 1/30;
            yield return new WaitForSeconds(1/30);
        }

        Destroy(gameObject);
    }
}
