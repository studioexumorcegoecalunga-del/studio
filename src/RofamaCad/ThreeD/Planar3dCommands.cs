using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.ThreeD;

public sealed class Planar3dCommands
{
 [CommandMethod("RFGERARLAJES3D")]
 public void Generate()
 {
  var doc=AcApp.DocumentManager.MdiActiveDocument;var db=doc.Database;int count=0;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  EnsureLayer(tr,db,"RF-3D-LAJE");var ids=ms.Cast<ObjectId>().ToArray();
  foreach(var id in ids)
  {
   if(tr.GetObject(id,OpenMode.ForRead) is not Polyline p||!p.Closed||(p.Layer!="RF-ARQ-LAJE"&&p.Layer!="RF-ARQ-PISO"))continue;
   double th=p.Layer=="RF-ARQ-LAJE"?.12:.03;
   if(!p.ExtensionDictionary.IsNull){var d=(DBDictionary)tr.GetObject(p.ExtensionDictionary,OpenMode.ForRead);if(d.Contains("ROFAMA_DATA")){var x=(Xrecord)tr.GetObject(d.GetAt("ROFAMA_DATA"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a!=null&&a.Length>1)th=Convert.ToDouble(a[1].Value);}}
   using var curves=new DBObjectCollection();curves.Add((Polyline)p.Clone());using var regs=Region.CreateFromCurves(curves);if(regs.Count==0)continue;using var reg=(Region)regs[0];
   var solid=new Solid3d();solid.SetDatabaseDefaults();solid.CreateExtrudedSolid(reg,new Vector3d(0,0,th),new SweepOptions());solid.Layer="RF-3D-LAJE";ms.AppendEntity(solid);tr.AddNewlyCreatedDBObject(solid,true);count++;
  }
  tr.Commit();doc.Editor.WriteMessage($"\n{count} piso(s)/laje(s) convertido(s) para 3D.");
 }
 static void EnsureLayer(Transaction tr,Database db,string n){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(n))return;lt.UpgradeOpen();var r=new LayerTableRecord{Name=n};lt.Add(r);tr.AddNewlyCreatedDBObject(r,true);}
}
