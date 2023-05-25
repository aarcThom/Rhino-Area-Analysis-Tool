﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace AreaAnalysis.Classes
{
    class EtoMethods
    {
        public static List<string> GetGridViewHeaders(GridView gView)
        {
            List<string> rhinoHeaders = new List<string>();
            foreach (var rHeader in gView.Columns)
            {
                rhinoHeaders.Add(rHeader.HeaderText);
            }
            return rhinoHeaders;
        }
    }
}