using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralIssueCommands
{
 [CommandMethod("RFINCONSISTENCIAS")]
 public void Mark(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var cols=new List<Point3d>();var beams=new List<Line>();foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add(new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add(l);}
  int n=0;foreach(var b in beams){int supports=cols.Count(c=>Dist(c,b.StartPoint,b.EndPoint)<=.25);if(supports>=2)continue;var m=new DBText{Position=new Point3d((b.StartPoint.X+b.EndPoint.X)/2,(b.StartPoint.Y+b.EndPoint.Y)/2,0),Height=.16,TextString=supports==0?"! VIGA SEM APOIO":"! REVISAR APOIOS",Layer="RF-EST-ALERTA"};ms.AppendEntity(m);tr.AddNewlyCreatedDBObject(m,true);n++;}tr.Commit();d.Editor.WriteMessage($"\nInconsistências estruturais marcadas no desenho: {n}.");
 }
 static double Dist(Point3d p,Point3d a,Point3d b){var ab=b-a;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(a);var t=Math.Max(0,Math.Min(1,(p-a).DotProduct(ab)/den));return p.DistanceTo(a+ab*t);}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-ALERTA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-EST-ALERTA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
