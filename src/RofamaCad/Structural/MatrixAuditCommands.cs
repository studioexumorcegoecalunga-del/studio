using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MatrixAuditCommands
{
 [CommandMethod("RFAUDITARMATRIZ")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0,bad=0;double worst=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V2")||!dic.Contains("ROFAMA_ANALYTICAL_SPANS"))continue;var ra=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V2"),OpenMode.ForRead)).Data?.AsArray();var sa=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();if(ra==null||sa==null)continue;int nodes=Convert.ToInt32(ra[3].Value);var reactions=ra.Skip(4+nodes).Take(nodes).Select(x=>Convert.ToDouble(x.Value)).ToArray();var spans=sa.Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();var total=Read(b,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY")??0;var diff=Math.Abs(reactions.Sum()-total);worst=Math.Max(worst,diff);if(diff>Math.Max(.01,Math.Abs(total)*.001))bad++;n++;}
  tr.Commit();d.Editor.WriteMessage($"\nAuditoria matricial: {n} viga(s); desequilíbrios: {bad}; pior diferença absoluta={worst:0.000} kN. Tolerância: máx(0,01 kN; 0,1%).");
 }
 static double? Read(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
}
