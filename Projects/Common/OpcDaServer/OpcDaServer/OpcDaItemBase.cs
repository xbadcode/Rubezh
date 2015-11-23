﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpcDaServer
{
	/// <summary>
	/// Тег или директория OPC DA сервера
	/// </summary>
	public abstract class OpcDaItemBase
	{
		public OpcDaItemBase()
		{
			_items = new List<OpcDaItemBase>();
		}

		#region Methods
		
		#endregion

		#region Fields And Properties

		protected List<OpcDaItemBase> _items;

		/// <summary>
		/// Наименование корневой директории тегов
		/// </summary>
		public const string RootDirectory = @"."; 

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsDirectory { get; }

		protected OpcDaDirectory _directory;
		/// <summary>
		/// Директория содержащая данный тег или директорию
		/// </summary>
		public OpcDaDirectory Directory { get { return _directory; } }

		#endregion
	}
}
