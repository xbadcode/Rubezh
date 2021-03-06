﻿using AutomationModule.ViewModels;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.ElementProperties.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AutomationModule.Plans.ViewModels
{
	public class ProcedurePropertiesViewModel : TextBlockPropertiesViewModel
	{
		private ElementProcedure _element;
		ElementBaseRectangle ElementBaseRectangle { get; set; }

		public ProcedurePropertiesViewModel(ElementProcedure element, ProceduresViewModel proceduresViewModel, CommonDesignerCanvas designerCanvas)
			: base(element, designerCanvas)
		{
			Procedures = proceduresViewModel.Procedures;
			_element = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			Title = "Свойства фигуры: Процедура";
			if (element.ProcedureUID != Guid.Empty)
				SelectedProcedure = Procedures.FirstOrDefault(x => x.Procedure.Uid == element.ProcedureUID);
		}

		public ObservableCollection<ProcedureViewModel> Procedures { get; private set; }

		private ProcedureViewModel _selectedProcedure;
		public ProcedureViewModel SelectedProcedure
		{
			get { return _selectedProcedure; }
			set
			{
				_selectedProcedure = value;
				OnPropertyChanged(() => SelectedProcedure);
			}
		}

		protected override bool Save()
		{
			AutomationPlanExtension.Instance.RewriteItem(_element, SelectedProcedure.Procedure);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedProcedure != null;
		}
	}
}