using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class PreSizingCommands
{
 [CommandMethod("RFPREDIMENSIONAR")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int v=0,p=0,l=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;
   if(e.Layer=="RF-EST-VIGA"&&e is Line b){var h=Math.Max(.30,b.Length/10.0);var bw=.15;Write(e,tr,"ROFAMA_PRESIZE",bw,h,"VIGA_HEURISTICA_L10");v++;}
   else if(e.Layer=="RF-EST-PILAR"&&e is Polyline c){var r=Read(e,tr,"ROFAMA_COLUMN_TRIBUTARY_REACTION")??0;var side=r<=200?.19:r<=400?.25:.30;Write(e,tr,"ROFAMA_PRESIZE",side,side,"PILAR_FAIXA_REACAO_PRELIMINAR");p++;}
   else if(e.Layer=="RF-EST-LAJE"&&e is Polyline s){var ex=s.GeometricExtents;var a=ex.MaxPoint.X-ex.MinPoint.X;var b=ex.MaxPoint.Y-ex.MinPoint.Y;var span=Math.Min(a,b);var th=Math.Max(.10,span/40.0);Write(e,tr,"ROFAMA_PRESIZE",th,0,"LAJE_HEURISTICA_L40");l++;}
  }tr.Commit();d.Editor.WriteMessage($"\nPré-dimensionamento heurístico registrado: {v} viga(s), {p} pilar(es), {l} laje(s). Valores NÃO constituem dimensionamento normativo.");
 }
 static double? Read(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,string k,double a,double b,string method){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,a),new TypedValue((int)DxfCode.Real,b),new TypedValue((int)DxfCode.Text,method),new TypedValue((int)DxfCode.Text,"NAO_NORMATIVO_REVISAR"));}
}
