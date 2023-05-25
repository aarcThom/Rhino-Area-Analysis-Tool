using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

using AreaAnalysis.Classes;
using Rhino.UI;
using AreaAnalysis.Views;

namespace AreaAnalysis.Commands
{
    public class AreaTracker : Command
    {
        public AreaTracker()
        {
            System.Drawing.Icon panelIco = new System.Drawing.Icon(System.Drawing.SystemIcons.Exclamation, 40, 40);

            Panels.RegisterPanel(PlugIn, typeof(Views.MainPluginPanel), "Area Tracking", panelIco);
            Instance = this;
        }

        public static AreaTracker Instance
        {
            get;
            private set;
        }

        public override string EnglishName
        {
            get { return "AreaTracker"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panelId = Views.MainPluginPanel.PanelId;
            var visible = Panels.IsPanelVisible(panelId);


            if (!visible)
            {
                Panels.OpenPanel(panelId);
                Rhino.RhinoApp.WriteLine("Area Tracker Go!");
            }
            else
            {
                Rhino.RhinoApp.WriteLine("The area tracker panel is already open!");
            }

            return Result.Success;
        }
    }
        
}
