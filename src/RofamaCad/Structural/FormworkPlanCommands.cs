using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class FormworkPlanCommands
{
 [CommandMethod("RFFORMAS")]
 public void Generate(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);Ensure(tr,db);
  int n=0;foreach(ObjectId id in ms.Cast<ObjectId>().ToArray()){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e||!(e.Layer=="RF-EST-PILAR"||e.Layer=="RF-EST-VIGA"||e.Layer=="RF-EST-LAJE"))continue;var ex=e.GeometricExtents;var p=new Point3d(ex.MinPoint.X,ex.MaxPoint.Y+.12,0);var tx=new DBText{Position=p,Height=.14,TextString=Code(e,tr)??e.Layer.Replace("RF-EST-",""),Layer="RF-EST-FORMAS"};ms.AppendEntity(tx);tr.AddNewlyCreatedDBObject(tx,true);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nPlanta de formas preliminar anotada: {n} elemento(s).");
 }
 static string? Code(Entity e,Transaction tr){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_STRUCT_ID"))return null;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_STRUCT_ID"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?a[0].Value?.ToString():null;}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-FORMAS"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-EST-FORMAS"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
