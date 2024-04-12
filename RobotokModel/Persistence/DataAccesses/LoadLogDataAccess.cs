﻿using RobotokModel.Model;
using RobotokModel.Model.Extensions;
using RobotokModel.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace RobotokModel.Persistence.DataAccesses
{
    public class LoadLogDataAccess : ILoadLogDataAccess
    {
        //do it just like ConfigDataAccess :)

        #region Private fields

        private Uri baseUri;
        private string path;
        private SimulationData simulationData = null!;
        private Log log;
        #endregion

        #region Constructor
        public LoadLogDataAccess(string path)
        {
            this.path = path;
            baseUri = new(path);
            log = LoadLog(path);
        }

        #endregion

        #region Public methods
        public SimulationData GetInitialSimulationData()
        {
            // azt még nemtom hogy ezt honnan kapjuk meg
            throw new NotImplementedException();
        }

        public RobotOperation[] GetRobotOperations(int step)
        {
            List<RobotOperation> operations = new List<RobotOperation>();
            foreach(List<RobotOperation> robotPath in log.ActualPaths)
            {
                operations.Add(robotPath[step]);
            }
            return operations.ToArray();
        }

        public TaskEvent[] GetTaskEvents()
        {
            int i = 0;
            int count = 0;
            foreach(List<TaskEvent> t in log.Events)
            {
                count += t.Count;
            }
            TaskEvent[] taskEvents = new TaskEvent[count];
            foreach (List<TaskEvent> ts in log.Events)
            {
                foreach (TaskEvent task in ts)
                {
                    taskEvents[i] = task;
                    i++;
                }
            }
            return taskEvents;
        }

        public IDataAccess NewInstance(string filePath)
        {
            return new LoadLogDataAccess(filePath);
        }

        public Log LoadLog(string path) // TODO: remove unnecessary elements
        {
            string jsonString = File.ReadAllText(path);
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new JsonStringEnumConverter());
            ExternalLog? externalLog = JsonSerializer.Deserialize<ExternalLog>(jsonString, options) ?? throw new NotImplementedException("Serialization of config file was unsuccesful!");
            Log log = new Log();

            log.ActionModel = externalLog.ActionModel;
            log.AllValid = externalLog.AllValid;
            log.TeamSize = externalLog.TeamSize;
            log.NumTaskFinished = externalLog.NumTaskFinished;
            log.SumOfCost = externalLog.SumOfCost;
            log.MakeSpan = externalLog.MakeSpan;
            log.ActualPaths = new List<List<RobotOperation>>();
            log.PlannerPaths = new List<List<RobotOperation>>();
            foreach (string robot in externalLog.ActualPaths)
            {
                List<RobotOperation> robotOperation = new List<RobotOperation>();
                RobotOperation operation;
                foreach (string op in robot.Split(','))
                {
                    operation = op[0].ToRobotOperation();
                    robotOperation.Add(operation);
                }
                log.ActualPaths.Add(robotOperation);
            }
            foreach (string robot in externalLog.PlannerPaths)
            {
                List<RobotOperation> robotOperation = new List<RobotOperation>();
                RobotOperation operation;
                foreach (string op in robot.Split(','))
                {
                    operation = op[0].ToRobotOperation();
                    robotOperation.Add(operation);
                }
                log.PlannerPaths.Add(robotOperation);
            }
            log.Events = new List<List<TaskEvent>>();
            foreach (List<List<object>> round in externalLog.Events)
            {
                List<TaskEvent> taskEvents = new List<TaskEvent>();
                foreach (List<object> task in round)
                {
                    string? t0 = task[0].ToString();
                    string? t1 = task[1].ToString();
                    if (t0 == null || t1 == null)
                    {
                        throw new Exception();
                    }

                    int taskID = int.Parse(t0);
                    int robotID = int.Parse(t1);
                    if (task[2].ToString() == "finished")
                    {
                        taskEvents.Add(new TaskEvent(taskID, robotID, TaskEventType.finished));
                    }
                    else
                    {
                        taskEvents.Add(new TaskEvent(taskID, robotID, TaskEventType.assigned));
                    }

                }
                log.Events.Add(taskEvents);
            }
            foreach (List<int> task in externalLog.Tasks)
            {
                Goal g = new Goal
                {
                    Id = task[0],
                    Position = new Position { X = task[1], Y = task[2] },
                };
                log.Tasks.Add(g);
            }
            int robotIDStart = 0;
            foreach (List<object> startingRobot in externalLog.Start)
            {
                string? x = startingRobot[0].ToString();
                string? y = startingRobot[1].ToString();
                string? direction = startingRobot[2].ToString();
                if (direction == null)
                {
                    throw new Exception("Direction was null while trying to parse in array: start.");
                }
                if (x == null || y == null)
                {
                    throw new Exception("Position was null while trying to parse in array: start.");
                }
                Robot r = new Robot
                {
                    Id = robotIDStart,
                    Position = new Position { X = int.Parse(x), Y = int.Parse(y) },
                    Rotation = ToDirection(direction)
                };
                robotIDStart++;
                log.Start.Add(r);
            }
            log.Errors = null!; // visszajátszásban nem lesz rá szükség
            /*
            foreach (List<Object> error in externalLog.Errors)
            {
                string? e0 = error[0].ToString();
                string? e1 = error[1].ToString();
                string? e2 = error[2].ToString();
                if (e0 == null || e1 == null || e2 == null)
                {
                    throw new Exception();
                }

                int robot1 = int.Parse(e0);
                int robot2 = int.Parse(e1);
                int stepID = int.Parse(e2);
                string? errorType = error[2].ToString();
                if(errorType == null)
                {
                    throw new Exception();
                }

            }
            */


            return log;
        }


        #endregion

        #region Private methods
        public Direction ToDirection(string rawDirection)
        {
            return rawDirection switch
            {
                "W" => Direction.Left,
                "N" => Direction.Up,
                "E" => Direction.Right,
                "S" => Direction.Down,
                _ => throw new Exception("Parsing in direction was not succesful!"),
            };
        }
        #endregion
    }
}