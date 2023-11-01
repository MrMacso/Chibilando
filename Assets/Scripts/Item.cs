using UnityEngine;

public abstract class Item : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        var playerInventory = collision.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.Pickup(this, true);
            gameObject.transform.rotation = collision.transform.rotation;
        }
    }
    public abstract void Use();
}

