using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Models;

namespace Managers {
    public class GameObjectManager : MonoBehaviour
    {
        Dictionary<int, Character> characters = new Dictionary<int, Character>(); //All characters in the current scene

        private void Start()
        {
            StartCoroutine(InitGameObjects());
            CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        }

        private void OnDestroy()
        {
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

                GameObject gameObject = (GameObject)Instantiate(obj);
                gameObject.name = "Character_" + chara.entityId + "_" + chara.Name;

                gameObject.transform.position = GameObjectTool.LogicUnitToWorld(chara.position);
                gameObject.transform.forward = GameObjectTool.LogicUnitToWorld(chara.direction);

                var ec = gameObject.GetComponent<EntityController>();
                if(ec != null)
                {
                    ec.entity = chara;
                    ec.isLocalPlayer = chara.IsPlayer;
                }

                var pc = gameObject.GetComponent<PlayerInputController>();
                if (pc != null)
                {
                    if(chara.Info.Id == User.Instance.CurrentCharacter.Id)
                    {
                        User.Instance.currentCharacterObj = gameObject;
                        MainPlayerCamera.Instance.player = gameObject;
                        pc.enabled = true;
                        pc.characterEntity = chara;
                        pc.entityController = ec;
                    } else
                    {
                        pc.enabled = false;
                    }
                }
                if(Debug.isDebugBuild) Debug.LogFormat("Character[{0}] TID:{1} ID:{2}", chara.Name, chara.Define.TID, chara.entityId);
                UIWorldElementManager.Instance.AddCharacterNameBar(gameObject.transform, chara);
            }
        }
    }
}