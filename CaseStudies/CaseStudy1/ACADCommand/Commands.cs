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

namespace CaseStudy1.ACADCommand
{
    public class Commands
    {
        public static readonly double DEFAULT_TEXT_HEIGHT = 5;
        public static readonly int DEFAULT_TEXT_COLOR = 1;// red

        [CommandMethod("Insert_Ballon")]
        public static void Insert_Ballon()
        {
            try
            {
                ViewBallon viewBallon = new ViewBallon();
                Application.ShowModalWindow(viewBallon);
                Ballon ballon = GetBallonInforFromView(viewBallon);
                CreateAndGroupBallon(ballon);
            }
            catch (Exception e )
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(e.Message);
            }

        }



        [CommandMethod("Edit_Ballon")]
        public static void EditBallon()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor editor = acDoc.Editor;

            // Start a transaction
            ViewBallon viewBallon = new ViewBallon();
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                Line line = null;
                Circle circle = null;
                DBText text = null;

                // Request for objects to be selected in the drawing area
                PromptSelectionResult acSSPrompt = acDoc.Editor.GetSelection();
                // If the prompt status is OK, objects were selected
                if (acSSPrompt.Status == PromptStatus.Cancel) return;



                line = acTrans.GetObject(acSSPrompt.Value[0].ObjectId, OpenMode.ForWrite) as Line;
                circle = acTrans.GetObject(acSSPrompt.Value[1].ObjectId, OpenMode.ForWrite) as Circle;
                text = acTrans.GetObject(acSSPrompt.Value[2].ObjectId, OpenMode.ForWrite) as DBText;




                // Dispose of the transaction
                editor.WriteMessage(line.Length.ToString() + " " + line.StartPoint.ToString());
                editor.WriteMessage(circle.Radius.ToString());
                editor.WriteMessage(text.TextString.ToString());




                //viewBallon.tbLength.Text = line.Length.ToString();
                //Point3d startPoint = line.StartPoint;
                //Point3d endPoint = line.EndPoint;
                //double h = endPoint.Y - startPoint.Y;
                //double w = endPoint.X - startPoint.X;
                //double angleOut = Math.Atan(h / w) * 180 / Math.PI;
                //editor.WriteMessage("thinh dep trai");
                //viewBallon.tbAngle.Text = "199";
                switch (line.Layer.ToLower())
                {
                    case "layer#1":
                        viewBallon.ccbLayer.SelectedIndex = 0;
                        break;
                    case "layer#2":
                        viewBallon.ccbLayer.SelectedIndex = 1;
                        break;
                    case "layer#3":
                        viewBallon.ccbLayer.SelectedIndex = 2;
                        break;
                    default:
                        break;
                }





                //BlockTable bt = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                //BlockTableRecord ms = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                //LayerTable layerTable;
                //layerTable = acTrans.GetObject(acCurDb.LayerTableId,
                //                                  OpenMode.ForRead) as LayerTable;

                //double newLength = editBallon.Length;


                //double newAngle = editBallon.Angle;


                //double newRadius = editBallon.Radius;

                ////line.Layer = layerName;line.EndPoint = new Point3d(line.StartPoint.X + );
                //// Save the new object to the database
                //int newColorIndex = editBallon.ColorIndex+ 1;

                //string layerName = editBallon.LayerName;
                //if (layerTable.Has(layerName) == false)
                //{
                //    LayerTableRecord ltr = new LayerTableRecord();
                //    layerTable.UpgradeOpen();
                //    layerTable.Add(ltr);
                //    ltr.Name = layerName;
                //    acTrans.AddNewlyCreatedDBObject(ltr, true);
                //}
                //line.Layer = layerName;
                //line.ColorIndex = newColorIndex;
                //line.EndPoint = new Point3d(line.StartPoint.X + newLength * Math.Cos(newAngle * Math.PI / 180),
                //    line.StartPoint.Y + newLength * Math.Sin(newAngle * Math.PI / 180), 0);

