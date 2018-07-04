using System.Collections;
using UnityEngine;
using Doodle.Game;

//the enum that defines all the game events
public enum EventType
{
    FlagCaptured,
    FlagGrabbed,
    GameWon,
    GameTied,
    TimeUp,
    PlayerDied,
    PlayerJump,
    BlockPlaced,
	GameStarted,
	ReturnToMenu,
	BlockDestroyed,
    Smoke,
    Explosion,
    JumpPad,
};

//interface for the objects implementing the game events
public interface IGameListener
{
    //the function called when the event fires
    void OnEvent( EventType eventType, Component sender, object param = null );
}
