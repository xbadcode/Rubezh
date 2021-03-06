﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using OpcClientSdk.Da;

namespace RubezhAPI.Automation
{
	[DataContract]
	[KnownType(typeof(OpcDaTag))]
	[KnownType(typeof(OpcDaGroup))]
	public abstract class OpcDaElement
	{
		#region Propeties

		[DataMember]
		public string Path { get; set; }
		[DataMember]
		public string ElementName { get; set; }
		public abstract bool IsTag { get; } // protected set; }
		public bool IsRoot
		{
			get
			{
				if (IsTag)
				{
					return false;
				}
				else
				{
					var segments = OpcDaElement.GetGroupNamesAndCheckFormatFromPath(this);
					return ((segments.Length == 1) && (segments[0] == ROOT)) ? true : false;
				}
			}
		}
		
		#endregion

		#region Constants

		public const string ROOT = ".";
		public const string SPLITTER = @"\";

		#endregion

		#region Methods

		public static string[] GetGroupNamesAndCheckFormatFromPath(OpcDaElement element)
		{
			string[] segments;
			const int MinSegmentsAmountForTag = 2; // Минимальное количество сегментов в пути для тега 2: .\Имя тега
			const int MinSegmentsAmountForGroup = 1; // Минимальное кол-во сегментов в пупи для группы

			segments = element.Path.Split(new string[] { SPLITTER }, System.StringSplitOptions.None);

			if (element.IsTag)
			{
				if (segments.Length < MinSegmentsAmountForTag)
				{
					throw new Exception(string.Format("Путь тега слишком короткий: {0}", element.Path));
				}
				if (segments[0] != ROOT)
				{
					throw new Exception(string.Format("Некорректный коренвой элемент пути: {0}", element.Path));
				}
			}
			else
			{
				if (segments.Length < MinSegmentsAmountForGroup)
				{
					throw new Exception(string.Format("Путь группы слишком короткий: {0}", element.Path));
				}
				if (segments[0] != ROOT)
				{
					throw new Exception(string.Format("Некорректный коренвой элемент пути: {0}", element.Path));
				}
			}
			return segments;
		}

		/// <summary>
		/// Создаёт на основе указанного объетка, OpcDaTag или OpcDaGroup
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static OpcDaElement Create(TsCDaBrowseElement element)
		{
			if (element.IsItem)
			{
				return new OpcDaTag(element);
			}
			else
			{
				return new OpcDaGroup { Path = element.ItemPath, ElementName = element.ItemName };
			}
		}

		protected static TsCDaItemProperty GetProperty(TsCDaBrowseElement element, TsDaPropertyID propertyId)
		{
			return element.Properties.FirstOrDefault(x => x.ID == propertyId);
		}

		#endregion
	}
}