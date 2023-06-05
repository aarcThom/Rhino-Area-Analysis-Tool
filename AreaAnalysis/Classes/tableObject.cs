﻿using System;
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
        // GUID LINKAGE STUFF ==========================================================================
        private Guid _rhinoGuid;
        private readonly string _linkColumnName = "Rhino Link";
        private readonly string _linkColDescription = "Use this column to link a Rhino object to your data table.";
        private readonly string _unlinked = "❌";
        private readonly string _linked = "✔️";


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




        //a list containing all the key lists - UPDATE THIS TOO!
        private List<List<string>> _allKeys = new List<List<string>>()
            { _textFieldKeys, _integerFieldKeys, _numberFieldKeys};

        // CONSTRUCTOR ========================================================================================
        public TableObject(Type type = null, string userKey = "")
        {
            //setting link status
            LinkStatus = _unlinked;


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

        // the rhino link property

        public string LinkStatus { get; set; }

        // dictionaries for new multiple columns
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

            fieldNames.Add(_linkColumnName); // need to add link name to end

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

            fieldDescs.Add(_linkColDescription); // need to add link description to end

            return fieldDescs;
        }

        public (List<string>, string) GetKeys()
        {
            List<string> allKeys = new List<string>();

            foreach (var keyList in _allKeys)
            {
                foreach (var key in keyList)
                {
                    allKeys.Add(key);
                }
            }

            return (allKeys, _linkColumnName); // need to return link column name as well
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

        public void ChangeField(Type type, string oldKey, string newKey)
        {
            if (type == typeof(string))
            {
                string value = _textField[oldKey];
                _textField.Remove(oldKey);
                _textField.Add(newKey, value);

                if (_textFieldKeys.Contains(oldKey))
                {
                    int index = _textFieldKeys.IndexOf(oldKey);
                    _textFieldKeys[index] = newKey;
                }
            }
            else if (type == typeof(int))
            {
                int value = _integerField[oldKey];
                _integerField.Remove(oldKey);
                _integerField.Add(newKey, value);

                if (_integerFieldKeys.Contains(oldKey))
                {
                    int index = _integerFieldKeys.IndexOf(oldKey);
                    _integerFieldKeys[index] = newKey;
                }
            }
            else // if (type = typeof(float))
            {
                float value = _numberField[oldKey];
                _numberField.Remove(oldKey);
                _numberField.Add(newKey, value);

                if (_numberFieldKeys.Contains(oldKey))
                {
                    int index = _numberFieldKeys.IndexOf(oldKey);
                    _numberFieldKeys[index] = newKey;
                }
            }
        }

        public void DeleteField(Type type, string key)
        {
            if (type == typeof(string))
            {
                _textField.Remove(key);

                if (_textFieldKeys.Contains(key))
                {
                    _textFieldKeys.Remove(key);
                }

            }
            else if (type == typeof(int))
            {
               _integerField.Remove(key);

               if (_integerFieldKeys.Contains(key))
               {
                   _integerFieldKeys.Remove(key);
               }
            }
            else // if (type = typeof(float))
            {
               _numberField.Remove(key);

               if (_numberFieldKeys.Contains(key))
               {
                   _numberFieldKeys.Remove(key);
               }
            }
        }

        public string GetLinkName()
        {
            return _linkColumnName;
        }


        // PRIVATE METHODS ======================================================

            private List<string> GetProperties()
        {
            Type tableType = typeof(TableObject);
            PropertyInfo[] tableProps = tableType.GetProperties();

            List<string> propNames = new List<string>();

            foreach (var name in tableProps)
            {
                if (name.Name != "LinkStatus")
                {
                    propNames.Add(name.Name);
                }
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
