using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameListener
{
    public ClipDictionary AudioClips;

    void Start()
    {
        // Listen to all events
        foreach( EventType ev in Enum.GetValues( typeof( EventType ) ) )
            EventManager.Instance.AddGameListener( ev, this );
    }

    public void OnEvent( EventType eventType, Component sender, object param = null )
    {
        var clip = AudioClips.GetObject( eventType ) as AudioClip;
        if( clip != null )
        {
            //Debug.LogFormat( "Playing: {0}", clip.name );
            AudioSource.PlayClipAtPoint( clip, Vector3.zero );
        }
    }

    [Serializable]
    public class ClipDictionary : EnumDictionary
    {
        public ClipDictionary()
            : base( typeof( EventType ), typeof( AudioClip ) )
        { }
    }
}