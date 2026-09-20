using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class BeamDiagramCommands
{
 [CommandMethod("RFDIAGRAMAVIGA")]
 public void Draw(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0;
  foreach(ObjectId id in ms.Cast<ObjectId>().ToArray()){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_SPAN_ANALYSIS"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_SPAN_ANALYSIS"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<3)continue;var m=Convert.ToDouble(a[1].Value);var mid=new Point3d((b.StartPoint.X+b.EndPoint.X)/2,(b.StartPoint.Y+b.EndPoint.Y)/2,0);var t=new DBText{Position=mid,Height=.14,TextString=$"M~{m:0.0} kN.m",Layer="RF-EST-DIAGRAMA"};ms.AppendEntity(t);tr.AddNewlyCreatedDBObject(t,true);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nDiagramas preliminares anotados em {n} viga(s). Nesta fase é uma anotação do M máximo, não diagrama contínuo de esforços.");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-DIAGRAMA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-EST-DIAGRAMA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
