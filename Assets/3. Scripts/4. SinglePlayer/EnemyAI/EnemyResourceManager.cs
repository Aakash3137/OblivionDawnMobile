public class EnemyResourceManager : ResourceManager
{
    public static EnemyResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
