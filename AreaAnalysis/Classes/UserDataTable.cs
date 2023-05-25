using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.DocObjects;

namespace AreaAnalysis.Classes
{
    class UserDataTable
    {
        // the Rhino UserData values
        private string pluginKey = "AARCAreaAnalysisTool~"; //make sure we have a unique key
        private List<string> valueLists = new List<string>();

        // the plugin dictionary to keep track of the changes
        public Dictionary<string, List<string>> docDictionary = new Dictionary<string, List<string>>();

        public void AddColumn(string colName)
        {
            docDictionary.Add(colName, new List<string>());
        }
    }
}
