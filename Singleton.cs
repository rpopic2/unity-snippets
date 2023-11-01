using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Init(T self)
    {
        if (Instance != null) {
            Debug.LogWarning("싱글톤이 이미 존재합니다. 이 오브젝트를 파괴합니다.");
            Destroy(Instance);
        }
        Instance = self;
    }
}

