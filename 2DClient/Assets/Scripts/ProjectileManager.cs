using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int id;
    public int owner;
    public SpriteRenderer spriteRender;

    public void Initialize(int _id,int _owner)
    {
        id = _id;
        owner = _owner;
        if (owner==Client.instance.myId)
        {
            spriteRender.color = Color.gray;
        }
    }

    public void Hit(Vector3 _position)
    {
        transform.position = _position;
        GameManager.projectiles.Remove(id);
        Destroy(gameObject);
    }
}
