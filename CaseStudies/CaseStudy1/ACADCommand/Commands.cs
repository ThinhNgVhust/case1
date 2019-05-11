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

namespace CaseStudy1.ACADCommand
{
    public class Commands
    {
        [CommandMethod("Insert_Ballon")]
        public static void Insert_Ballon()
        {
            ViewBallon viewBallon = new ViewBallon();
            BallonViewModel ballonViewModel = new BallonViewModel();
            var result = Application.ShowModalWindow(viewBallon);
            double angle;
            double.TryParse(viewBallon.tbAngle.Text, out angle);
            double length;
            double.TryParse(viewBallon.tbLength.Text, out length);
            double radius;
            double.TryParse(viewBallon.tbRadius.Text, out radius);
            int colorIndex = viewBallon.ccbColor.SelectedIndex;// 0red 1green 2blue 
            ComboBoxItem comboBoxItem = (ComboBoxItem)viewBallon.ccbColor.SelectedItem;
            comboBoxItem = (ComboBoxItem)viewBallon.ccbLayer.SelectedItem;
            string layerName = comboBoxItem.Content.ToString();
            string textInput = viewBallon.tbStringInput.Text;
            CreateAndGroupBallon(angle, length, radius, colorIndex, layerName, textInput);

        }

        private static void CreateAndGroupBallon(double angle, double length, double radius, int colorIndex, string layerName, string textInput)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();
            using (trans)
            {
                PromptPointResult pPtRes;
                PromptPointOptions pPtOpts = new PromptPointOptions("Enter point[X,Y]: ");
                pPtRes = doc.Editor.GetPoint(pPtOpts);
                if (pPtRes.Status == PromptStatus.Cancel) return;

                Point3d startPoint = pPtRes.Value;
                double radian = angle * Math.PI / 180;
                Point3d endPoint = new Point3d(startPoint.X + length * Math.Cos(radian), startPoint.Y + length * Math.Sin(radian), startPoint.Z);
                Line line = new Line(startPoint, endPoint);
                line.ColorIndex = colorIndex;

                Point3d center = new Point3d(startPoint.X + (length + radius) * Math.Cos(radian), startPoint.Y + (length + radius) * Math.Sin(radian), startPoint.Z);
                Circle cir = new Circle();
                cir.SetDatabaseDefaults();
                cir.Center = center;
                cir.Radius = radius;
                cir.ColorIndex = colorIndex;

                DBText text = new DBText();
                text.Position = center;
                text.Height = 2;
                //text.AlignmentPoint = center;
               
                text.TextString = textInput;
                
                // Open the Layer table for read
                LayerTable layerTable;
                layerTable = trans.GetObject(db.LayerTableId,
                                                  OpenMode.ForRead) as LayerTable;

                //if (!layerTable.Has(layerName))
                //{
                //    LayerTableRecord ltr = new LayerTableRecord();
                //    ltr.Name = layerName;
                //    trans.AddNewlyCreatedDBObject(ltr, true);
                //}
                //line.Layer = layerName;

                //create group
                Group grp = new Group("Test group", true);
                // Add the new group to the dictionary
                DBDictionary gd =  trans.GetObject(  db.GroupDictionaryId,  OpenMode.ForRead  ) as DBDictionary;
                gd.UpgradeOpen();
                trans.AddNewlyCreatedDBObject(grp, true);

                // Open the model-space
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                //add lines, ballons, layer to groups
                // (the entities belong to the model-space)
                ObjectIdCollection ids = new ObjectIdCollection();
                DBObjectCollection ents = new DBObjectCollection() { line, cir,text };
                foreach (Entity ent in ents)
                {
                    ObjectId id = ms.AppendEntity(ent);
                    ids.Add(id);
                    trans.AddNewlyCreatedDBObject(ent, true);
                }
                ObjectId grpId = gd.SetAt("group ballon", grp);
                grp.InsertAt(0, ids);
                trans.Commit();
                //save xdata
            }
        }
    }
}
