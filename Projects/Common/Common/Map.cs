﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
	public class Map<T> : IMap
		where T : IIdentity
	{
		List<T> _map;

		public Func<IEnumerable<T>> Builder { get; private set; }

		public Map(Func<IEnumerable<T>> builder)
		{
			Builder = builder;
		}

		public T GetItem(Guid guid)
		{
			return _map != null ? _map.FirstOrDefault(x => x.UID == guid) : default(T);
		}

		public void BuildSafe(IEnumerable<T> items = null)
		{
			if (items == null && Builder != null)
				items = Builder();
			_map = items == null ? new List<T>() : items.ToList();
		}
		void IMap.BuildSafe()
		{
			BuildSafe();
		}

	}
	interface IMap
	{
		void BuildSafe();
	}
}