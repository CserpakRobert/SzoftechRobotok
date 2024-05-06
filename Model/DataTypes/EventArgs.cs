﻿
using Persistence.DataTypes;

namespace Model.DataTypes
{
    public class SimulationStateEventArgs : EventArgs
    {
        public required SimulationState SimulationState { get; init; }
        public required bool IsReplayMode { get; init; }
    }

    public class RobotsMovedEventArgs : EventArgs
    {
        public required TimeSpan TimeSpan { get; init; }
        public required RobotOperation[] RobotOperations { get; init; }
    }
}