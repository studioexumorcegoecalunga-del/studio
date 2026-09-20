using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class Structural3dCommands
{
 [CommandMethod("RFESTRUTURA3D")]
 public void Build(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;int cols=0,beams=0;
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var ids=ms.Cast<ObjectId>().ToArray();
  foreach(var id in ids){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;
   if(e is Polyline p&&p.Layer=="RF-EST-PILAR"&&p.Closed){double h=2.80,z=0;Storey(e,tr,ref z,ref h);using var curves=new DBObjectCollection();curves.Add((Polyline)p.Clone());using var regs=Region.CreateFromCurves(curves);if(regs.Count==0)continue;using var reg=(Region)regs[0];var s=new Solid3d();s.CreateExtrudedSolid(reg,new Vector3d(0,0,h),new SweepOptions());s.TransformBy(Matrix3d.Displacement(new Vector3d(0,0,z)));s.Layer="RF-EST-3D";ms.AppendEntity(s);tr.AddNewlyCreatedDBObject(s,true);cols++;}
   else if(e is Line l&&l.Layer=="RF-EST-VIGA"){double z=0,hstorey=2.80;Storey(e,tr,ref z,ref hstorey);double bw=.15,bh=.40;Struct(e,tr,ref bw,ref bh);var len=l.Length;var s=new Solid3d();s.CreateBox(len,bw,bh);var ang=Math.Atan2(l.EndPoint.Y-l.StartPoint.Y,l.EndPoint.X-l.StartPoint.X);s.TransformBy(Matrix3d.Rotation(ang,Vector3d.ZAxis,Point3d.Origin));s.TransformBy(Matrix3d.Displacement(new Vector3d(l.StartPoint.X,l.StartPoint.Y,z+hstorey-bh)));s.Layer="RF-EST-3D";ms.AppendEntity(s);tr.AddNewlyCreatedDBObject(s,true);beams++;}
  }tr.Commit();d.Editor.WriteMessage($"\nModelo estrutural 3D preliminar: {cols} pilar(es), {beams} viga(s).");}
 static void Storey(Entity e,Transaction tr,ref double z,ref double h){if(e.ExtensionDictionary.IsNull)return;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_STOREY"))return;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_STOREY"),OpenMode.ForRead)).Data?.AsArray();if(a?.Length>3){z=Convert.ToDouble(a[2].Value);h=Convert.ToDouble(a[3].Value);}}
 static void Struct(Entity e,Transaction tr,ref double a,ref double b){if(e.ExtensionDictionary.IsNull)return;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_STRUCT"))return;var x=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_STRUCT"),OpenMode.ForRead)).Data?.AsArray();if(x?.Length>2&&x[1].Value is double){a=Convert.ToDouble(x[1].Value);b=Convert.ToDouble(x[2].Value);}}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-3D"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-EST-3D"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
