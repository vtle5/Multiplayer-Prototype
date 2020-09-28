using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    public TextMeshPro nameDisplay;
    public GameObject cursorObj;

    public float health;
    public float maxHealth = 100f;
    public float mana;
    public float maxMana = 100f;

    public Vector2 cursor;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        cursor = Vector2.zero;
        nameDisplay.text = _username;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //model.enabled = false;
    }

    public void Respawn()
    {
        //model.enabled = true;
        SetHealth(maxHealth);
    }
    public void Update()
    {
        if (Client.instance.myId == id)
        {
            Debug.DrawLine(transform.position, cursor, Color.yellow);  //draws the "cursor"
        }
        else
        {
            Debug.DrawLine(transform.position, cursor, Color.red);  //draws the "cursor"
        }

        cursorObj.transform.position = cursor;
    }
}
