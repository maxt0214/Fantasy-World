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
        public UIMiniMap miniMap;
        private Collider miniMapBoundingBox;
        public Collider MiniMapBoundingBox { get { return miniMapBoundingBox; } }
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

        public void UpdateMiniMap(Collider boundingBox)
        {
            miniMapBoundingBox = boundingBox;
            if (miniMap != null)
                miniMap.UpdateMap();
        }
    }
}
