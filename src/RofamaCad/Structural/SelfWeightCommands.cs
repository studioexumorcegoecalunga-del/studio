using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SelfWeightCommands
{
 [CommandMethod("RFPESOPROPRIO")]
 public void Calculate(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;const double gamma=25.0;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e||e.Layer!="RF-EST-VIGA"||e is not Line l)continue;var dims=Dims(e,tr);if(!dims.HasValue)continue;var q=dims.Value.A*dims.Value.B*gamma;Write(e,tr,q);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nPeso próprio preliminar registrado em {n} viga(s), usando peso específico do concreto de {gamma:0} kN/m³ e dimensões cadastradas.");
 }
 static(double A,double B)? Dims(Entity e,Transaction tr){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_STRUCT"))return null;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_STRUCT"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>2?(Convert.ToDouble(a[1].Value),Convert.ToDouble(a[2].Value)):null;}
 static void Write(Entity e,Transaction tr,double q){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains("ROFAMA_SELF_WEIGHT"))x=(Xrecord)tr.GetObject(d.GetAt("ROFAMA_SELF_WEIGHT"),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt("ROFAMA_SELF_WEIGHT",x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,q),new TypedValue((int)DxfCode.Text,"kN/m"));}
}
