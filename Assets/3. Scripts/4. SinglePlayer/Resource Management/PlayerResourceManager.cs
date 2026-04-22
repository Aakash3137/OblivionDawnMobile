public class PlayerResourceManager : ResourceManager
{
    public static PlayerResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
