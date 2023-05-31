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

        private FieldDict<string, float> _numericalField = new FieldDict<string, float>();
        private readonly string _numericalFieldDesc =
            "Use this field type to a non-tracked numerical value like room number, cost, etc.";

        //CONSTRUCTOR ========================================================================================
        public TableObject()
        {
            SubscribeToPropertyChanged(nameof(TextField));
            SubscribeToPropertyChanged(nameof(NumericalField));
        }


        //GETTERS SETTERS W/ PROPERTY CHANGE =================================================================

        // Text descriptors-----------------------------------------------------
        public FieldDict<string, string> TextField
        {
            get => _textField;
            set => SetPropertyValue(ref _textField, value, nameof(TextField));
        }

        public FieldDict<string, float> NumericalField
        {
            get => _numericalField;
            set => SetPropertyValue(ref _numericalField, value, nameof(_numericalField));
        }


        //WILD EVENT SUBSCRIPTION THAT NEEDS TO BE DONE FOR EVERY DICTIONARY! =================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            Rhino.RhinoApp.WriteLine("THE PROPERTY CHANGED!!!");
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

        //Splitting out the properties and property names ======================================================
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
