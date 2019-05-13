using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CaseStudy1.View;
using CaseStudy1.ViewModel;
using System.Windows.Controls;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Colors;
using CaseStudy1.Model;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
using System.Threading;
using System.Windows;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using CaseStudy1.Actions;
using System.Collections;

namespace CaseStudy1.ACADCommand
{
    public class Commands
    {

        [CommandMethod("Insert_Ballon")]
        public void Insert_Ballon()
        {
            new BalloonActions().ShowWindow();
        }

        [CommandMethod("Edit_Ballon")]
        public void EditBallon()
        {
            // Get the current document and database
            Document acDoc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            ObjectId groupId = ObjectId.Null;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Request for objects to be selected in the drawing area
                PromptEntityResult entres = editor.GetEntity("\nSelect entity: ");
              
                if (entres.Status == PromptStatus.OK)
                {
                    Entity en = (Entity)acTrans.GetObject(entres.ObjectId, OpenMode.ForRead);
                    ObjectIdCollection objectIdCollection = en.GetPersistentReactorIds();
                    foreach (ObjectId item in objectIdCollection)
                    {
                        Group dB = acTrans.GetObject(item, OpenMode.ForRead)  as Group;
                        if (dB != null) {
                            groupId = item;
                            break;
                        }
                    }
                    
                }
               
            }
            if (!groupId.IsNull) {
                BalloonActions.EditWindow(groupId);
            }
            
          


        }
        
    }
}
