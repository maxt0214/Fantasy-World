using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    public GameObject nameBarPrefab;

    public Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void AddCharacterNameBar(Transform owner, Character chara)
    {
        var newNameBar = Instantiate(nameBarPrefab,transform);
        newNameBar.name = chara.Name + "_" + chara.entityId + "_Name_Bar";

        var UIelement = newNameBar.GetComponent<UIWorldElement>();
        if (UIelement == null)
            Debug.LogErrorFormat("{0} does not contain a UIWorldElement", nameBarPrefab.name);
        UIelement.owner = owner;

        var UINameBar = newNameBar.GetComponent<UINameBar>();
        if(UINameBar == null)
            Debug.LogErrorFormat("{0} does not contain a UINameBar", nameBarPrefab.name);
        UINameBar.character = chara;

        newNameBar.SetActive(true);

        elements[owner] = newNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (elements.ContainsKey(owner))
        {
            Destroy(elements[owner]);
            elements.Remove(owner);
        }
    }
}
