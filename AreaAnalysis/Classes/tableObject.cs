using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rhino;

namespace AreaAnalysis.Classes
{
    public class TableObject : INotifyPropertyChanged
    {
        // FIELDS ======================================================================================

        private FieldDict<string, string> _textField = new FieldDict<string, string>();
        private  readonly string _textFieldDesc =
            "Use this field type to name a room, give a room a unique ID, etc.";
        public readonly string TextFieldDefault = "undefined";
        //static field to propagate to all classes
        private static List<string> _textFieldKeys = new List<string>();

        private FieldDict<string, int> _integerField = new FieldDict<string, int>();
        private readonly string _integerFieldDesc =
            "Use this field type to a non-tracked numerical integer values like room numbers";
        public readonly int IntegerFieldDefault = 0;
        //static field to propagate to all classes
        private static List<string> _integerFieldKeys = new List<string>();


        private FieldDict<string, float> _numberField = new FieldDict<string, float>();
        private readonly string _numberFieldDesc =
            "Use this field type to a non-tracked numerical floating point values like cost, area targets";
        public readonly float NumberFieldDefault = 0f;
        //static field to propagate to all classes
        private static List<string> _numberFieldKeys = new List<string>();


        // CONSTRUCTOR ========================================================================================
        public TableObject(Type type = null, string userKey = "")
        {

            // add existing keys to new instance
            foreach (var key in _textFieldKeys)
            {
                _textField.Add(key, TextFieldDefault);
            }

            foreach (var key in _integerFieldKeys)
            {
                _integerField.Add(key, IntegerFieldDefault);
            }

            foreach (var key in _numberFieldKeys)
            {
                _numberField.Add(key, NumberFieldDefault);
            }

            // adding new keys to new instance
            if (type != null && userKey != "")
            {
                if (type == typeof(string))
                {
                    _textFieldKeys.Add(userKey); // add to all future instances
                    _textField.Add(userKey, TextFieldDefault);
                } 
                else if (type == typeof(int))
                {
                    _integerFieldKeys.Add(userKey); // add to all future instances
                    _integerField.Add(userKey, IntegerFieldDefault);
                }
                else // if (type = typeof(float))
                {
                    _numberFieldKeys.Add(userKey); // add to all future instances
                    _numberField.Add(userKey, NumberFieldDefault);
                }

                //NOTE: REMEMBER TO ADD NEW FIELDS TO AddNewField() AS WELL!!!
            }

            // subscribe to change events
            SubscribeToPropertyChanged(nameof(TextField));
            SubscribeToPropertyChanged(nameof(IntegerField));
            SubscribeToPropertyChanged(nameof(NumberField));
        }


        // PROPERTIES =================================================================

        public FieldDict<string, string> TextField
        {
            get => _textField;
            set => SetPropertyValue(ref _textField, value, nameof(TextField));
        }

        public FieldDict<string, int> IntegerField
        {
            get => _integerField;
            set => SetPropertyValue(ref _integerField, value, nameof(_integerField));
        }

        public FieldDict<string, float> NumberField
        {
            get => _numberField;
            set => SetPropertyValue(ref _numberField, value, nameof(_numberField));
        }


        // WILD EVENT SUBSCRIPTION THAT NEEDS TO BE DONE FOR EVERY DICTIONARY! =================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SubscribeToPropertyChanged(string propertyName)
        {
            var propertyInfo = GetType().GetProperty(propertyName);
            var observableObject = propertyInfo.GetValue(this) as INotifyPropertyChanged;
            observableObject.PropertyChanged += PropertyChangedEventHandler;
        }

        private void UnsubscribeFromPropertyChanged(string propertyName)
        {
            var propertyInfo = GetType().GetProperty(propertyName);
            var observableObject = propertyInfo.GetValue(this) as INotifyPropertyChanged;
            observableObject.PropertyChanged -= PropertyChangedEventHandler;
        }

        private void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void SetPropertyValue<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            UnsubscribeFromPropertyChanged(propertyName);

            field = value;

            SubscribeToPropertyChanged(propertyName);

            OnPropertyChanged(propertyName);
        }

        // PUBLIC METHODS ======================================================
        public List<string> GetFieldsNames()
        {
            List<string> fieldNames = new List<string>();

            foreach (var prop in GetProperties())
            {
                fieldNames.Add(FormatFieldNames(prop));
            }
            return fieldNames;
        }

        public List<string> GetFieldsDescriptions()
        {
            List<string> fieldDescs = new List<string>();

            foreach (var prop in GetProperties())
            {
                string propName = FormatFieldDescs(prop);
                fieldDescs.Add(GetPropValue(propName));
            }
            return fieldDescs;
        }

        public void AddNewField(Type type, string userKey)
        {
            if (type == typeof(string))
            {
                _textField.Add(userKey, TextFieldDefault);

                if (!_textFieldKeys.Contains(userKey))
                {
                    _textFieldKeys.Add(userKey);
                }
            }
            else if (type == typeof(int))
            {
                _integerField.Add(userKey, IntegerFieldDefault);

                if (!_integerFieldKeys.Contains(userKey))
                {
                    _integerFieldKeys.Add(userKey);
                }
            }
            else // if (type = typeof(float))
            {
                _numberField.Add(userKey, NumberFieldDefault);

                if (!_numberFieldKeys.Contains(userKey))
                {
                    _numberFieldKeys.Add(userKey);
                }
            }
        }

        public List<string> GetKeys()
        {
            return _textFieldKeys;
        }

        // PRIVATE METHODS ======================================================

        private List<string> GetProperties()
        {
            Type tableType = typeof(TableObject);
            PropertyInfo[] tableProps = tableType.GetProperties();

            List<string> propNames = new List<string>();

            foreach (var name in tableProps)
            {
                propNames.Add(name.Name);
            }

            return propNames;
        }

        public static string FormatFieldNames(string prop)
        {
            string pattern = @"(?<!^)(?=[A-Z])";
            string[] words = Regex.Split(prop, pattern);
            return string.Join(" ", words);
        }

        private string FormatFieldDescs(string prop)
        {
            return "_" + char.ToLower(prop[0]) + prop.Substring(1) + "Desc";
        }

        private string GetPropValue(string propName)
        {
            Type tableType = typeof(TableObject);
            FieldInfo fieldInfo = tableType.GetField(propName, 
                BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo.GetValue(this).ToString();
        }

    }
}
