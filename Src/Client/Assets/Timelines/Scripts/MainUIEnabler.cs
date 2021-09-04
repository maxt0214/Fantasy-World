using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// A behaviour that is attached to a playable
public class MainUIEnabler : PlayableBehaviour
{
    public bool active;
    private bool activeState;
    public ActivationControlPlayable.PostPlaybackState postPlayBack;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        if (UIMain.Instance == null) return;
        activeState = UIMain.Instance.Show;
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable,info);

        if (UIMain.Instance == null) return;
        UIMain.Instance.Show = active;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (UIMain.Instance == null) return;
        if (postPlayBack == ActivationControlPlayable.PostPlaybackState.Active)
            UIMain.Instance.Show = true;
        else if (postPlayBack == ActivationControlPlayable.PostPlaybackState.Inactive)
            UIMain.Instance.Show = false;
        else
            UIMain.Instance.Show = activeState;
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
