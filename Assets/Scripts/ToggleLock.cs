using UnityEngine;
using UnityEngine.Events;

public class ToggleLock : MonoBehaviour
{
    [SerializeField] UnityEvent OnUnlocked;
    bool _unlocked;
    SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _unlocked = false;
        _spriteRenderer.color = Color.grey;
    }

    [ContextMenu(nameof(Toggle))]
    public void Toggle()
    {
        _unlocked = !_unlocked;
        _spriteRenderer.color = _unlocked ? Color.white : Color.grey;
        if (_unlocked)
            OnUnlocked?.Invoke();
    }
}
