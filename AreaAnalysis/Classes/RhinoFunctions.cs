using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using ObjRef = Rhino.DocObjects.ObjRef;

namespace AreaAnalysis.Classes
{
    public class RhinoFunctions
    {
        public static (Result, ObjRef[]) UserSelect()
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            // Phantoms, grips, lights, etc., cannot be in blocks.
            const ObjectType forbiddenGeoFilter = ObjectType.Light |
                                                  ObjectType.Grip | 
                                                  ObjectType.Phantom;

            const ObjectType geoFilter = forbiddenGeoFilter ^ ObjectType.AnyObject;

            GetObject go = new GetObject();
            go.GeometryFilter = geoFilter;
            go.SetCommandPrompt("Select stuff");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;


            //deselect invalid objects before post selection
            var selectedObjs = doc.Objects.GetSelectedObjects(true,true);
            foreach (var obj in selectedObjs)
            {
                if (obj.ObjectType == ObjectType.Light ||
                    obj.ObjectType == ObjectType.Grip ||
                    obj.ObjectType == ObjectType.Phantom )
                {
                    obj.Select(false);
                }
            }
            doc.Views.Redraw();

            while (true)
            {
                var result = go.GetMultiple(1, 0);

                if (result == GetResult.Cancel)
                {
                    RhinoApp.WriteLine("user cancelled");
                    return (Result.Cancel, null);
                }

                if (result == GetResult.Nothing)
                {
                    RhinoApp.WriteLine("nothing selected");
                    return (Result.Cancel, null);
                }

                if (go.ObjectsWerePreselected)
                {
                    go.EnablePreSelect(false, true);
                    continue;
                }

                break;
            }


            //make sure objects are selected at end of command
            foreach (var obj in go.Objects())
            {
                obj.Object().Select(true);
            }

            doc.Views.Redraw();

            return (Result.Success, go.Objects());

        }

        public static Result CreateBlock(ObjRef[] objects, string blockName)
        {
            // set block base point
            Rhino.Geometry.Point3d basePoint3d;
            var rc = Rhino.Input.RhinoGet.GetPoint("Block base point", false, out basePoint3d);
            if (rc != Result.Success)
            {
                return rc;
            }

            return Result.Success;
        }

    }
}