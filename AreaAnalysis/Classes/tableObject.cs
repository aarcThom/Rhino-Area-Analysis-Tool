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
using Rhino.Render.Fields;
using static Rhino.Render.Dithering;

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

        private readonly string _textFieldDesc =
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
            { _textFieldKeys, _integerFieldKeys, _numberFieldKeys };

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

                //NOTE: REMEMBER TO ADD NEW FIELDS TO AddNewColumnToField() AS WELL!!!
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
            set => SetPropertyValue(ref _integerField, value, nameof(IntegerField));
        }

        public FieldDict<string, float> NumberField
        {
            get => _numberField;
            set => SetPropertyValue(ref _numberField, value, nameof(NumberField));
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

        //gets the column types (properties of table object)
        public List<string> GetPropertiesNames()
        {
            List<string> fieldNames = new List<string>();

            foreach (var prop in GetProperties())
            {
                fieldNames.Add(FormatPropertyNamesForUI(prop));
            }

            fieldNames.Add(_linkColumnName); // need to add link name to end

            return fieldNames;
        }

        // returns column type classifications - mainly used for column add modal.
        public List<string> GetFieldsDescriptions()
        {
            List<string> fieldDescs = new List<string>();

            foreach (var prop in GetProperties())
            {
                string propName = FormatFieldDescs(prop);
                fieldDescs.Add(GetFieldValueToString(propName));
            }

            fieldDescs.Add(_linkColDescription); // need to add link description to end

            return fieldDescs;
        }

        //returns Rhino link column name so I don't have to manually update it everywhere
        public string GetLinkName()
        {
            return _linkColumnName;
        }

        //gets all keys of all fields (aka column header names in gridview)
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

        // adds new columns (aka key:value pairings) into a given field (aka column type)
        public void AddNewColumnToField(Type type, string userKey)
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

        //changes the name of a column (aka a key/value pair) in a given field (aka column type)
        public void ChangeColumnName(string oldKey, string newKey)
        {
            Type type = GetKeyType(oldKey);
            string value = _textField[oldKey];

            object[] oldKeyArgs = new object[] { oldKey };
            object[] newKeyArgs = new object[] { newKey, value };
            Type[] overrides = new[] { typeof(string) };

            Generic(type, "Remove", "dict", oldKeyArgs);
            Generic(type, "Add", "dict", newKeyArgs);

            if ((bool)Generic(type, "Contains", "list", oldKeyArgs))
            {
                int index = (int)Generic(type, "IndexOf", "list", oldKeyArgs, overrides);
                GenericListReplace(type, index, newKey);
            }
        }
        
        
        //deletes a column (aka key/value pair) from a given column (aka field in tObject)
        public void DeleteColumn(string key)
        {
            Type type = GetKeyType(key);
            var args = new object[]{ key };

            Generic(type, "Remove", "dict", args);
            Generic(type, "Remove", "list", args);

        }



        // PRIVATE METHODS ======================================================

        //gets all properties of table object - used to get field names and descriptions (column types and descriptions)
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

        //takes property names in table object and formats them as human friendly names for the UI
        public static string FormatPropertyNamesForUI(string prop)
        {
            string pattern = @"(?<!^)(?=[A-Z])";
            string[] words = Regex.Split(prop, pattern);
            return string.Join(" ", words);
        }

        // takes property names and formats them into related description field name
        private string FormatFieldDescs(string prop)
        {
            return "_" + char.ToLower(prop[0]) + prop.Substring(1) + "Desc";
        }

        //retrieves the string value from a given field
        private string GetFieldValueToString(string fieldName)
        {
            return GetFieldValue(fieldName).ToString();
        }

        //retrieves the value from a given field - used to grab column descriptions and to grab the fields themselves
        private object GetFieldValue(string fieldName)
        {
            Type tableType = typeof(TableObject);
            FieldInfo fieldInfo = tableType.GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo.GetValue(this);
        }

        //given a key - figure out what type the value is. Needed to choose the proper field in fieldSwitch()
        private Type GetKeyType(string key)
        {
            Type dictType = null;

            //get the property dictionary location
            PropertyInfo[] props = typeof(TableObject).GetProperties();
            foreach (var prop in props)
            {
                if (prop.Name != "LinkStatus")
                {
                    var dictProp = prop.GetValue(this);
                    IDictionary dict = (IDictionary)dictProp;

                    if (dict.Contains(key))
                    {
                        dictType = dict[key].GetType();

                    }
                }
            }

            return dictType;

        }

        //performs a generic dictionary method for a given method (eg. add, remove) on the proper fieldDict, based on type
        //valType is the type of value contained in a dictionary
        //genericMethod is the method you want to invoke on the list or dictionary
        //dataType can be set to either dict or list - determines what type we're working with
        // methodArgs are the arguments passed to a method
        //override args are overloads to avoid ambiguous matches on a method, if required.
        private object Generic(Type valType, string genericMethod, string dataType,
            object[] methodArgs, System.Type[] overrideArgs = null)
        {

            if (valType == typeof(string))
            {
                var enumType = (dataType == "dict") ? typeof(FieldDict<string,string>) : typeof(List<string>);
                var genMethod = (overrideArgs == null) ? enumType.GetMethod(genericMethod) : 
                    enumType.GetMethod(genericMethod, overrideArgs);

                if (genMethod != null)
                {
                    if (dataType == "dict") return genMethod.Invoke(_textField, methodArgs);
                    return genMethod.Invoke(_textFieldKeys, methodArgs);
                }
                else return null;


            }
            else if (valType == typeof(int))
            {

                var enumType = (dataType == "dict") ? typeof(FieldDict<string, int>) : typeof(List<string>);
                var genMethod = (overrideArgs == null) ? enumType.GetMethod(genericMethod) :
                    enumType.GetMethod(genericMethod, overrideArgs);

                if (genMethod != null)
                {
                    if (dataType == "dict") return genMethod.Invoke(_integerField, methodArgs);
                    return genMethod.Invoke(_integerFieldKeys, methodArgs);
                }
                else return null;

            }
            else //if (valType == typeof(float))
            {
                var enumType = (dataType == "dict") ? typeof(FieldDict<string, float>) : typeof(List<string>);
                var genMethod = (overrideArgs == null) ? enumType.GetMethod(genericMethod) :
                    enumType.GetMethod(genericMethod, overrideArgs);

                if (genMethod != null)
                {
                    if (dataType == "dict") return genMethod.Invoke(_numberField, methodArgs);
                    return genMethod.Invoke(_numberFieldKeys, methodArgs);
                }
                else return null;

            }

        }

        //replaces an item in a list dependent on type
        private void GenericListReplace(Type type, int index, string key)
        {
            if (type == typeof(string))
            {
                _textFieldKeys[index] = key;
            }
            else if (type == typeof(int))
            {
                _integerFieldKeys[index] = key;
            }
            else // if (type = typeof(float))
            {
                _numberFieldKeys[index] = key;
            }
        }

    }
}
