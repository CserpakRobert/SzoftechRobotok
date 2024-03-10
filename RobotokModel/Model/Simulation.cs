﻿using RobotokModel.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RobotokModel.Model
{
    public class Simulation
    {
        private SimulationData SimulationData { get; set; }
        private List<List<RobotOperation>> ExecutedOperations { get; set; } = new List<List<RobotOperation>>();
        private Log CurrentLog { get; set; }
        private ITaskDistributor Distributor { get; set; }
        private IController Controller { get; set; }
        private int RemainingSteps { get; set; }
        public EventHandler<RobotMove> RobotMovedEvent { get; set; }
        public EventHandler<int> GoalFinished { get; set; }


        public Simulation(Config config, string Strategy, int StepLimit)
        {
            throw new NotImplementedException();

        }
        public void StepForward()
        {
            throw new NotImplementedException();
        }

        public void StepBackward()
        {
            throw new NotImplementedException();
        }
        public void StartSimulation()
        {
            throw new NotImplementedException();
        }

        private void SimulationStep()
        {

            var operations = Controller.NextStep();
            for (int i = 0; i < Robot.Robots.Count; i++)
            {
                var robot = Robot.Robots[i];
                switch (operations[i])
                {
                    case RobotOperation.Forward:
                        var newPos = PoistionInDirection(robot.Rotation, robot.Position);
                        if (SimulationData.Map[newPos.X, newPos.Y].IsPassable)
                        {
                            MoveRobotToNewPosition(robot, newPos, operations[i]);
                        }
                        else if (newPos.X == robot.CurrentGoal.Position.X && newPos.Y == robot.CurrentGoal.Position.Y)
                        {
                            MoveRobotToNewPosition(robot, newPos, operations[i]);
                            GoalFinished.Invoke(this, robot.CurrentGoal.Id);
                        }
                        else if (SimulationData.Map[newPos.X, newPos.Y] is Robot)
                        {
                            throw new NotImplementedException();
                        }

                        break;
                    case RobotOperation.Clockwise:
                        robot.Rotation.RotateClockWise();
                        break;
                    case RobotOperation.CounterClockwise:
                        robot.Rotation.RotateCounterClockWise();
                        break;
                    case RobotOperation.Backward:
                        break;
                    case RobotOperation.Wait:
                        break;
                }
            }
            throw new NotImplementedException();

        }
        private void MoveRobotToNewPosition(Robot robot, Position newPosition, RobotOperation operaition)
        {
            SimulationData.Map[newPosition.X, newPosition.Y] = robot;
            SimulationData.Map[robot.Position.X, robot.Position.Y] = new EmptyTile();
            robot.Position = newPosition;

            var rmEvent = new RobotMove();
            rmEvent.Position = newPosition;
            rmEvent.Operation = operaition.ToChar();
            RobotMovedEvent.Invoke(this, rmEvent);
        }
        private Position PoistionInDirection(Direction rotation, Position position)
        {
            var newPosition = new Position();
            switch (rotation)
            {
                case Direction.Left:
                    newPosition.X = position.X - 1;
                    newPosition.Y = position.Y;
                    break;
                case Direction.Up:
                    newPosition.X = position.X;
                    newPosition.Y = position.Y + 1;
                    break;
                case Direction.Right:
                    newPosition.X = position.X;
                    newPosition.Y = position.Y + 1;
                    break;
                case Direction.Down:
                    newPosition.X = position.X;
                    newPosition.Y = position.Y - 1;
                    break;
            }
            return newPosition;
        }

        public void StopSimulation()
        {
            throw new NotImplementedException();
        }
        public void PauseSimulation()
        {
            throw new NotImplementedException();
        }
        public void SetSimulationSpeed()
        {
            throw new NotImplementedException();
        }
        public void JumpToStep(int step)
        {
            throw new NotImplementedException();
        }
        public void GetLog(Log log)
        {
            throw new NotImplementedException();
        }
        private int GoalsRemaining()
        {
            throw new NotImplementedException();
        }
        private RobotOperation ReverseOperation(RobotOperation operation)
        {
            switch (operation)
            {
                case RobotOperation.Forward:
                    return RobotOperation.Backward;
                case RobotOperation.Clockwise:
                    return RobotOperation.CounterClockwise;
                case RobotOperation.CounterClockwise:
                    return RobotOperation.Clockwise;
                case RobotOperation.Backward:
                    return RobotOperation.Forward;
                case RobotOperation.Wait:
                    return RobotOperation.Wait;
            }
            return RobotOperation.Wait;
        }


    }
}
