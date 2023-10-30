public interface ITakeDamage
{
    void TakeDamage();
}

public interface INamed
{
    string Name { get; set; }
}
public interface IBind<T>
{
    void Bind(T data);
}