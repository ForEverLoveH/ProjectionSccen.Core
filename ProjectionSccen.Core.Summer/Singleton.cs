namespace ProjectionSccen.Core.Summer;

public class Singleton<T> where T:new()
{
    private static T instance;

    private static T Instance
    {
        get
        {
            if (instance == null) instance = new T();
            return instance;
        }
    }

    public static T GetInstance()
    {
        return Instance;
    }
}