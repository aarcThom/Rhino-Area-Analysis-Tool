using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;

namespace AreaAnalysis.Classes
{
    public class FieldDict<TKey, TValue> : Dictionary<TKey, TValue>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                OnPropertyChanged($"[{key}]");
            }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnPropertyChanged(key.ToString());
        }

        public new void Remove(TKey key)
        {
            base.Remove(key);
            OnPropertyChanged(key.ToString());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
