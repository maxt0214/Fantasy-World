using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    public GameObject nameBarPrefab;
    public GameObject npcStatPrefab;

    public Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    public Dictionary<Transform, GameObject> elementStats = new Dictionary<Transform, GameObject>();

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

        elementNames[owner] = newNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (elementNames.ContainsKey(owner))
        {
            Destroy(elementNames[owner]);
            elementNames.Remove(owner);
        }
    }

    public void AddNpcStatus(Transform owner, NPCQuestStatus stat)
    {
        if(elementStats.ContainsKey(owner))
        {
            elementStats[owner].GetComponent<UIQuestStatus>().SetQuestStat(stat);
        } else
        {
            GameObject gameObject = Instantiate(npcStatPrefab, transform);
            gameObject.name = "NpcQuestStatus " + owner.name;
            gameObject.GetComponent<UIWorldElement>().owner = owner;
            gameObject.GetComponent<UIQuestStatus>().SetQuestStat(stat);
            gameObject.SetActive(true);
            elementStats[owner] = gameObject;
        }
    }

    public void RemoveNpcStatus(Transform owner)
    {
        if (elementStats.ContainsKey(owner))
        {
            Destroy(elementStats[owner]);
            elementStats.Remove(owner);
        }
    }
}
