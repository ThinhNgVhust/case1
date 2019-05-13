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
using CaseStudy1.Utils;

namespace CaseStudy1.Actions
{
    public class BalloonActions
    {

        const double DEFAULT_TEXT_HEIGHT = 5;
        const int DEFAULT_TEXT_COLOR = 1;

        public void ShowWindow()
        {
            try
            {
                ViewBallon viewBallon = new ViewBallon();
                BallonViewModel ballonViewModel = new BallonViewModel();
                ballonViewModel.GetData = new RelayCommand(GetDataInvoke);
                viewBallon.DataContext = ballonViewModel;
                ballonViewModel.Layers = DatabaseHelper.GetAllLayerFromCad();
                if (ballonViewModel.Layers != null && ballonViewModel.Layers.Count > 0)
                    ballonViewModel.LayerName = ballonViewModel.Layers.ElementAt(0);

                AcAp.Application.ShowModalWindow(viewBallon);
            }
            catch (Exception e)
            {
                AcAp.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(e.Message);
            }
        }

        protected void GetDataInvoke(object obj)
        {
            Window wnd = obj as Window;
            BallonViewModel result = wnd.DataContext as BallonViewModel;
            if (result != null)
            {
                string error = string.Empty;
                if (result.Validate(ref error))
                {
                    wnd.Close();
                    CreateAndGroupBallon(result.MyBallon);
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
        }

        private void CreateAndGroupBallon(Ballon ballon)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
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
                //Open the Layer table for read
                LayerTable layerTable;
                layerTable = trans.GetObject(db.LayerTableId,
                                                  OpenMode.ForRead) as LayerTable;

                if (layerTable.Has(layerName) != true)
                {
                    DatabaseHelper.CreateLayer(layerName);
                }
                line.Layer = layerName;

                ObjectId oLineId = DatabaseHelper.AppendEntityToDatabase(line);
                ObjectId oCircleId = DatabaseHelper.AppendEntityToDatabase(cir);
                ObjectId oTextId = DatabaseHelper.AppendEntityToDatabase(text);
                Group grp = new Group("Test group description", true);
                // Add the new group to the dictionary
                DBDictionary gd = trans.GetObject(db.GroupDictionaryId, OpenMode.ForRead) as DBDictionary;
                gd.UpgradeOpen();
                ObjectId grpId = gd.SetAt("GroupCaseStudy1", grp);
                trans.AddNewlyCreatedDBObject(grp, true);
                ObjectIdCollection ids = new ObjectIdCollection();
                ids.Add(oLineId);
                ids.Add(oCircleId);
                ids.Add(oTextId);
                grp.InsertAt(0, ids);
                string ballonXDataName = "BallonXdata";
                DatabaseHelper.AddRegAppTableRecord(ballonXDataName);
                ResultBuffer rb = CreateBalloonBuffer(ballon, ballonXDataName);

                grp.XData = rb;
                rb.Dispose();
                trans.Commit();
            }
        }

        public static void EditWindow(ObjectId groupId)
        {
            Group group = null;
            Ballon ballon = null;
            try
            {
                Document acDoc = AcAp.Application.DocumentManager.MdiActiveDocument;
                Database acCurDb = acDoc.Database;
                Editor editor = acDoc.Editor;
                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    group = acTrans.GetObject(groupId, OpenMode.ForRead) as Group;


                }

            }
            catch (Exception e)
            {

            }
            if (group != null)
            {
                ballon = ReadBallonFromBuffer(group.XData);
                ViewBallon viewBallon = new ViewBallon();
                BallonViewModel ballonViewModel = new BallonViewModel(); ballonViewModel.MyBallon = ballon;
                viewBallon.DataContext = ballonViewModel;
                ballonViewModel.GetData = new RelayCommand(EditDataInvoke);
            }


        }

        private static void EditDataInvoke(object obj)
        {
            //edit, update
            //save xdate
        }

        public static ResultBuffer CreateBalloonBuffer(Ballon ballon, string ballonXDataName)
        {
            var para0 = ballon.Radius;
            var para1 = ballon.Angle;
            var para2 = ballon.Length;
            var para3 = ballon.LayerName;
            var para4 = ballon.ColorIndex;
            string s = String.Format("R: {0}         A: {1}            L: {2}         Layer:{3}              Color:{4}", para0, para1, para2, para3, para4);
            MessageBox.Show(s);
            ResultBuffer rb =
                  new ResultBuffer(
                    new TypedValue(1001, ballonXDataName),// ten
                    new TypedValue(1000, para3),// ten layer
                    new TypedValue(1040, para1),//Angle
                    new TypedValue(1040, para2),//length
                     new TypedValue(1040, para0),//ban kinh
                      new TypedValue(1071, para4)//color index
                  );

            return rb;
        }


        public static Ballon ReadBallonFromBuffer(ResultBuffer buffer)
        {
            Ballon bal = null;
            TypedValue[] rvArr = buffer.AsArray();
            bal.LayerName = rvArr[1].Value as String;
            bal.Angle = (double)rvArr[2].Value;
            bal.Length = (double)rvArr[3].Value;
            bal.Radius = (double)rvArr[0].Value;
            bal.ColorIndex = (int)rvArr[4].Value;
            return bal;
        }

    }
}
