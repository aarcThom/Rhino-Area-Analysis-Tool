using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AreaAnalysis.Interfaces;
using Rhino;

namespace AreaAnalysis.Classes
{
    public class CellValue<T> : ICellValue
    {
        private T _value;
        private readonly string _floatFormat;

        public CellValue(object value, int floatPoint = 2)
        {
            SetCellValue(value);
            _floatFormat = $"F{floatPoint}";
        }

        public string ValueToString()
        {
            if (typeof(T) == typeof(float)) return ((float)(object)_value).ToString(_floatFormat);
            return _value?.ToString();
        }

        public void SetCellValue(object value)
        {
            if (value is T typedValue)
            {
                _value = typedValue;
            }
            else
            {
                throw new ArgumentException($"Invalid value type. Expected type: {typeof(T)}");
            }
        }
    }
}
