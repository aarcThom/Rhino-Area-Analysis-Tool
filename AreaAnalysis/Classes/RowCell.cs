using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using AreaAnalysis.Interfaces;
using Eto.Forms;
using Rhino;

namespace AreaAnalysis.Classes
{
    public class RowCell : INotifyPropertyChanged
    {
        private readonly Type _cellType;

        // GUID LINKAGE STUFF ==========================================================================

        //default fields and field values / descriptions for Modal
        private bool _linkVal = false;
        private static  readonly string LinkColText = "Link"; // change this to change the text for the Rhino link column header
        public static readonly string UnLinkedSymbol = "❌";
        public static readonly string LinkedSymbol = "✔️";

        //Rhino link
        private Guid _selectedBlock;

        private static string _defaultStringVal = "undefined";
        private string _stringVal; // generic string default per object

        private int _intVal = 0;

        private float _floatVal = 0f;
        private string _floatFormat = "F2";

        // lists for Modal - make sure you update these if you add new fields / column types
        private static readonly List<string> FieldNames = new List<string> 
        {   
            // {LinkColText}   // for _linkVal -- added to list in GetColumns()
            "String Column",   // for _stringVal
            "Integer Column", // for _intVal
            "Float Column"   // for _floatVal
        };

        private static readonly List<string> FieldDescriptions = new List<string>
        {
            "Establish a link with a Rhino object",  // for _linkVal
            "Define a string column",               // for _stringVal
            "Define an integer column",            //for _intVal
            "Define a float column"               // for _floatVal
        };

        private static readonly List<Type> FieldTypes = new List<Type>
        {
            typeof(bool),     // for _linkVal
            typeof(string),  // for _stringVal
            typeof(int),    // for _intVal
            typeof(float)  // for _floatVal
        };

        public RowCell(Type cellType)
        {
            _cellType = cellType;
            _stringVal = _defaultStringVal;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string CellValue
        {
            get => GetCellVal();
            set => SetCellVal(value);
        }

        private string GetCellVal()
        {
            if (_cellType == typeof(bool))
            {
                if (_linkVal) return LinkedSymbol;
                return UnLinkedSymbol;
            }
            else if (_cellType == typeof(string))
            {
                return _stringVal;
            }
            else if (_cellType == typeof(int))
            {
                return _intVal.ToString();
            }
            else // if (_cellType == typeof(float))
            {
                return _floatVal.ToString(_floatFormat);
            }
        }

        private void SetCellVal(string value)
        {

            if (_cellType == typeof(bool))
            {
                //deny this change
                // replace this with an event...maybe
                RhinoApp.WriteLine("How did you even get to the point where you could edit this link manually?"); 
            }
            else if (_cellType == typeof(string))
            {
                _stringVal = value;
                OnPropertyChanged(nameof(CellValue));
            } 
            else if (_cellType == typeof(int))
            {
                try
                {
                    _intVal = Int32.Parse(value);
                    OnPropertyChanged(nameof(CellValue));
                }
                catch
                {
                    RhinoApp.WriteLine("INVALID INT INPUT"); // replace this with an event...maybe

                }

            }
            else if (_cellType == typeof(float))
            {
                try
                {
                    _floatVal = Single.Parse(value);
                    OnPropertyChanged(nameof(CellValue));
                }
                catch
                {
                    RhinoApp.WriteLine("INVALID FLOAT INPUT"); // replace this with an event...maybe
                }
                
            }
        }

        public void SetFloatRounding(int floatingPoints)
        {
            _floatFormat = $"F{floatingPoints}";
        }

        public Type GetCellType()
        {
            return _cellType;
        }

        public static string GetLinkColumnText()
        {
            return LinkColText;
        }

        public bool CheckForDefaultName()
        {
            return _stringVal == _defaultStringVal;
        }

        public void EnableLink()
        {
            if (_cellType == typeof(bool))
            {
                _linkVal = true;
                OnPropertyChanged(nameof(CellValue));

            }
            else
            {
                throw new Exception("You cannot set link value in non Rhino link cell");
            }
        }

        public static (List<string> ColumnNames, List<string> ColumnDescriptions, List<Type> ColumnTypes) GetColumns()
        {
            // creating a shallow copy of fieldNames so I can add the linkName field
            List<string> fieldNames = new List<string>(FieldNames);
            List<string> fieldDescriptions = new List<string>(FieldDescriptions);
            List<Type> fieldTypes = new List<Type>(FieldTypes);

            fieldNames.Insert(0, LinkColText);
            return (fieldNames, fieldDescriptions, fieldTypes);
        }


    }
}
