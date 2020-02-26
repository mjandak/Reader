using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Reader.DAL
{
	public abstract class ModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropChanged(string name)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
	}
}
