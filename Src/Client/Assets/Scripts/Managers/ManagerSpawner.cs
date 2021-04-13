using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Network;

public class ManagerSpawner : MonoBehaviour {
    [Header("Manager Prefabs")]
    public Transform netClient;
    public Transform sceneManager;

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
        if (!NetClient.Instance)
        {
            Instantiate(netClient, transform.position, transform.rotation);
        }

        if(!SceneManager.Instance)
        {
            Instantiate(sceneManager, transform.position, transform.rotation);
        }
    }
}
