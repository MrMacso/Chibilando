using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float _maxHorizonalSpeed = 5;
    [SerializeField] float _maxVerticalSpeed = 5;
    [SerializeField] float _jumpVelocity = 5;
    [SerializeField] float _jumpDuration = 0.5f;
    [SerializeField] Sprite _jumpSprite;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] LayerMask _waterLayerMask;
    [SerializeField] float _footOffset = 0.35f;
    [SerializeField] float _groundAcceleration = 25;
    [SerializeField] float _snowAcceleration = 1;
    [SerializeField] AudioClip _coinSfx;
    [SerializeField] AudioClip _hurtSfx;
    [SerializeField] float _knockbackVelocity = 300;
    [SerializeField] Collider2D _duckCollider;
    [SerializeField] Collider2D _standingCollider;
    [SerializeField] float _groundDetectionOffset = 1.01f;
    [SerializeField] float _wallDetectionDistance = 0.5f;
    [SerializeField] int _wallCheckPoints = 5;
    [SerializeField] float _buffer = 0.1f;

    public bool IsGrounded;
    public bool IsInWater;
    public bool IsOnSnow;
    public bool IsDucking;
    public bool IsClimbing;
    public bool IsTouchingRightWall;
    public bool IsTouchingLeftWall;

    Animator _animator;

    AudioSource _audioSource;
    Rigidbody2D _rb;
    PlayerInput _playerInput;

    float _horizontal;
    float _vertical;
    int _jumpRemaining;
    float _jumpEndTime;


    PlayerData _playerData = new PlayerData();
    RaycastHit2D[] _results = new RaycastHit2D[100];

    public event Action CoinsChanged;
    public event Action HealthChanged;

    public int Coins { get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health => _playerData.Health;

    public Vector2 Direction { get; private set; } = Vector2.right;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGrounding();
        UpdateWallTouching();

        UpdateMovement();
     
        UpdateAnimation();
        UpdateDirection();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _groundDetectionOffset);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
        //draw left foot
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _groundDetectionOffset);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);
        //draw right foot
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _groundDetectionOffset);
        Gizmos.DrawLine(origin, origin + Vector2.down * 0.1f);

        DrawGizmosForSide(Vector2.left);
        DrawGizmosForSide(Vector2.right);
    }

    void DrawGizmosForSide(Vector2 direction)
    {
        var activeCollider = IsDucking ? _duckCollider : _standingCollider;
        float colliderHeight = activeCollider.bounds.size.y - 2 * _buffer;
        float segmentSize = colliderHeight / (float)(_wallCheckPoints - 1);

        for (int i = 0; i < _wallCheckPoints; i++)
        {
            var origin = transform.position - new Vector3(0, activeCollider.bounds.size.y / 2f, 0);
            origin += new Vector3(0, _buffer + segmentSize * i, 0);
            origin += (Vector3)direction * _wallDetectionDistance;
            Gizmos.DrawWireSphere(origin, 0.05f);
        }
    }

    bool CheckForWall(Vector2 direction)
    {
        var activeCollider = IsDucking ? _duckCollider : _standingCollider;
        float colliderHeight = activeCollider.bounds.size.y - 2 * _buffer;
        float segmentSize = colliderHeight / (float)(_wallCheckPoints - 1);

        for (int i = 0; i < _wallCheckPoints; i++)
        {
            var origin = transform.position - new Vector3(0, activeCollider.bounds.size.y / 2f, 0);
            origin += new Vector3(0, _buffer + segmentSize * i, 0);
            origin += (Vector3)direction * _wallDetectionDistance;

            int hits = Physics2D.Raycast(origin,
                             direction,
                             new ContactFilter2D() { layerMask = _layerMask, useLayerMask = true },
                             _results,
                             0.1f);
            for (int hitIndex = 0; hitIndex < hits; hitIndex++)
            {
                var hit = _results[hitIndex];
                if (hit.collider && hit.collider.isTrigger == false)
                    return true;
            }
        }
        return false;
    }

    void UpdateWallTouching()
    {
        IsTouchingRightWall = CheckForWall(Vector2.right);
        IsTouchingLeftWall = CheckForWall(Vector2.left);
    }
    void UpdateMovement()
    {
        var input = _playerInput.actions["Move"].ReadValue<Vector2>();
        var horizontalInput = input.x;
        var verticalInput = input.y;

        var vertical = _rb.velocity.y;

        if (_playerInput.actions["Jump"].WasPerformedThisFrame() && _jumpRemaining > 0)
        {
            _jumpEndTime = Time.time + _jumpDuration;
            _jumpRemaining--;

            _audioSource.pitch = _jumpRemaining > 0 ? 1 : 1.2f;

            _audioSource.Play();
        }

        if (_playerInput.actions["Jump"].ReadValue<float>() > 0 && _jumpEndTime > Time.time)
            vertical = _jumpVelocity;

        var desiredHorizontal = horizontalInput * _maxHorizonalSpeed;
        var acceleration = IsOnSnow ? _snowAcceleration : _groundAcceleration;

        //_animator.SetBool("Duck", verticalInput < 0 && Math.Abs(verticalInput) > Math.Abs(horizontalInput));

        /*IsDucking = _animator.GetBool("IsDucking");
        if (IsDucking)
            desiredHorizontal = 0;
        _duckCollider.enabled = IsDucking;
        _standingCollider.enabled = !IsDucking;*/
        if (IsClimbing)
        {
            var desiredVertical = verticalInput * _maxVerticalSpeed;

            if (desiredVertical > _vertical)
            {
                _vertical += acceleration * Time.deltaTime;
                if (_vertical > desiredVertical)
                    _vertical = desiredVertical;
            }
            else if (desiredVertical < _vertical)
            {
                _vertical -= acceleration * Time.deltaTime;
                if (_vertical < desiredVertical)
                    _vertical = desiredVertical;
            }
        }

        if (desiredHorizontal > _horizontal)
        {
            _horizontal += acceleration * Time.deltaTime;
            if (_horizontal > desiredHorizontal)
                _horizontal = desiredHorizontal;
        }
        else if (desiredHorizontal < _horizontal)
        {
            _horizontal -= acceleration * Time.deltaTime;
            if (_horizontal < desiredHorizontal)
                _horizontal = desiredHorizontal;
        }

        if (desiredHorizontal > 0 && IsTouchingRightWall)
            _horizontal = 0;
        if (desiredHorizontal < 0 && IsTouchingLeftWall)
            _horizontal = 0;

        if (IsInWater)
            _rb.velocity = new Vector2(_rb.velocity.x, vertical);
        else if (IsClimbing)
            _rb.velocity = new Vector2(_horizontal, _vertical);
        else
            _rb.velocity = new Vector2(_horizontal, vertical);
    }

    void UpdateGrounding()
    {
        IsGrounded = false;
        IsOnSnow = false;
        IsInWater = false;

        //check center
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - _groundDetectionOffset);
        CheckGrounding(origin);

        //check left
        origin = new Vector2(transform.position.x - _footOffset, transform.position.y - _groundDetectionOffset);
        CheckGrounding(origin);

        //check right
        origin = new Vector2(transform.position.x + _footOffset, transform.position.y - _groundDetectionOffset);
        CheckGrounding(origin);

        if ((IsGrounded || IsInWater) && _rb.velocity.y <= 0)
            _jumpRemaining = 2;

    }

    void CheckGrounding(Vector2 origin)
    {
        int hits = Physics2D.Raycast(origin,
                                     Vector2.down,
                                     new ContactFilter2D() { layerMask = _layerMask, useLayerMask = true, useTriggers = true },
                                     _results,
                                     0.1f);

        for (int i = 0; i < hits; i++)
        {
            var hit = _results[i];

            if (!hit.collider)
                continue;

            IsGrounded = true;
            IsOnSnow |= hit.collider.CompareTag("Snow");
        }
        var water = Physics2D.OverlapPoint(origin, _waterLayerMask);
        if (water != null)
            IsInWater = true;
    }

    void UpdateAnimation()
    {
        _animator.SetBool("Jump", !IsGrounded);
        _animator.SetBool("IsClimbing", IsClimbing);
        if (IsClimbing)
            _animator.SetBool("Move", _vertical != 0f);
        else
        _animator.SetBool("Move", _horizontal != 0f);
    }

    private void UpdateDirection()
    {
        if (_horizontal > 0)
        {
            _animator.transform.rotation = Quaternion.identity;
            Direction = Vector2.right;
        }
        else if (_horizontal < 0)
        {
            _animator.transform.rotation = Quaternion.Euler(0, 180, 0);
            Direction = Vector2.left;
        }
    }
    public void AddPoint()
    {
        Coins++;
        _audioSource.PlayOneShot(_coinSfx);
        CoinsChanged?.Invoke();
    }
    public void Bind(PlayerData playerData)
    {
        _playerData = playerData;
    }
    public void RestorePositionAndVelocity()
    {
        _rb.position = _playerData.Position;
        _rb.velocity = _playerData.Velocity;
    }
    public void TakeDamage(Vector2 hitNormal)
    {
        _playerData.Health--;
        if (_playerData.Health <= 0)
        {
            SceneManager.LoadScene(0);
            return;
        }
        _rb.AddForce(-hitNormal * _knockbackVelocity);
        _audioSource.PlayOneShot(_hurtSfx);
        HealthChanged?.Invoke();
    }

    public void StopJump()
    {
        _jumpEndTime = Time.time;
    }

    public void Bounce(Vector2 normal, float bounciness)
    {
        _rb.AddForce(-normal * bounciness);
    }
    public void SetIsClimbing(bool isClimbing) 
    {
        IsClimbing= isClimbing;
    }
    public void SetGravity(float gravity)
    {
        _rb.gravityScale = gravity;
    }
}
