using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SerializationFail.ViewModels
{
	/// <summary>
	/// Provides a INotifyPropertyChanged implementation that uses a dispatcher.
	/// </summary>
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		/// <summary>
		/// Checks if the value has changed and uses the call stack to determine
		/// which property this was called from, then calls OnPropertyChanged
		/// with the property name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="current"></param>
		/// <param name="newValue"></param>
		protected bool SetProperty<T>(ref T current, T newValue, [CallerMemberName] string propertyName = "")
		{
			if (!EqualityComparer<T>.Default.Equals(current, newValue))
			{
				T oldValue = current;
				current = newValue;
				RaisePropertyChanged(propertyName);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected bool SetProperty<T>(ref T current, T newValue, params string[] propertyNames)
		{
			if (!EqualityComparer<T>.Default.Equals(current, newValue))
			{
				current = newValue;
				RaisePropertyChanged(propertyNames);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected bool SetProperty([CallerMemberName] string propertyName = "")
		{
			RaisePropertyChanged(propertyName);
			return true;
		}

		protected bool SetProperty(Action action, [CallerMemberName] string propertyName = "")
		{
			action();
			RaisePropertyChanged(propertyName);
			return true;
		}

		protected bool SetProperty(Func<bool> action, [CallerMemberName] string propertyName = "")
		{
			if (action())
			{
				RaisePropertyChanged(propertyName);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Triggers PropertyChanged event in a way that it can be handled
		/// by controls on a different thread.
		/// </summary>
		/// <param name="propertyName"></param>
		protected virtual void RaisePropertyChanged(params string[] propertyNames)
		{
			foreach (string propertyName in propertyNames)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
			if (propertyNames.Length == 0)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
