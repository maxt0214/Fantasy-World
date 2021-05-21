using Managers;
using Services;
using SkillBridge.Message;
using UnityEngine;

public class UIGuildApplicantList : UIWindow
{
    public GameObject itemPrefab;

    public ListView applicantList;
    public Transform applicantRoot;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += RefreshUI;
        GuildService.Instance.SendGuildListRequest();
        RefreshUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearList();
        InitList();
    }

    private void ClearList()
    {
        applicantList.Clear();
    }

    private void InitList()
    {
        foreach (var applicant in GuildManager.Instance.applicants)
        {
            var gameObj = Instantiate(itemPrefab, applicantRoot);
            var applicantItem = gameObj.GetComponent<UIGuildApplicantItem>();
            applicantItem.SetMemberInfo(applicant);
            applicantList.AddItem(applicantItem);
        }
    }
}
