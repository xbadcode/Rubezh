﻿using System;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class ElementSubPlanViewModel : BaseViewModel
    {
        public ElementSubPlanViewModel()
        {
            ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public Guid PlanUID { get; private set; }
        public string PresentationName { get; private set; }
        ElementSubPlanView _elementSubPlanView;

        public void Initialize(ElementSubPlan elementSubPlan, Canvas canvas)
        {
            PlanUID = elementSubPlan.UID;
            PresentationName = elementSubPlan.Caption;

            _elementSubPlanView = new ElementSubPlanView()
            {
                DataContext = this
            };
            _elementSubPlanView._polygon.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(subPlanPolygon_PreviewMouseLeftButtonDown);

            foreach (var polygonPoint in elementSubPlan.PolygonPoints)
            {
                _elementSubPlanView._polygon.Points.Add(new System.Windows.Point() { X = polygonPoint.X, Y = polygonPoint.Y });
            }

            canvas.Children.Add(_elementSubPlanView);
        }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                OnPropertyChanged("StateType");
            }
        }

        void subPlanPolygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ServiceFactory.Events.GetEvent<SelectPlanEvent>().Publish(PlanUID);
            }
        }

        public RelayCommand ShowPropertiesCommand { get; private set; }
        void OnShowProperties()
        {
        }

        public void Update(StateType stateType)
        {
            StateType = stateType;
        }
    }
}