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
        // PROPERTIES ======================================================================================

        private Dictionary<string, string> _textField;
        private  readonly string _textFieldDesc =
            "Use this field type to name a room, give a room a unique ID, etc.";

        private Dictionary<string, string> _numericalField;
        private readonly string _numericalFieldDesc =
            "Use this field type to a non-tracked numerical value like room number, cost, etc.";


        // PROPERTY CHANGE NOTIFICATION IMPLEMENTATION =======================================================
        public event PropertyChangedEventHandler PropertyChanged;


        //GETTERS SETTERS W/ PROPERTY CHANGE =================================================================

        // Text descriptors-----------------------------------------------------
        public Dictionary<string, string> TextField
        {
            get => _textField;
            set
            {
                _textField = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, string> NumericalField
        {
            get => _numericalField;
            set
            {
                _numericalField = value;
                OnPropertyChanged();
            }
        }


        //Property change method to raise the event ===========================================================
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
