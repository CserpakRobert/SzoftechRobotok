﻿using Persistence.DataTypes;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ViewModel.ViewModel;

namespace View.Grid
{
    /// <summary>
    /// Interaction logic for MapGoal.xaml
    /// </summary>
    public partial class MapGoal : Canvas
    {
        private Dictionary<int, int> _robotIdToCanvasIndex = new();
        private SolidColorBrush _brush;
        public MapGoal()
        {
            InitializeComponent();
            _brush = new(Color.FromRgb(251, 171, 9));
            _brush.Freeze();
        }

        public void SetDataContext(MainWindowViewModel viewModel)
        {
            this.DataContext = viewModel;
            viewModel.MapLoaded += new EventHandler(
                (s, _) => App.Current?.Dispatcher.Invoke((Action)delegate { ClearGoals(s); })
                );
            viewModel.GoalChanged += new EventHandler<Goal>(
                (r, g) => App.Current?.Dispatcher.Invoke((Action)delegate { RefreshGoal((Robot)r!, g); })
                );
        }

        #region Private methods
        private void ClearGoals(object? sender)
        {
            if (sender == null)
                return;

            MapCanvas.Children.Clear();
            _robotIdToCanvasIndex.Clear();
        }
        private void RefreshGoal(Robot robot, Goal goal)
        {
            if (_robotIdToCanvasIndex.ContainsKey(robot.Id))
            {
                MapCanvas.Children.RemoveAt(_robotIdToCanvasIndex[robot.Id]);
                _robotIdToCanvasIndex.Remove(robot.Id);
            }

            Debug.WriteLine("eeere");

            if (!goal.IsAssigned)
                return;          

            System.Windows.Controls.Grid grid = new()
            {
                Width = GridConverterFunctions.unit,
                Height = GridConverterFunctions.unit,
                ToolTip = goal.Id,
                Margin = new Thickness(
                    GridConverterFunctions.unit * goal.Position.X,
                    GridConverterFunctions.unit * goal.Position.Y,
                    0,
                    0)
            };
            ToolTipService.SetInitialShowDelay(grid, 0);
            ToolTipService.SetShowDuration(grid, 9999999);
            ToolTipService.SetBetweenShowDelay(grid, 0);

            System.Windows.Shapes.Rectangle rectangle = new()
            {
                Fill = _brush,
                Margin = new Thickness(0.5)
            };

            System.Windows.Controls.TextBlock textBlock = new()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                Text = goal.Id.ToString()
            };

            grid.Children.Add(rectangle);
            grid.Children.Add(textBlock);

            _robotIdToCanvasIndex[robot.Id] = MapCanvas.Children.Count;
            MapCanvas.Children.Add(grid);

        }
        #endregion
    }
}
