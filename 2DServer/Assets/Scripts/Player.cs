using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Rigidbody2D rb;
    public float moveSpeed = 40f;
    public float maxSpeed =4f;
    public float slideFactor=4f;

    public Vector2 cursor;

    public float health;
    public float maxHealth = 100f;
    public float mana;
    public float maxMana = 100f;
    public float bulletSpeed = 14f;

    private bool[] inputs;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        mana = maxMana;

        inputs = new bool[4];
    }

    public void FixedUpdate()
    {
        if (health <= 0)
        {
            return;
        }

        Vector2 _inputDirection = Vector2.zero; //processing input from client...
        if (inputs[0])//W
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])//A
        {
            _inputDirection.x -= 1;
        }
        if (inputs[2])//S
        {
            _inputDirection.y -= 1;
        }
        if (inputs[3])//D
        {
            _inputDirection.x += 1;
        }

        Move(_inputDirection.normalized * moveSpeed);
        Debug.DrawLine(transform.position, cursor, Color.red);  //draws the "cursor"
    }

    public void Move(Vector2 _moveVelocity)   //calculates the players movement
    {
        rb.AddForce(_moveVelocity);    //moves the player

        if (rb.velocity.magnitude > maxSpeed)      //cap the speed
        {
            rb.velocity = (rb.velocity.normalized) * maxSpeed;  //normalizes speed and sets it to the maxSpeed variable
        }
        
        //movement decay
        if (_moveVelocity.Equals(new Vector2(0, 0)))     //if no input is given
        {
            if (rb.velocity.magnitude < .0001f)         //reduce velocity to zero if slowed down enough
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity -= rb.velocity / slideFactor;   //otherwise cut velocity
            }
        }

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerCursorPos(this);   //updates everyone elses cursor
    }

    public void SetInput(bool[] _inputs, Vector2 _cursor)
    {
        inputs = _inputs;
        cursor = _cursor;
    }

    public void Shoot()
    {
        if (health <= 0f)
        {
            return;
        }
        Vector2 _lookDir = cursor - rb.position;
        Quaternion _angleDir = Quaternion.Euler(0f, 0f, Mathf.Atan2(_lookDir.y, _lookDir.x) * Mathf.Rad2Deg -90f);

        NetworkManager.instance.InstantiateProjectile(transform.position,_angleDir).Initialize(bulletSpeed, id);
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0)
        {
            return;
        }
        health -= _damage;
        if (health <= 0)
        {
            health = 0f;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }
        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        health = maxHealth;
        ServerSend.PlayerRespawned(this);
    }
}
