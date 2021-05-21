using Services;
using SkillBridge.Message;
using UnityEngine.UI;

public class UIGuildApplicantItem : ListView.ListViewItem
{
    public Text cName;
    public Text level;
    public Text occupation;

    public NGuildApplicantInfo info;

    public void SetMemberInfo(NGuildApplicantInfo memberInfo)
    {
        info = memberInfo;
        cName.text = info.Name;
        level.text = info.Level.ToString();
        occupation.text = info.Class.ToString();
    }

    public void OnClickApprove()
    {
        MessageBox.Show(string.Format("Do You Wanna Approve [{0}] On Joining Your Guild?", info.Name), "Approve Applicant", MessageBoxType.Confirm, "Approve", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendGuildApplicant(true,info);
        };
    }

    public void OnClickDeny()
    {
        MessageBox.Show(string.Format("Do You Wanna Deny [{0}] On Joining Your Guild?", info.Name), "Approve Applicant", MessageBoxType.Confirm, "Deny", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendGuildApplicant(false, info);
        };
    }
}
