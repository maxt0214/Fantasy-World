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
        public Transform playerTransform
        {
            get
            {
                if (User.Instance.currentCharacterObj == null) return null;
                return User.Instance.currentCharacterObj.transform;
            }
        }

        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.currMap.MiniMap);
        }
    }
}
