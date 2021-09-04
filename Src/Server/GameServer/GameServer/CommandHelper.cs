using GameServer.Managers;
using GameServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class CommandHelper
    {
        public static void Run()
        {
            bool run = true;
            while (run)
            {
                Console.Write(">");
                string line = Console.ReadLine().ToLower().Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    Help();
                    continue;
                }
                try
                {
                    string[] cmd = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    switch (cmd[0])
                    {
                        case "addexp":
                            AddExp(int.Parse(cmd[1]), int.Parse(cmd[2]));
                            break;
                        case "exit":
                            run = false;
                            break;
                        default:
                            Help();
                            break;
                    }
                }catch(Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }

        public static void AddExp(int cid, int expAmt)
        {
            var chara = CharacterManager.Instance.GetCharacter(cid);
            if(chara != null)
            {
                chara.AddExp(expAmt);
            } else
            {
                var tChara = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID == cid);
                if(tChara == null)
                {
                    Console.Write("Target Character Does Not Exist");
                    return;
                }
                tChara.Exp += expAmt;
                DBService.Instance.Save();
            }
        }

        public static void Help()
        {
            Console.Write(@"
Help:
    addexp <character id> <exp amount> Add Exp To Target Character
    exit    Exit Game Server
    help    Show Help
");
        }
    }
}
