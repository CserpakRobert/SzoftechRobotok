﻿using Model.Interfaces;
using Persistence.DataTypes;
using Persistence.Interfaces;

namespace Model.Mediators.ReplayMediatorUtils
{
    public class ReplayController : IController
    {
        private readonly ILoadLogDataAccess loadLogDataAccess = null!;
        private SimulationData simulationData = null!;
        private List<TaskEvent[]> taskEvents;
        private int[] taskEventIterator = null!;
        public string Name => "ReplayController";

        public event EventHandler<IControllerEventArgs>? FinishedTask;

        public ReplayController(ILoadLogDataAccess loadLogDataAccess)
        {
            this.loadLogDataAccess = loadLogDataAccess;
            taskEvents = loadLogDataAccess.GetTaskEvents();
        }

        public void CalculateOperations(TimeSpan timeSpan)
        {
            for (int i = 0; i < taskEventIterator.Length; i++)
            {
                var robot = simulationData.Robots[i];
                while (taskEventIterator[i] < taskEvents[i].Length && taskEvents[i][taskEventIterator[i]].step <= simulationData.Step)
                {
                    TaskEvent taskEvent = taskEvents[i][taskEventIterator[i]];
                    Goal goal = simulationData.Goals[taskEvent.taskId];
                    if(taskEvent.eventType == TaskEventType.assigned)
                        robot.CurrentGoal = goal;
                    else
                        robot.CurrentGoal = null;
                    
                    //Goal.OnGoalsChanged();
                    taskEventIterator[i]++;
                }
            }

            OnTaskFinished(loadLogDataAccess.GetRobotOperations(simulationData.Step));
        }

        public void InitializeController(SimulationData simulationData, TimeSpan timeSpan, ITaskDistributor distributor)
        {
            this.simulationData = simulationData;
            taskEventIterator = Enumerable.Repeat(0, simulationData.Robots.Count).ToArray();
        }

        public IController NewInstance()
        {
            return new ReplayController(loadLogDataAccess);
        }

        private void OnTaskFinished(RobotOperation[] result)
        {
            FinishedTask?.Invoke(this, new(result));
        }
    }
}
