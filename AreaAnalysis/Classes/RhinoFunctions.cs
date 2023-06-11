using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static (Result, ObjRef[]) UserSelect(RhinoDoc doc)
        {

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
            var selectedObjs =  doc.Objects.GetSelectedObjects(true,true);
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

        public static Result CreateBlock(ObjRef[] objects, string blockName, RhinoDoc doc)
        {

            // set block base point
            Point3d basePoint3d;
            var rc = RhinoGet.GetPoint("Block base point", false, out basePoint3d);
            if (rc != Result.Success)
            {
                return rc;
            }

            // See if block name already exists
            InstanceDefinition existingIdef = doc.InstanceDefinitions.Find(blockName);
            if (existingIdef != null)
            {
                RhinoApp.WriteLine($"Block definition {blockName} already exists. " +
                                         "Please rename your column or delete the existing block of the same name");
                return Result.Nothing;
            }

            // Gather all of the selected objects
            var geometry = new List<GeometryBase>();
            var attributes = new List<ObjectAttributes>();
            foreach (var obj in objects)
            {
                if (obj != null)
                {
                    geometry.Add(obj.Object().Geometry);
                    attributes.Add(obj.Object().Attributes);
                }
            }
            // Gather all of the selected objects
            int idefIndex = doc.InstanceDefinitions.Add(blockName, string.Empty, basePoint3d, geometry, attributes);

            if (idefIndex < 0)
            {
                RhinoApp.WriteLine("Unable to create block definition", blockName);
                return Result.Failure;
            }

            // Create a block instance
            Transform t = Transform.Translation(basePoint3d.X,basePoint3d.Y,basePoint3d.Z);
            doc.Objects.AddInstanceObject(idefIndex, t);

            //delete the original geometry
            foreach (var obj in objects)
            {
                doc.Objects.Delete(obj.ObjectId, true);
            }

            doc.Views.Redraw();

            return Result.Success;
        }

    }
}