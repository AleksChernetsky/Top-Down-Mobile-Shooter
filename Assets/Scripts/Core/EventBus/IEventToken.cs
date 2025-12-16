using System;

namespace TowerDefence.Core
{
    public interface IEventToken
    {
        Type EventType { get; }
    }
}

