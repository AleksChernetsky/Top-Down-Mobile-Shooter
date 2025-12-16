using System;
using TowerDefence.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TowerDefence.Systems
{
    /// <summary>
    /// Input service providing both direct access to Input System
    /// and convenience events for common tower defense inputs.
    /// </summary>
    public interface IInputService : IService
    {
        // Direct access to Input System (for advanced usage)
        Input InputActions { get; }
        
        void Enable();
        void Disable();
        bool IsEnabled { get; }
        
        // Convenience events (fire only when NOT over UI)
        event Action<Vector2> OnTap;
        event Action<Vector2> OnHold;
        event Action<Vector2> OnTouchMoved;
        
        // Utility
        bool IsPointerOverUI();
        Vector2 GetTouchPosition();
    }
}
