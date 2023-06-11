using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
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
            GetObject go = new GetObject();
            go.SetCommandPrompt("Select stuff");
            go.GroupSelect = true;
            go.SubObjectSelect = false;
            go.EnableClearObjectsOnEntry(false);
            go.EnableUnselectObjectsOnExit(false);
            go.DeselectAllBeforePostSelect = false;

            bool bHavePreselectedObjects = false;

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
                    bHavePreselectedObjects = true;
                    go.EnablePreSelect(false, true);
                    continue;
                }

                break;
            }

            if (bHavePreselectedObjects)
            {
                for (int i = 0; i < go.ObjectCount; i++)
                {
                    RhinoObject rhinoObject = go.Object(i).Object();
                    if (null != rhinoObject)
                        rhinoObject.Select(false);
                }

                RhinoDoc.ActiveDoc.Views.Redraw();
            }

            return (Result.Success, go.Objects());

        }
    }
}