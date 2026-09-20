using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.ThreeD;
public sealed class Parapet3dCommands
{
 [CommandMethod("RFPLATIBANDA3D")]
 public void Build(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;int n=0;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);Ensure(tr,db);var ids=ms.Cast<ObjectId>().ToArray();
  foreach(var id in ids){if(tr.GetObject(id,OpenMode.ForRead) is not Polyline p||p.Layer!="RF-ARQ-PLATIBANDA"||!p.Closed)continue;double h=.80,z=0;
   if(!p.ExtensionDictionary.IsNull){var dic=(DBDictionary)tr.GetObject(p.ExtensionDictionary,OpenMode.ForRead);if(dic.Contains("ROFAMA_DATA")){var x=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_DATA"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a?.Length>1)h=Convert.ToDouble(a[1].Value);}if(dic.Contains("ROFAMA_STOREY")){var x=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_STOREY"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a?.Length>2)z=Convert.ToDouble(a[2].Value);}}
   using var curves=new DBObjectCollection();curves.Add((Polyline)p.Clone());using var regs=Region.CreateFromCurves(curves);if(regs.Count==0)continue;using var reg=(Region)regs[0];var s=new Solid3d();s.SetDatabaseDefaults();s.CreateExtrudedSolid(reg,new Vector3d(0,0,h),new SweepOptions());s.TransformBy(Matrix3d.Displacement(new Vector3d(0,0,z)));s.Layer="RF-3D-PLATIBANDA";ms.AppendEntity(s);tr.AddNewlyCreatedDBObject(s,true);n++;
  }tr.Commit();d.Editor.WriteMessage($"\nPlatibandas 3D geradas: {n}.");}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-3D-PLATIBANDA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-3D-PLATIBANDA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
