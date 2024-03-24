﻿using RobotokModel.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotokModel.Model.Distributors
{
    public class DemoDistributor : ITaskDistributor
    {
        private SimulationData simulationData;
        public DemoDistributor(SimulationData simulationData)
        {
            this.simulationData = simulationData;
        }

        public bool AllTasksAssigned { get; private set; } = false;

        /// <summary>
        /// Assignes the first available goal.
        /// If there is no available goal, assigns <c>null</c>
        /// </summary>
        /// <param name="robot"></param>
        public void AssignNewTask(Robot robot)
        {
            for(int i=0; i< simulationData.Goals.Count; i++)
            {
                Goal goal = simulationData.Goals[i];

                if(goal.IsAssigned)
                    continue;
                robot.CurrentGoal = goal;
                goal.IsAssigned = true;
                if(i == simulationData.Goals.Count-1) AllTasksAssigned = true;
                return;
            }
            AllTasksAssigned = true;
            robot.CurrentGoal = null;
        }

        public ITaskDistributor NewInstance(SimulationData simulationData)
        {
            return new DemoDistributor(simulationData);
        }
    }
}
