using System;
using UnityEngine;

public class ShootAnimationWrapper : MonoBehaviour
{
    public event Action OnShoot;
    public event Action OnReload;

    public void Shoot() => OnShoot?.Invoke();
    public void Reload() => OnReload?.Invoke();
}
