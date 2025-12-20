using TowerDefence.Systems;
using UnityEngine;

namespace TowerDefence.Game
{
    /// <summary>
    /// Fired when player requests to start a game level
    /// </summary>
    public struct StartGameRequestedEvent
    {
    }

    /// <summary>
    /// Fired when player requests to return to main menu
    /// </summary>
    public struct ReturnToMenuRequestedEvent
    {
    }

    /// <summary>
    /// Fired when player requests to pause the game
    /// </summary>
    public struct PauseGameRequestedEvent
    {
    }

    /// <summary>
    /// Fired when player requests to resume the game
    /// </summary>
    public struct ResumeGameRequestedEvent
    {
    }

    /// <summary>
    /// Fired when game is over (win or loss)
    /// </summary>
    public struct GameOverEvent { public Team Winner; }
    public struct CharacterDied { public GameObject Character; }
}

