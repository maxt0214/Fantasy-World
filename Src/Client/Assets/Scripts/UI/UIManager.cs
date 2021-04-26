using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public class UIElement
    {
        public string resource;
        public bool cache;
        public GameObject instance;
    }

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        UIResources.Add(typeof(UITestingWindow),new UIElement() { resource = "UI/UITester", cache = true});
        UIResources.Add(typeof(UIBagView), new UIElement() { resource = "UI/UIBagView", cache = false});
    }

    public T Show<T>()
    {
        var type = typeof(T);
        if(UIResources.ContainsKey(type))
        {
            var UIInfo = UIResources[type];
            if (UIInfo.instance != null)
            {
                UIInfo.instance.SetActive(true);
            } else
            {
                var prefab = Resources.Load(UIInfo.resource);
                if(prefab == null)
                {
                    return default(T);
                }
                UIInfo.instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return UIInfo.instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        if(UIResources.ContainsKey(type))
        {
            var info = UIResources[type];
            if(info.cache)
            {
                info.instance.SetActive(false);
            } else
            {
                GameObject.Destroy(info.instance);
                info.instance = null;
            }
        }
    }
}
