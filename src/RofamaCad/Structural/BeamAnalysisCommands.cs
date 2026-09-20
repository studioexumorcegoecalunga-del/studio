using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class BeamAnalysisCommands
{
 [CommandMethod("RFVIGAANALISE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA")continue;var f=Read(b,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY");if(!f.HasValue||b.Length<=0)continue;var w=f.Value/b.Length;var m=w*b.Length*b.Length/8.0;var v=w*b.Length/2.0;Write(b,tr,m,v,w);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nAnálise estática preliminar executada em {n} viga(s): modelo biapoiado com carga uniformemente distribuída. Não considera continuidade, combinações normativas ou rigidez.");
 }
 static double? Read(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,double m,double v,double w){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_BEAM_ANALYSIS";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,w),new TypedValue((int)DxfCode.Real,m),new TypedValue((int)DxfCode.Real,v),new TypedValue((int)DxfCode.Text,"BIAPOIADA_UDL_PRELIMINAR"));}
}
