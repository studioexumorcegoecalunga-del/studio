using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class PointLoadAuditCommands
{
 [CommandMethod("RFAUDITARCARGAS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0,bad=0;double worst=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V5")||!dic.Contains("ROFAMA_ANALYTICAL_SPANS")||!dic.Contains("ROFAMA_SPAN_LOADS"))continue;var rr=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V5"),OpenMode.ForRead)).Data?.AsArray();var ss=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();var qq=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_SPAN_LOADS"),OpenMode.ForRead)).Data?.AsArray();if(rr==null||ss==null||qq==null)continue;int nd=Convert.ToInt32(rr[1].Value);var reac=rr.Skip(2+nd).Take(nd).Select(x=>Convert.ToDouble(x.Value)).ToArray();var L=ss.Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();var W=qq.Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();double load=L.Zip(W,(l,w)=>l*w).Sum()+PointSum(dic,tr);double rv=Enumerable.Range(0,nd/2).Sum(i=>reac[2*i]);double diff=Math.Abs(rv-load);worst=Math.Max(worst,diff);if(diff>Math.Max(.01,Math.Abs(load)*.001))bad++;n++;}
  tr.Commit();d.Editor.WriteMessage($"\nAuditoria total V5: {n} viga(s), desequilíbrios={bad}, pior diferença={worst:0.000} kN.");
 }
 static double PointSum(DBDictionary d,Transaction tr){if(!d.Contains("ROFAMA_POINT_LOADS"))return 0;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_POINT_LOADS"),OpenMode.ForRead)).Data?.AsArray();if(a==null)return 0;double s=0;for(int i=0;i+2<a.Length;i+=3)s+=Convert.ToDouble(a[i+2].Value);return s;}
}
