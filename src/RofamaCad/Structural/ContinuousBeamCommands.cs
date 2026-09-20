using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class ContinuousBeamCommands
{
 [CommandMethod("RFVIGACONTINUA")]
 public void Analyze(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_NODES"))continue;var ids=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_NODES"),OpenMode.ForRead)).Data?.AsArray();if(ids==null||ids.Length<2)continue;var load=Read(b,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY")??0;var spans=ids.Length-1;var L=b.Length/spans;var w=b.Length>0?load/b.Length:0;var mpos=w*L*L/8.0;var mneg=spans>1?-w*L*L/12.0:0;Write(b,tr,mpos,mneg,w,spans);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nModelo contínuo experimental preparado em {n} viga(s). Momentos internos usam aproximações heurísticas; a matriz global de rigidez ainda será implementada.");
 }
 static double? Read(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,double mp,double mn,double w,int spans){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_CONTINUOUS_BEAM";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Int16,spans),new TypedValue((int)DxfCode.Real,w),new TypedValue((int)DxfCode.Real,mp),new TypedValue((int)DxfCode.Real,mn),new TypedValue((int)DxfCode.Text,"EXPERIMENTAL_HEURISTICO"));}
}
