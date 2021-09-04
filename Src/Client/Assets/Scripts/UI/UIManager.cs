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
        UIResources.Add(typeof(UIBagView), new UIElement() { resource = "UI/UIBagView", cache = false });
        UIResources.Add(typeof(UIEquip), new UIElement() { resource = "UI/UIEquipView", cache = true });
        UIResources.Add(typeof(UIShop), new UIElement() { resource = "UI/UIShopView", cache = false });
        UIResources.Add(typeof(UIQuestSystem), new UIElement() { resource = "UI/UIQuestView", cache = true });
        UIResources.Add(typeof(UIQuestDialog), new UIElement() { resource = "UI/UIQuestDialog", cache = true });
        UIResources.Add(typeof(UIFriendView), new UIElement() { resource = "UI/UIFriendView", cache = true });
        UIResources.Add(typeof(UIGuildList), new UIElement() { resource = "UI/UIGuildList", cache = true });
        UIResources.Add(typeof(UIGuild), new UIElement() { resource = "UI/UIGuild", cache = true });
        UIResources.Add(typeof(UINoneGuildPrompt), new UIElement() { resource = "UI/UINoneGuildPrompt", cache = true });
        UIResources.Add(typeof(UICreateGuildPrompt), new UIElement() { resource = "UI/UICreateGuildPrompt", cache = true });
        UIResources.Add(typeof(UIGuildApplicantList), new UIElement() { resource = "UI/UIGuildApplicantList", cache = true });
        UIResources.Add(typeof(UISetting), new UIElement() { resource = "UI/UISetting", cache = true });
        UIResources.Add(typeof(UIChatPopUp), new UIElement() { resource = "UI/UIChatPopUp", cache = true });
        UIResources.Add(typeof(UIRide), new UIElement() { resource = "UI/UIRide", cache = true });
        UIResources.Add(typeof(UISystemConfig), new UIElement() { resource = "UI/UISystemConfig", cache = true });
        UIResources.Add(typeof(UISkillView), new UIElement() { resource = "UI/UISkillView", cache = true });
        UIResources.Add(typeof(UIDungeonView), new UIElement() { resource = "UI/UIDungeonView", cache = true });
    }

    public T Show<T>()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
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
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        if (UIResources.ContainsKey(type))
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

    public void Close<T>()
    {
        Close(typeof(T));
    }
}
