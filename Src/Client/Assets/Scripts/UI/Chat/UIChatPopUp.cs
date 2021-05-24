using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using Managers;

public class UIChatPopUp : UIWindow, IDeselectHandler
{
    public int targetId;
    public string targetName;

    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(gameObject))
            return;
        Close(WindowResult.None);
    }

    public void OnEnable()
    {
        GetComponent<Selectable>().Select();
        Root.transform.position = new Vector3(Input.mousePosition.x, Mathf.Clamp(Input.mousePosition.y,150f,Input.mousePosition.y), Input.mousePosition.z);
    }

    public void OnClickChat()
    {
        ChatManager.Instance.StartPrivateChat(targetId, targetName);
        Close(WindowResult.No);
    }

    public void OnClickAddFriend()
    {

        Close(WindowResult.No);
    }

    public void OnClickInviteToTeam()
    {

        Close(WindowResult.No);
    }
}
