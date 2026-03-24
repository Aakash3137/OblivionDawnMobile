public abstract class AbilityBase : IAbility
{
    protected Stats owner;

    public virtual void Initialize(Stats owner)
    {
        this.owner = owner;
    }

    public abstract void Activate();

    public abstract bool CanActivate();
    
}