﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Controls
{
    /// <summary>
    /// Creates a CollectionView for databinding to a HierarchicalTemplate ItemSource
    /// </summary>
    [ValueConversion(typeof(System.Collections.IList), typeof(System.Collections.IEnumerable))]
    public class CollectionViewFactoryConverter : IValueConverter
    {
        /// <summary>
        /// returns sorted CollectionView
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Collections.IList collection = value as System.Collections.IList;
            ListCollectionView view = new ListCollectionView(collection);
            var parameters = (parameter as string).Split('.');
            foreach (var param in parameters)
            {
                SortDescription sort = new SortDescription(param.ToString(), ListSortDirection.Ascending);
                view.SortDescriptions.Add(sort);
            }

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}