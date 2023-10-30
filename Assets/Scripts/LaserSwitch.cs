using UnityEngine;
using UnityEngine.Events;

public class LaserSwitch : MonoBehaviour, IBind<LaserSwitchData>
{
    [SerializeField] Sprite _left;
    [SerializeField] Sprite _right;

    [SerializeField] UnityEvent _on;
    [SerializeField] UnityEvent _off;

    SpriteRenderer _spriteRenderer;
    LaserSwitchData _data;
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();

        if (player == null)
            return;

        var rigidbody = player.GetComponent<Rigidbody2D>();
        if (rigidbody.velocity.x > 0)
            TurnOn();
        else if (rigidbody.velocity.x < 0)
            TurnOff();
    }

    public void Bind(LaserSwitchData data)
    {
        _data = data;
        UpdateSwitchState();
    }
    void TurnOff()
    {
        if (_data.IsOn)
        {
            _data.IsOn = false;
            UpdateSwitchState();

            Debug.Log("OFF");
        }
    }

    private void TurnOn()
    {
        if (!_data.IsOn)
        {
            _data.IsOn = true;
            UpdateSwitchState();
            Debug.Log("ON");
        }
    }

    private void UpdateSwitchState()
    {
        if (_data.IsOn)
        {
            _on.Invoke();
            _spriteRenderer.sprite = _right;
        }
        else
        {
            _off.Invoke();
            _spriteRenderer.sprite = _left;
        }
    }
}
