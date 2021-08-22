using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Models;
using Common.Data;

namespace Managers {
    public class GameObjectManager : MonoSingleton<GameObjectManager>
    {
        Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>(); //All characters in the current scene

        protected override void OnStart()
        {
            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
            CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
        }

        private void OnDestroy()
        {
            CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
            CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        }

        private IEnumerator InitGameObjects()
        {
            foreach (var chara in CharacterManager.Instance.characters.Values)
            {
                CreateCharacterObject(chara);
                yield return null;
            }
        }

        private void OnCharacterEnter(Character chara)
        {
            CreateCharacterObject(chara);
        }

        private void OnCharacterLeave(Character chara)
        {
            if (!characters.ContainsKey(chara.entityId))
                return;

            if(characters[chara.entityId] != null)
            {
                Destroy(characters[chara.entityId]);
                characters.Remove(chara.entityId);
            }
        }

        private void CreateCharacterObject(Character chara)
        {
            if (!characters.ContainsKey(chara.entityId) || characters[chara.entityId] == null)
            {
                Object obj = Resloader.Load<Object>(chara.Define.Resource);
                if(obj == null)
                {
                    Debug.LogErrorFormat("Character[{0}] Resource{1} does not exist!", chara.Define.TID, chara.Define.Resource);
                    return;
                }

                GameObject gameObject = (GameObject)Instantiate(obj,transform);
                gameObject.name = "Character_" + chara.Id + "_" + chara.Name;
                characters[chara.entityId] = gameObject;
                
                Debug.LogFormat("Character[{0}] TID:{1} ID:{2} is created", chara.Name, chara.Define.TID, chara.entityId);
                UIWorldElementManager.Instance.AddCharacterNameBar(gameObject.transform, chara);
            }
            InitGameObject(characters[chara.entityId], chara);
        }

        private void InitGameObject(GameObject gameObject, Character chara)
        {
            gameObject.transform.position = GameObjectTool.LogicUnitToWorld(chara.position);
            gameObject.transform.forward = GameObjectTool.LogicUnitToWorld(chara.direction);

            var ec = gameObject.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.entity = chara;
                ec.isLocalPlayer = chara.IsLocalPlayer;
                ec.Ride(chara.Info.Ride);
                chara.Controller = ec;
            }

            var pc = gameObject.GetComponent<PlayerInputController>();
            if (pc != null)
            {
                if (chara.IsLocalPlayer)
                {
                    User.Instance.currentCharacterObj = pc;
                    MainPlayerCamera.Instance.player = gameObject;
                    pc.enabled = true;
                    pc.characterEntity = chara;
                    pc.entityController = ec;
                }
                else
                {
                    pc.enabled = false;
                }
            }
        }

        public RideController LoadRide(int rideId, Transform parent)
        {
            RideDefine ride;
            DataManager.Instance.Rides.TryGetValue(rideId,out ride);
            if(ride == null)
            {
                Debug.LogErrorFormat("RideDefine[{0}] Does Not Exit",rideId);
                return null;
            }
            Object obj = Resloader.Load<Object>(ride.Resource);
            if(obj == null)
            {
                Debug.LogErrorFormat("Ride[{0}] Resource:{1} Does Not Exist", rideId, ride.Resource);
                return null;
            }
            GameObject gameObj = (GameObject)Instantiate(obj,parent);
            gameObj.name = "Ride_" + ride.ID + "_" + ride.Name;
            return gameObj.GetComponent<RideController>();
        }
    }
}