using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamMemberItem : ListView.ListViewItem
{
    public Text nickName;
    public Image classIcon;
    public Image leaderFlag;

    public Image background;
    public Sprite select;
    public Sprite normal;

    public int idx;

    public NCharacterInfo memberInfo;

    public override void OnSelected(bool ifSelected)
    {
        background.overrideSprite = ifSelected ? select : normal;
    }

    public void SetMemberInfo(int indx, NCharacterInfo charaInfo, bool isLeader)
    {
        idx = indx;
        memberInfo = charaInfo;
        leaderFlag.gameObject.SetActive(isLeader);
        nickName.text = memberInfo.Level.ToString().PadRight(4) + memberInfo.Name;
        classIcon.overrideSprite = SpriteManager.Instance.characterIcons[(int)memberInfo.Class - 1];
    }
}
