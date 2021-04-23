using UnityEngine;
using UnityEngine.SceneManagement;
using Network;
using Managers;

public class ManagerSpawner : MonoBehaviour {
    [Header("Manager Prefabs")]
    public Transform netClient;
    public int netClientScene;
    public Transform sceneManager;
    public int sceneManagerScene;
    public Transform gameObjManager;
    public int gOMSpawnScene;
    public Transform playerCamera;
    public int playerCamScene;
    public Transform UIWorldManager;
    public int UIWorldManagerScene;

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
        if(scene.buildIndex == netClientScene)
        {
            if(NetClient.Instance == null) Instantiate(netClient);
        }

        if (scene.buildIndex == sceneManagerScene)
        {
            if (SceneManager.Instance == null) Instantiate(sceneManager);
        }

        if (scene.buildIndex == gOMSpawnScene)
        {
            if (GameObjectManager.Instance == null) Instantiate(gameObjManager);
        }

        if (scene.buildIndex == playerCamScene)
        {
            if (MainPlayerCamera.Instance == null) Instantiate(playerCamera);
        }

        if (scene.buildIndex == UIWorldManagerScene)
        {
            if (UIWorldElementManager.Instance == null) Instantiate(UIWorldManager);
        }
    }
}
