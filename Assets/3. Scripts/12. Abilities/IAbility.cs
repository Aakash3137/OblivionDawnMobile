public interface IAbility
{
    void Initialize(Stats owner);
    void Activate();
    bool CanActivate();
}