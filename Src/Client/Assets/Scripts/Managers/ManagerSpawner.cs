using UnityEngine;
using UnityEngine.SceneManagement;
using Network;
using Managers;

public class ManagerSpawner : MonoBehaviour {
    [Header("Manager Prefabs")]
    public Transform netClient;
    public int nCSpawnScene = 0;
    public Transform sceneManager;
    public int sMSpawnScene = 0;
    //public Transform gameObjManager;
    //public int gOMSpawnScene = 2;

    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (!NetClient.Instance && scene.buildIndex >= nCSpawnScene)
        {
            Instantiate(netClient, transform.position, transform.rotation);
        }

        if(!SceneManager.Instance && scene.buildIndex >= sMSpawnScene)
        {
            Instantiate(sceneManager, transform.position, transform.rotation);
        }
    }
}
