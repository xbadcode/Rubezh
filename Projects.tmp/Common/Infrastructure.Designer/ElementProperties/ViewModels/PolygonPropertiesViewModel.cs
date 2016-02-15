﻿using System.Windows.Media;
using RubezhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace Infrastructure.Designer.ElementProperties.ViewModels
{
	public class PolygonPropertiesViewModel : SaveCancelDialogViewModel
	{
		ElementPolygon _elementPolygon;
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public PolygonPropertiesViewModel(ElementPolygon elementPolygon)
		{
			Title = "Свойства фигуры: Полигон";
			_elementPolygon = elementPolygon;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(_elementPolygon);
			CopyProperties();
		}

		void CopyProperties()
		{
			ElementBase.Copy(this._elementPolygon, this);
			StrokeThickness = _elementPolygon.BorderThickness;
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		string _presentationName;
		public string PresentationName
		{
			get { return _presentationName; }
			set
			{
				_presentationName = value;
				OnPropertyChanged(() => PresentationName);
			}
		}

		double _strokeThickness;
		public double StrokeThickness
		{
			get { return _strokeThickness; }
			set
			{
				_strokeThickness = value;
				OnPropertyChanged(() => StrokeThickness);
			}
		}

		bool _showTooltip;
		public bool ShowTooltip
		{
			get { return this._showTooltip; }
			set
			{
				this._showTooltip = value;
				OnPropertyChanged(() => this.ShowTooltip);
			}
		}

		protected override bool Save()
		{
			ElementBase.Copy(this, this._elementPolygon);
			_elementPolygon.BorderThickness = StrokeThickness;
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}