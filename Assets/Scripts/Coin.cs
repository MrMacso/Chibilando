using UnityEngine;

public class Coin : MonoBehaviour, IBind<CoinData>
{
    CoinData _data;

    public void Bind(CoinData data)
    {
        _data = data;
        if (_data.IsCollected)
            gameObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        var player = collision.GetComponent<Player>();

        if (player)
        {
            player.AddPoint();
            _data.IsCollected = true;
            gameObject.SetActive(false);
        }
    }
}


