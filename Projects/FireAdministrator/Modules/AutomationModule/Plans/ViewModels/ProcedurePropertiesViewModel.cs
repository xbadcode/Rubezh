﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutomationModule.ViewModels;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace AutomationModule.Plans.ViewModels
{
	public class ProcedurePropertiesViewModel : TextBlockPropertiesViewModel
	{
		private ElementProcedure _element;

		public ProcedurePropertiesViewModel(ElementProcedure element, ProceduresViewModel proceduresViewModel)
			: base(element)
		{
			Procedures = proceduresViewModel.Procedures;
			_element = element;
		    Title = Resources.Language.Plans.ViewModels.ProcedurePropertiesViewModel.Title;
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
			_element.ProcedureUID = SelectedProcedure == null ? Guid.Empty : SelectedProcedure.Procedure.Uid;
			AutomationPlanExtension.Instance.SetItem<Procedure>(_element, SelectedProcedure == null ? null : SelectedProcedure.Procedure);
			UpdateProcedures(_element.ProcedureUID);
			return base.Save();
		}
		private void UpdateProcedures(Guid procedureUID)
		{
			if (Procedures != null)
			{
				if (procedureUID != _element.ProcedureUID)
					Update(procedureUID);
				Update(_element.ProcedureUID);
			}
		}
		private void Update(Guid procedureUID)
		{
			var procedure = Procedures.FirstOrDefault(x => x.Procedure.Uid == procedureUID);
			if (procedure != null)
				procedure.Update();
		}
	}
}