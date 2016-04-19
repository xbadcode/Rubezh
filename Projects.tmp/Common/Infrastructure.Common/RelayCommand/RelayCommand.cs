﻿using System;
using System.Windows.Input;
using Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services;

namespace Infrastructure.Common
{
	public delegate bool PredicateDelegate();

	public class RelayCommand : ICommand
	{
		#region Fields
		readonly Action _execute;
		readonly Predicate<object> _canExecute;
		#endregion

		#region Ctors
		public RelayCommand(Action execute)
			: this(execute, (Predicate<object>)null)
		{
		}

		public RelayCommand(Action execute, Predicate<object> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action execute, PredicateDelegate canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = (object obj) => { return canExecute(); };
		}
		#endregion

		public void Execute()
		{
			if (CanExecute(null))
				ForceExecute();
		}
		public void ForceExecute()
		{
			try
			{
				_execute();
			}
			catch (Exception e)
			{
				Logger.Error(e, "RelayCommand.ForceExecute");
				ServiceFactoryBase.MessageBoxService.ShowException(e, "При выполнении операции возникло исключение");
			}
		}

		#region ICommand Members
		void ICommand.Execute(object parameter)
		{
			ForceExecute();
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
			{
				try
				{
					return _canExecute(parameter);
				}
				catch (Exception e)
				{
					Logger.Error(e, "RelayCommand.CanExecute");
					return false;
				}
			}
			return true;
		}

		public event EventHandler CanExecuteChanged;
		#endregion
	}

	public class RelayCommand<T> : ICommand
	{
		#region Fields

		readonly Action<T> _execute;
		readonly Predicate<T> _canExecute;

		#endregion

		#region Ctors

		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		#endregion

		public void Execute(T parameter)
		{
			if (CanExecute(parameter))
				ForceExecute(parameter);
		}
		public void ForceExecute(T parameter)
		{
			try
			{
				_execute(parameter);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RelayCommand.ForceExecute");
				ServiceFactoryBase.MessageBoxService.ShowException(e, "При выполнении операции возникло исключение");
			}
		}

		#region ICommand Members
		void ICommand.Execute(object parameter)
		{
			ForceExecute((T)parameter);
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute != null)
				return _canExecute((T)(parameter ?? default(T)));
			return true;
		}

		event EventHandler ICommand.CanExecuteChanged
		{
			add { }
			remove { }
		}
		#endregion
	}
}