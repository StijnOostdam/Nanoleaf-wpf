﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Hardcodet.Wpf.TaskbarNotification;

using NLog;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Views.Options;
using Winleafs.Wpf.Views.Scheduling;

namespace Winleafs.Wpf.Views.MainWindows
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Winleafs.Wpf.Views.Layout;
    using Winleafs.Wpf.Views.Popup;
    using Winleafs.Wpf.Views.Setup;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                SelectedDeviceChanged();
            }
        }

        public ObservableCollection<string> DeviceNames { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            LayoutDisplay.SetWithAndHeight((int)LayoutDisplay.Width, (int)LayoutDisplay.Height);
            LayoutDisplay.DrawLayout();

            var taskbarIcon = (TaskbarIcon)FindResource("NotifyIcon"); //https://www.codeproject.com/Articles/36468/WPF-NotifyIcon-2
            taskbarIcon.DoubleClickCommand = new TaskbarDoubleClickCommand(this);

            UpdateDeviceNames();
            SelectedDevice = UserSettings.Settings.ActiveDevice.Name;

            BuildScheduleList();

            OverrideScheduleUserControl.MainWindow = this;

            DataContext = this;
        }

        public void UpdateDeviceNames()
        {
            DeviceNames = new ObservableCollection<string>(UserSettings.Settings.Devices.Select(d => d.Name));
        }

        private void SelectedDeviceChanged()
        {
            if (_selectedDevice != null)
            {
                UserSettings.Settings.SetActiveDevice(_selectedDevice);

                BuildScheduleList();

                LayoutDisplay.DrawLayout();

                UpdateCurrentEffectLabelsAndLayout();
            }
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Add);
            scheduleWindow.ShowDialog();
        }

        public void AddedSchedule(Schedule schedule)
        {
            UserSettings.Settings.AddSchedule(schedule, true);

            OrchestratorCollection.ResetOrchestratorForActiveDevice();

            BuildScheduleList();

            UpdateCurrentEffectLabelsAndLayout();
        }

        public void UpdatedSchedule(Schedule originalSchedule, Schedule newSchedule)
        {
            UserSettings.Settings.DeleteSchedule(originalSchedule);
            UserSettings.Settings.AddSchedule(newSchedule, false);

            OrchestratorCollection.ResetOrchestratorForActiveDevice();

            BuildScheduleList();

            UpdateCurrentEffectLabelsAndLayout();
        }

        private void BuildScheduleList()
        {
            ScheduleList.Children.Clear();

            foreach (var schedule in UserSettings.Settings.ActiveDevice.Schedules)
            {
                ScheduleList.Children.Add(new ScheduleItemUserControl(this, schedule));
            }
        }

        public void EditSchedule(Schedule schedule)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Edit, schedule);
            scheduleWindow.ShowDialog();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            UserSettings.Settings.DeleteSchedule(schedule);

            OrchestratorCollection.ResetOrchestratorForActiveDevice();

            BuildScheduleList();

            UpdateCurrentEffectLabelsAndLayout();
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void ActivateSchedule(Schedule schedule)
        {
            UserSettings.Settings.ActivateSchedule(schedule);

            OrchestratorCollection.ResetOrchestratorForActiveDevice();

            BuildScheduleList();

            UpdateCurrentEffectLabelsAndLayout();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.ShowDialog();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(string.Format(MainWindows.Resources.AreYouSure, _selectedDevice), MainWindows.Resources.DeleteConfirmation, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                App.ResetAllSettings(this);
            }
        }

        private class TaskbarDoubleClickCommand : ICommand
        {
            private readonly MainWindow _window;

            public TaskbarDoubleClickCommand(MainWindow window)
            {
                _window = window;
            }

            public void Execute(object parameter)
            {
                _window.Show();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged; //Must be included for the interface
        }

        private async void Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var device = UserSettings.Settings.ActiveDevice;
                var nanoleafClient = NanoleafClient.GetClientForDevice(device);
                var effects = await nanoleafClient.EffectsEndpoint.GetEffectsListAsync();

                device.LoadEffectsFromNameList(effects);

                UserSettings.Settings.SaveSettings();

                PopupCreator.Success(MainWindows.Resources.ReloadSuccessful);
            }
            catch (Exception exception)
            {
                PopupCreator.Error(MainWindows.Resources.ReloadFailed);
                LogManager.GetCurrentClassLogger().Error(exception, "Failed to reload effects list");
            }
        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            var setupWindow = new SetupWindow(this);
            setupWindow.Show();
        }
        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void RemoveDevice_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(string.Format(MainWindows.Resources.DeleteDeviceAreYouSure, _selectedDevice), MainWindows.Resources.DeleteConfirmation, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                UserSettings.Settings.DeleteActiveDevice();

                if (UserSettings.Settings.Devices.Count > 0)
                {
                    DeviceNames.Remove(_selectedDevice);
                    SelectedDevice = DeviceNames.FirstOrDefault();

                    DevicesDropdown.SelectedItem = SelectedDevice;

                    UpdateCurrentEffectLabelsAndLayout();
                }
                else
                {
                    var setupWindow = new SetupWindow();
                    setupWindow.Show();

                    Close();
                }
            }
        }

        public void UpdateCurrentEffectLabelsAndLayout()
        {
            CurrentEffectUserControl.UpdateLabels();
            LayoutDisplay.UpdateColors();
        }

        private void PercentageProfile_Click(object sender, RoutedEventArgs e)
        {
            var percentageProfileWindow = new PercentageProfileWindow();
            percentageProfileWindow.Show();
        }
    }
}
