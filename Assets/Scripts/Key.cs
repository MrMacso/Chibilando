using UnityEngine;

public class Key : Item
{
    [SerializeField] float _useRange = 1f;

    public override void Use()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, _useRange);
        foreach (var hit in hits)
        {
            var toggleLock = hit.GetComponent<ToggleLock>();
            if (toggleLock)
            {
                toggleLock.Toggle();
                break;
            }
        }
    }
}
