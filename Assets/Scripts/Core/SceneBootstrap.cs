using UnityEngine;

public class SceneBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        if (GameManager.Instance == null)
        {
            GameObject managerObj = new GameObject("GameManager");
            managerObj.AddComponent<GameManager>();
            managerObj.AddComponent<LevelManager>();
            managerObj.AddComponent<WordValidator>();
            managerObj.AddComponent<WordDatabase>();
            managerObj.AddComponent<AudioManager>();
            managerObj.AddComponent<AdsManager>();
        }
    }
}
