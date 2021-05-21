using System.Linq;
using System.Collections.Generic;
using Common.Data;
using Models;
using SkillBridge.Message;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public List<NGuildMemberInfo> members { get { return guildInfo != null ? guildInfo.Members : null; } }
        public List<NGuildApplicantInfo> applicants { get { return guildInfo != null ? guildInfo.Applicants : null; } }
        public NGuildInfo guildInfo;
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return guildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            guildInfo = guild;
            if (guild == null) return;
            myMemberInfo = members.FirstOrDefault(m => m.characterId == User.Instance.CurrentCharacter.Id);
        }

        public void ShowGuild()
        {
            if (HasGuild)
                UIManager.Instance.Show<UIGuild>();
            else
            {
                var prompt = UIManager.Instance.Show<UINoneGuildPrompt>();
                prompt.OnClose += OnNoneGuildPromptClose;
            }
        }

        private void OnNoneGuildPromptClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if(result == UIWindow.WindowResult.Confirm)
            {
                UIManager.Instance.Show<UICreateGuildPrompt>();
            } else if(result == UIWindow.WindowResult.No)
            {
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
