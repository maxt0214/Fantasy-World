using UnityEngine;
using UnityEngine.UI;
using Managers;
using Models;

public class UISKillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;

    private void Start()
    {
    }

    public void RefreshUI()
    {
        if (User.Instance.currentCharacter == null) return;
        var skills = User.Instance.currentCharacter.skillMgr.skills;
        int indx = 0;
        foreach (var skill in skills)
        {
            slots[indx].SetSkillSlot(skill);
            indx++;
        }
    }
}