                //circle.Radius = newRadius;
                //circle.Center = new Point3d(line.EndPoint.X + newRadius * Math.Cos(newAngle * Math.PI / 180),
                //    line.EndPoint.Y + newRadius * Math.Sin(newAngle * Math.PI / 180), 0);
                //text.TextString = viewBallon.tbStringInput.ToString();
                //text.AlignmentPoint = circle.Center;
                //line.UpgradeOpen(); circle.UpgradeOpen(); text.UpgradeOpen();
                acTrans.Commit();
            }
           
            Application.ShowModalWindow(viewBallon);


        }

        static void AddRegAppTableRecord(string regAppName)

        {

            Document doc =

              Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;

            Database db = doc.Database;


            Transaction tr =

              doc.TransactionManager.StartTransaction();

            using (tr)

            {

                RegAppTable rat =

                  (RegAppTable)tr.GetObject(

                    db.RegAppTableId,

                    OpenMode.ForRead,

                    false

                  );

                if (!rat.Has(regAppName))

                {

                    rat.UpgradeOpen();

                    RegAppTableRecord ratr =

                      new RegAppTableRecord();

                    ratr.Name = regAppName;

                    rat.Add(ratr);

                    tr.AddNewlyCreatedDBObject(ratr, true);

                }

                tr.Commit();

            }

        }

        private static Ballon GetBallonInforFromView(ViewBallon viewBallon)
        {
            double angle;
            double.TryParse(viewBallon.tbAngle.Text, out angle);
            double length;
            double.TryParse(viewBallon.tbLength.Text, out length);
            double radius;
            double.TryParse(viewBallon.tbRadius.Text, out radius);
            int colorIndex = viewBallon.ccbColor.SelectedIndex + 1;// 0 red 1green 2blue 
            ComboBoxItem comboBoxItem = (ComboBoxItem)viewBallon.ccbColor.SelectedItem;
            comboBoxItem = (ComboBoxItem)viewBallon.ccbLayer.SelectedItem;
            string layerName = comboBoxItem.Content.ToString();
            string textInput = viewBallon.tbStringInput.Text;
            Ballon ballon = new Ballon();
            ballon.Angle = angle;
            ballon.Length = length;
            ballon.Radius = radius;
            ballon.ColorIndex = colorIndex;
            ballon.LayerName = layerName;
            ballon.StringInput = textInput;
            return ballon;
        }

        private static void CreateAndGroupBallon(Ballon ballon)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();
            using (trans)
            {
                PromptPointResult pPtRes;
                PromptPointOptions pPtOpts = new PromptPointOptions("Enter Base point[X,Y]: ");
                pPtRes = doc.Editor.GetPoint(pPtOpts);
                if (pPtRes.Status == PromptStatus.Cancel) return;
                double angle = ballon.Angle;
                double length = ballon.Length;
                double radius = ballon.Radius;
                int colorIndex = ballon.ColorIndex;
                string layerName = ballon.LayerName;
                string textInput = ballon.StringInput;
                Point3d startPoint = pPtRes.Value;
                double radian = angle * Math.PI / 180;
                Point3d endPoint = new Point3d(startPoint.X + length * Math.Cos(radian), startPoint.Y + length * Math.Sin(radian), startPoint.Z);
                Line line = new Line(startPoint, endPoint);
                short index = (short)colorIndex;
                line.ColorIndex = colorIndex;
                Point3d center = new Point3d(startPoint.X + (length + radius) * Math.Cos(radian), startPoint.Y + (length + radius) * Math.Sin(radian), startPoint.Z);
                Circle cir = new Circle();
                cir.SetDatabaseDefaults();
                cir.Center = center;
                cir.Radius = radius;
                DBText text = new DBText();
                text.VerticalMode = TextVerticalMode.TextVerticalMid;
                text.HorizontalMode = TextHorizontalMode.TextCenter;
                text.AlignmentPoint = center;
                text.ColorIndex = DEFAULT_TEXT_COLOR;
                text.Height = DEFAULT_TEXT_HEIGHT;
                text.TextString = textInput;
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord ms = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);


                // Open the Layer table for read
                LayerTable layerTable;
                layerTable = trans.GetObject(db.LayerTableId,
                                                  OpenMode.ForRead) as LayerTable;

                if (layerTable.Has(layerName) == false)
                {
                    LayerTableRecord ltr = new LayerTableRecord();
                    layerTable.UpgradeOpen();
                    layerTable.Add(ltr);
                    ltr.Name = layerName;
                    trans.AddNewlyCreatedDBObject(ltr, true);
                }

                line.Layer = layerName;
                //create group
                Group grp = new Group("Test group", true);
                // Add the new group to the dictionary

                DBDictionary gd = trans.GetObject(db.GroupDictionaryId, OpenMode.ForRead) as DBDictionary;
                gd.UpgradeOpen();
                ObjectId grpId = gd.SetAt("GroupCaseStudy1", grp);
                trans.AddNewlyCreatedDBObject(grp, true);
                ObjectIdCollection ids = new ObjectIdCollection();
                DBObjectCollection ents = new DBObjectCollection();
                ents.Add(line);
                ents.Add(cir);
                ents.Add(text);
                foreach (Entity ent in ents)
                {
                    ObjectId id = ms.AppendEntity(ent);
                    ids.Add(id);
                    trans.AddNewlyCreatedDBObject(ent, true);
                }
                grp.InsertAt(0, ids);

                AddRegAppTableRecord("Thinh");
                ResultBuffer rb =
                  new ResultBuffer(
                    new TypedValue(1001, "Thinh"),
                    new TypedValue(1000, "Ballon co 1 circle, 1 line va 1 text")
                  );
                grp.XData = rb;
                rb.Dispose();
                trans.Commit();
            }
        }
    }
}
