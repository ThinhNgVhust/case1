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
        const string BALLON_XDATA_NAME= "BallonXdata";

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

        private void GetDataInvoke(object obj)
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
                try
                {
                    PromptPointResult pPtRes;
                    PromptPointOptions pPtOpts = new PromptPointOptions("Enter Base point[X,Y]: ");
                    pPtRes = doc.Editor.GetPoint(pPtOpts);
                    if (pPtRes.Status == PromptStatus.Cancel) return;
                    Point3d startPoint = pPtRes.Value;
                    //Line line = new Line(startPoint, endPoint);
                    Line line = CreateLine(startPoint, ballon.Angle,ballon.Length,ballon.LayerName,ballon.ColorIndex);
                    Circle cir = CreateCirle(line.EndPoint,ballon.Angle,ballon.Radius);
                    DBText text = CreateText(cir.Center, ballon.StringInput);
                    //Open the Layer table for read
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
                    ResultBuffer rb = CreateBalloonBuffer(ballon, BALLON_XDATA_NAME);
                    grp.XData = rb;
                    rb.Dispose();
                    trans.Commit();
                }
                catch (System.Exception e)
                {
                    trans.Abort();
                }
            }
        }

        private DBText CreateText(Point3d center, string stringInput)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();
            using (trans)
            {
                try
                {
                    DBText text = new DBText();
                    text.VerticalMode = TextVerticalMode.TextVerticalMid;
                    text.HorizontalMode = TextHorizontalMode.TextCenter;
                    text.AlignmentPoint = center;
                    text.ColorIndex = DEFAULT_TEXT_COLOR;
                    text.Height = DEFAULT_TEXT_HEIGHT;
                    text.TextString = stringInput;
                    return text;
                }
                catch (System.Exception e)
                {
                    trans.Abort();
                    throw e;
                }
            }
        }

        private Circle CreateCirle(Point3d endPoint, double angle, double radius)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();
            using (trans)
            {
                try
                {
                    double radian = angle * Math.PI / 180;
                    Point3d centerPoint = new Point3d(endPoint.X + radius * Math.Cos(radian), endPoint.Y + radius * Math.Sin(radian), endPoint.Z);
                    Circle circle = new Circle();
                    circle.Center = centerPoint;
                    circle.Radius = radius;
                    return circle;
                }
                catch (System.Exception e)
                {
                    trans.Abort();
                    throw e;
                }
            }
        }

        private Line CreateLine(Point3d startPoint, double angle, double length, string layerName, int colorIndex)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();
            using (trans)
            {
                try
                {
                    double radian = angle * Math.PI / 180;
                    Point3d endPoint = new Point3d(startPoint.X + length * Math.Cos(radian), startPoint.Y + length * Math.Sin(radian), startPoint.Z);
                    Line line = new Line();
                    line.StartPoint = startPoint;
                    line.EndPoint = endPoint;
                    line.ColorIndex = colorIndex;
                    LayerTable layerTable;
                    layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                    if (layerTable.Has(layerName) != true)
                    {
                        DatabaseHelper.CreateLayer(layerName);
                    }
                    line.Layer = layerName;
                    trans.Commit();
                    return line;
                }
                catch (System.Exception e )
                {
                    trans.Abort();
                    throw e;
                }
            }
        }

        public static void EditWindow(ObjectId groupId)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Group group = null;
            Ballon ballon = null;
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    group = acTrans.GetObject(groupId, OpenMode.ForWrite) as Group;
                    acTrans.Commit();
                }
                catch (System.Exception)
                {
                    acTrans.Abort();
                }
            }

            if (group != null)
            {
                ballon = ReadBallonFromBuffer(group.XData);
                ViewBallon viewBallon = new ViewBallon();
                BallonViewModel ballonViewModel = new BallonViewModel();
                ballonViewModel.BallonObjectId = groupId;
                ballonViewModel.MyBallon = ballon;
                viewBallon.DataContext = ballonViewModel;
                ballonViewModel.GetData = new RelayCommand(EditDataInvoke);
                ballonViewModel.Layers = DatabaseHelper.GetAllLayerFromCad();
                AcAp.Application.ShowModalWindow(viewBallon);
               
            }
        }

        private static void EditDataInvoke(object obj)
        {
            Window wnd = obj as Window;
            BallonViewModel ballonViewModel = wnd.DataContext as BallonViewModel;
            if (ballonViewModel != null)
            {
                string error = string.Empty;
                if (ballonViewModel.Validate(ref error))
                {
                    EditBallon(ballonViewModel);
                    wnd.Close();
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
        }

        private static void EditBallon(BallonViewModel ballonViewModel)
        {
            Document doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Transaction trans = doc.TransactionManager.StartTransaction();
            using (trans)
            {
                try
                {
                    Group group = trans.GetObject(ballonViewModel.BallonObjectId, OpenMode.ForWrite) as Group;
                    ObjectId[] objectIds = group.GetAllEntityIds();
                    Line line = null;
                    Circle circle = null;
                    DBText dBText = null;
                    foreach (ObjectId item in objectIds)
                    {
                        DBObject ent = trans.GetObject(item, OpenMode.ForWrite);
                        if (ent is Line)
                        {
                            line = ent as Line;
                        }
                        else if (ent is Circle)
                        {
                            circle = ent as Circle;
                        }
                        else if (ent is DBText)
                        {
                            dBText = ent as DBText;
                        }
                    }
                    //edit graphical properties for ballon
                    dBText.TextString = ballonViewModel.StringInput;
                    double newLenth = ballonViewModel.Length;
                    double newRadian = ballonViewModel.Angle * Math.PI / 180;
                    line.EndPoint = new Point3d(line.StartPoint.X + newLenth * Math.Cos(newRadian), line.StartPoint.Y + newLenth * Math.Sin(newRadian), line.StartPoint.Z);
                    line.Layer = ballonViewModel.LayerName;
                    line.ColorIndex = ballonViewModel.ColorIndex;
                    double newRadius = ballonViewModel.Radius;
                    circle.Radius = newRadius;
                    circle.Center = new Point3d(line.EndPoint.X + newRadius * Math.Cos(newRadian), line.EndPoint.Y + newRadius * Math.Sin(newRadian), line.EndPoint.Z);
                    dBText.AlignmentPoint = circle.Center;
                    //edit xdata
                    ResultBuffer rb = CreateBalloonBuffer(ballonViewModel.MyBallon, BALLON_XDATA_NAME);
                    group.XData = rb;// new ResultBuffer(new TypedValue(1001, BALLON_XDATA_NAME));
                    rb.Dispose();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Abort();
                    throw e;
                }
            }
        }

        public static ResultBuffer CreateBalloonBuffer(Ballon ballon, string BALLON_XDATA_NAME)
        {
             DatabaseHelper.AddRegAppTableRecord(BALLON_XDATA_NAME);
            ResultBuffer rb =
                  new ResultBuffer(
                    new TypedValue((int)DxfCode.ExtendedDataRegAppName, BALLON_XDATA_NAME),// ten
                    new TypedValue((int)DxfCode.ExtendedDataAsciiString, ballon.LayerName),// ten layer
                    new TypedValue((int)DxfCode.ExtendedDataReal, ballon.Angle),//Angle
                    new TypedValue((int)DxfCode.ExtendedDataReal, ballon.Length),//length
                    new TypedValue((int)DxfCode.ExtendedDataReal, ballon.Radius),//ban kinh
                    new TypedValue((int)DxfCode.ExtendedDataInteger32, ballon.ColorIndex),//color index
                    new TypedValue((int)DxfCode.ExtendedDataAsciiString, ballon.StringInput)//string input
                  );
            return rb;
        }


        public static Ballon ReadBallonFromBuffer(ResultBuffer buffer)
        {

            try
            {
                Ballon bal = new Ballon();
                TypedValue[] rvArr = buffer.AsArray();
                bal.LayerName = (string)rvArr[1].Value;
                bal.Angle = (double)rvArr[2].Value;
                bal.Length = (double)rvArr[3].Value;
                bal.Radius = (double)rvArr[4].Value;
                bal.ColorIndex = (int)rvArr[5].Value;
                bal.StringInput = (string)rvArr[6].Value;
                return bal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
