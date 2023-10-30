using UnityEngine;
using UnityEngine.Pool;

public class BlasterShot : MonoBehaviour
{
    [SerializeField] float _speed = 8.0f;
    [SerializeField] float _maxLifetime = 4f;

    Rigidbody2D _rb;
    Vector2 _direction = Vector2.right;
    ObjectPool<BlasterShot> _pool;
    float _selfDestructTime;
    bool _exploded;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void SelfDestruct()
    {
        _pool.Release(this);
    }

    void Update()
    {
        _rb.velocity = _direction * _speed;
        if (Time.time >= _selfDestructTime)
            SelfDestruct();
    }

    public void Launch(Vector2 direction, Vector2 position)
    {
        _exploded = false;
        transform.position = position;
        _direction = direction;
        transform.rotation = _direction == Vector2.left ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        _selfDestructTime = Time.time + _maxLifetime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var damageable = collision.gameObject.GetComponent<ITakeDamage>();
        if (damageable == null)
            damageable = collision.gameObject.GetComponentInParent<ITakeDamage>();

        if (damageable != null)
            damageable.TakeDamage();

        if (_exploded == false)
        {
            _exploded = true;
            PoolManager.Instance.GetBlasterExplosion(collision.contacts[0].point);
            SelfDestruct();
        }
    }

    public void SetPool(ObjectPool<BlasterShot> pool)
    {
        _pool = pool;
    }
}
