using System;
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

namespace CaseStudy1.Utils
{
    public class DatabaseHelper
    {
        public static void AddRegAppTableRecord(string regAppName)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                RegAppTable rat = tr.GetObject(db.RegAppTableId, OpenMode.ForRead, false) as RegAppTable;
                if (!rat.Has(regAppName))
                {
                    rat.UpgradeOpen();
                    RegAppTableRecord ratr = new RegAppTableRecord();
                    ratr.Name = regAppName; rat.Add(ratr);
                    tr.AddNewlyCreatedDBObject(ratr, true);
                }
                tr.Commit();
            }
        }

        public static List<string> GetAllLayerFromCad()
        {
            Document acDoc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            List<string> result = new List<string>();
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // This example returns the layer table for the current database
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;

                // Step through the Layer table and print each layer name
                foreach (ObjectId acObjId in acLyrTbl)
                {
                    LayerTableRecord acLyrTblRec;
                    acLyrTblRec = acTrans.GetObject(acObjId,
                                                    OpenMode.ForRead) as LayerTableRecord;

                    result.Add(acLyrTblRec.Name);

                }
                return result;
            }
        }
            public static ObjectId AppendEntityToDatabase(Entity ent)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();

            using (trans)
            {
                BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord blockTableRecord = (BlockTableRecord)trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                blockTableRecord.AppendEntity(ent);
                trans.AddNewlyCreatedDBObject(ent, true);
                trans.Commit();
                doc.Editor.WriteMessage("\nAdd success");
            }
            return ent.ObjectId;
        }

        public static void CreateLayer(string layerName)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();

            using (trans)
            {
                LayerTable layerTable;
                layerTable = trans.GetObject(db.LayerTableId,
                                                  OpenMode.ForRead) as LayerTable;
                LayerTableRecord ltr = new LayerTableRecord();
                layerTable.UpgradeOpen();
                layerTable.Add(ltr);
                ltr.Name = layerName;
                trans.AddNewlyCreatedDBObject(ltr, true);
                trans.Commit();
            }
        }
    }
}
