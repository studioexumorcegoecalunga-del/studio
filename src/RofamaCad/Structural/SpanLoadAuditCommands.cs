using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SpanLoadAuditCommands
{
 [CommandMethod("RFAUDITARVAOS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int beams=0,ok=0,missing=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA")continue;beams++;if(b.ExtensionDictionary.IsNull){missing++;continue;}var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS")||!dic.Contains("ROFAMA_SPAN_LOADS")){missing++;continue;}var s=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();var q=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_SPAN_LOADS"),OpenMode.ForRead)).Data?.AsArray();if(s!=null&&q!=null&&s.Length==q.Length)ok++;else missing++;}
  tr.Commit();d.Editor.WriteMessage($"\nAuditoria de vãos/cargas: vigas={beams}; consistentes={ok}; incompletas/incompatíveis={missing}.");
 }
}
