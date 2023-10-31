using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player == null)
            return;

        player.SetIsClimbing(true);
        player.SetGravity(0f);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player == null)
            return;

        player.SetIsClimbing(false);
        player.SetGravity(1f);
    }
}
