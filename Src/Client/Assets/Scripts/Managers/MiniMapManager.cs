using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Models;

namespace Managers
{
    class MiniMapManager : Singleton<MiniMapManager>
    {
        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.currMap.MiniMap);
        }
    }
}
