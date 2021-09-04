using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MainUIEnable : PlayableAsset
{
    public bool active = true;
    public ActivationControlPlayable.PostPlaybackState postPlayBack;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<MainUIEnabler>.Create(graph);
        playable.GetBehaviour().active = active;
        playable.GetBehaviour().postPlayBack = postPlayBack;
        return playable;
    }
}
