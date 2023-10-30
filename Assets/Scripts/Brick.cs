using UnityEngine;

public class Brick : MonoBehaviour, ITakeDamage
{
    [SerializeField] ParticleSystem _brickParticles;
    [SerializeField] float _laserDestructionTime = 1f;
    [SerializeField] int _shotsToDestroy = 3;
    float _takenDamageTime;
    SpriteRenderer _spriteRenderer;
    float _resetColorTime;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void TakeLaserDamage()
    {
        _spriteRenderer.color = Color.red;
        _resetColorTime = Time.time + 0.1f;

        _takenDamageTime += Time.deltaTime;
        if (_takenDamageTime >= _laserDestructionTime)
            Explode();
    }
    void Update()
    {
        if (_resetColorTime > 0 && Time.time >= _resetColorTime)
        {
            _resetColorTime = 0;
            _spriteRenderer.color = Color.white;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null)
            return;
        Vector2 normal = collision.contacts[0].normal;
        float dot = Vector2.Dot(normal, Vector2.up);

        if (dot > 0.5)
        {
            player.StopJump();
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(_brickParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void TakeDamage()
    {
        _takenDamageTime += _laserDestructionTime / _shotsToDestroy;
        if (_takenDamageTime >= _laserDestructionTime)
            Explode();
    }
}
