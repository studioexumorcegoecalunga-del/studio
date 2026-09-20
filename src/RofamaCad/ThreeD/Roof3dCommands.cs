using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.ThreeD;
public sealed class Roof3dCommands
{
 [CommandMethod("RFCOBERTURA3D")]
 public void Roof3d(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;int count=0;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);Ensure(tr,db,"RF-3D-COBERTURA");var ids=ms.Cast<ObjectId>().ToArray();
  foreach(var id in ids){if(tr.GetObject(id,OpenMode.ForRead) is not Polyline p||p.Layer!="RF-ARQ-COBERTURA"||!p.Closed)continue;
   double slope=30; if(!p.ExtensionDictionary.IsNull){var dic=(DBDictionary)tr.GetObject(p.ExtensionDictionary,OpenMode.ForRead);if(dic.Contains("ROFAMA_DATA")){var x=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_DATA"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a!=null&&a.Length>1)slope=Convert.ToDouble(a[1].Value);}}
   var ex=p.GeometricExtents;double span=Math.Min(ex.MaxPoint.X-ex.MinPoint.X,ex.MaxPoint.Y-ex.MinPoint.Y);double rise=span*.5*slope/100.0;
   using var curves=new DBObjectCollection();curves.Add((Polyline)p.Clone());using var regs=Region.CreateFromCurves(curves);if(regs.Count==0)continue;using var reg=(Region)regs[0];
   var s=new Solid3d();s.SetDatabaseDefaults();s.CreateExtrudedSolid(reg,new Vector3d(0,0,Math.Max(.08,rise)),new SweepOptions());s.Layer="RF-3D-COBERTURA";ms.AppendEntity(s);tr.AddNewlyCreatedDBObject(s,true);count++;
  }tr.Commit();d.Editor.WriteMessage($"\nCobertura 3D preliminar: {count} sólido(s). A geometria de águas será refinada por tipo de telhado.");}
 static void Ensure(Transaction tr,Database db,string n){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(n))return;lt.UpgradeOpen();var r=new LayerTableRecord{Name=n};lt.Add(r);tr.AddNewlyCreatedDBObject(r,true);}
}
