using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    public EntityEffectManager EffectMgr;

    public void PlayEffect(string name)
    {
        Debug.LogFormat("PlayEffect:{0}", name);
        EffectMgr.PlayEffect(name);
    }

    public void PlaySound(string name)
    {
        Debug.LogFormat("PlaySound:{0}", name);
    }
}
