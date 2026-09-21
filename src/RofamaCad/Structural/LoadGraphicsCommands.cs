using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class LoadGraphicsCommands
{
 [CommandMethod("RFDESENHARCARGAS")]
 public void Draw(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var source=ms.Cast<ObjectId>().ToArray();int n=0;
  foreach(var id in source){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_POINT_LOADS")||!dic.Contains("ROFAMA_ANALYTICAL_SPANS"))continue;var sp=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data!.AsArray().Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_POINT_LOADS"),OpenMode.ForRead)).Data?.AsArray();if(a==null)continue;var dir=(b.EndPoint-b.StartPoint).GetNormal();var normal=new Vector3d(-dir.Y,dir.X,0);for(int i=0;i+2<a.Length;i+=3){int e=Convert.ToInt32(a[i].Value);double x=Convert.ToDouble(a[i+1].Value),p=Convert.ToDouble(a[i+2].Value),off=sp.Take(e).Sum()+x;var pt=b.StartPoint+dir*off;var top=pt+normal*.5;var ln=new Line(top,pt){Layer="RF-EST-CARGAS"};ms.AppendEntity(ln);tr.AddNewlyCreatedDBObject(ln,true);var tx=new DBText{Position=top+normal*.05,Height=.12,TextString=$"{p:0.##} kN",Layer="RF-EST-CARGAS"};ms.AppendEntity(tx);tr.AddNewlyCreatedDBObject(tx,true);n++;}}
  tr.Commit();d.Editor.WriteMessage($"\nRepresentadas {n} carga(s) pontual(is).");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-CARGAS"))return;lt.UpgradeOpen();var l=new LayerTableRecord{Name="RF-EST-CARGAS"};lt.Add(l);tr.AddNewlyCreatedDBObject(l,true);}
}
