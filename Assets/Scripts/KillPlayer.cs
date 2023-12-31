using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player != null)
            player.PlayerDeath();
    }
}
