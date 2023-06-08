using AreaAnalysis.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreaAnalysis.Interfaces
{
    public interface ICellValue
    {
        string ValueToString();
        void SetCellValue(object value);
    }
}
