﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlanCanvasViewModel SelectedPlanCanvasViewModel { get; set; }
        public static PlansViewModel Current;

        public PlansViewModel()
        {
            Current = this;
            MainCanvas = new Canvas();
            ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);

            DrawAllPlans();

            Plans = new ObservableCollection<PlanViewModel>();
            BuildTree();
            if (Plans.IsNotNullOrEmpty())
                SelectedPlan = Plans[0];
        }

        List<PlanCanvasViewModel> PlanCanvasViewModels;

        void DrawAllPlans()
        {
            PlanCanvasViewModels = new List<PlanCanvasViewModel>();

            foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
            {
                var planCanvasViewModel = new PlanCanvasViewModel(plan);
                PlanCanvasViewModels.Add(planCanvasViewModel);
            }
        }

        void BuildTree()
        {
            if (FiresecManager.PlansConfiguration.Plans.IsNotNullOrEmpty())
            {
                foreach (var plan in FiresecManager.PlansConfiguration.Plans)
                {
                    var planTreeItemViewModel = new PlanViewModel(plan, Plans)
                    {
                        Parent = null,
                        IsExpanded = true
                    };
                    Plans.Add(planTreeItemViewModel);
                    AddPlan(plan, planTreeItemViewModel);
                }
            }
        }

        void AddPlan(Plan parentPlan, PlanViewModel parentPlanTreeItem)
        {
            if (parentPlan.Children != null)
                foreach (var plan in parentPlan.Children)
                {
                    var planTreeItemViewModel = new PlanViewModel(plan, Plans)
                    {
                        Parent = parentPlanTreeItem,
                        IsExpanded = true
                    };
                    parentPlanTreeItem.Children.Add(planTreeItemViewModel);
                    Plans.Add(planTreeItemViewModel);
                    AddPlan(plan, planTreeItemViewModel);
                }
        }

        Canvas _mainCanvas;
        public Canvas MainCanvas
        {
            get { return _mainCanvas; }
            set
            {
                _mainCanvas = value;
                OnPropertyChanged("MainCanvas");
            }
        }

        public ObservableCollection<PlanViewModel> Plans { get; set; }

        PlanViewModel _selectedPlan;
        public PlanViewModel SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                _selectedPlan = value;
                SelectedPlanCanvasViewModel = PlanCanvasViewModels.FirstOrDefault(x => x.Plan.UID == value.Plan.UID);
                SelectedPlanCanvasViewModel.Update();
                MainCanvas = SelectedPlanCanvasViewModel.Canvas;
                OnPropertyChanged("SelectedPlan");
            }
        }

        public void OnSelectPlan(Guid PlanUID)
        {
            SelectedPlan = Plans.FirstOrDefault(x => x.Plan.UID == PlanUID);
        }

        public void ShowDevice(Guid deviceUID)
        {
            foreach (var planViewModel in Plans)
            {
                if (planViewModel.DeviceStates.Any(x => x.UID == deviceUID))
                {
                    SelectedPlan = planViewModel;
                    SelectedPlanCanvasViewModel.SelectDevice(deviceUID);
                    return;
                }
            }
        }

        public void ShowZone(ulong zoneNo)
        {
            foreach (var planViewModel in Plans)
            {
                foreach (var zone in planViewModel.Plan.ElementPolygonZones.Where(x => x.ZoneNo.HasValue))
                {
                    if (zone.ZoneNo.Value == zoneNo)
                    {
                        SelectedPlan = planViewModel;
                        SelectedPlanCanvasViewModel.SelectZone(zoneNo);
                        return;
                    }
                }
                foreach (var zone in planViewModel.Plan.ElementRectangleZones.Where(x => x.ZoneNo.HasValue))
                {
                    if (zone.ZoneNo.Value == zoneNo)
                    {
                        SelectedPlan = planViewModel;
                        SelectedPlanCanvasViewModel.SelectZone(zoneNo);
                        return;
                    }
                }
            }
        }

        public bool CanZoom
        {
            get { return FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_ShowPlans); }
        }
    }
}