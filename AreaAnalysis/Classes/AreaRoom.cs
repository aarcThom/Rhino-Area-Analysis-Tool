using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
namespace AreaAnalysis.Classes
{
    public class AreaRoom
    {
        public bool valid = false;
        public ObjRef roomOutline;
        public AreaRoom() {

            // initializing the class with a get object
            // NOTE: YOU SHOULD ADD A BOOL THAT TOGGLES WHETHER THE OBJECT IS PICKED OR PRE-LOADED
            var gc = new GetObject();
            gc.SetCommandPrompt("Select a closed curve, polycurve, or polyline that represents a room");
            gc.GeometryFilter = ObjectType.Curve;
            gc.SubObjectSelect = false;
            gc.DisablePreSelect();
            gc.GeometryAttributeFilter = GeometryAttributeFilter.ClosedCurve;

            gc.Get();

            if (gc.CommandResult() == Result.Success && gc.Object(0).Curve() != null)
            { 
                valid = true;
                roomOutline = gc.Object(0);
            }

        }
    }


}
