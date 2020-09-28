using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;
    public int id;
    public Rigidbody2D rigidBody;
    public int owner;
    public float speed;

    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);
        rigidBody.AddForce(transform.up * speed, ForceMode2D.Impulse);
        ServerSend.SpawnProjectile(this);

        StartCoroutine(DecayAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        GameObject obj = _collision.gameObject;
        switch (obj.tag)
        {
            case "Player":
                Player _player = obj.GetComponent<Player>();
                if (_player.id!=owner)                          //if the bullet is not from its owner, do damage
                {
                    obj.GetComponent<Player>().TakeDamage(1f);
                    Hit();
                }
                break;
            case "Solid":
                Hit();
                break;
            default:
                //do nothing
                break;
        }
    }

    public void Initialize(float _speed, int _owner)
    {
        speed = _speed;
        owner = _owner;
    }
    private void Hit()
    {
        ServerSend.ProjectileHit(this);
        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator DecayAfterTime()
    {
        yield return new WaitForSeconds(8f);
        Hit();
    }
}
