﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Common;
using DeviceControls;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrustructure.Plans.Painters;
using Infrastructure.Common.Windows;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : TreeNodeViewModel<CameraViewModel>
	{
		public Camera Camera { get; set; }
		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new CameraDetailsViewModel(Camera));
		}
		public CameraViewModel(Camera camera)
		{
			VisualCameraViewModels = new List<CameraViewModel>();
			Camera = camera;
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		public List<CameraViewModel> VisualCameraViewModels;

		public string PresentationName
		{
			get
			{
				return Camera.Name + " " + Camera.Ip;
			}
		}

		public string PresentationAddress
		{
			get
			{
				return Camera.Ip;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Camera);
			OnPropertyChanged(() => PresentationAddress);
			OnPropertyChanged(() => IsOnPlan);
		}

		public bool IsOnPlan
		{
			get { return Camera.PlanElementUIDs.Count > 0; }
		}

		public void StartAll()
		{
			foreach (var visualCameraViewModel in VisualCameraViewModels)
			{
				visualCameraViewModel.Start(false);
			}
		}

		public void StopAll()
		{
			foreach (var visualCameraViewModel in VisualCameraViewModels)
			{
				visualCameraViewModel.Stop(false);
			}
		}

		public void Start(bool addToRootCamera = true)
		{
			//_cellPlayerWrap.Start(Camera, Camera.ChannelNumber);
			if ((addToRootCamera)&&(RootCamera != null))
				RootCamera.VisualCameraViewModels.Add(this);
		}

		public void Stop(bool addToRootCamera = true)
		{
			//_cellPlayerWrap.Stop();
			if ((addToRootCamera) && (RootCamera != null))
				RootCamera.VisualCameraViewModels.Remove(this);
		}

		CameraViewModel RootCamera
		{
			get
			{
				return CamerasViewModel.Current.Cameras.FirstOrDefault(x => x.Camera.Ip == Camera.Ip);
			}
		}
	}
}