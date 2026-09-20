using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class PreSizingReportCommands
{
 [CommandMethod("RFPREDIMRELATORIO")]
 public void Report(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0;
  d.Editor.WriteMessage("\nPRÉ-DIMENSIONAMENTO HEURÍSTICO");
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e||e.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_PRESIZE"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_PRESIZE"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<3)continue;d.Editor.WriteMessage($"\n{e.Layer}: {Convert.ToDouble(a[0].Value):0.000} / {Convert.ToDouble(a[1].Value):0.000} m | {a[2].Value}");n++;}
  tr.Commit();d.Editor.WriteMessage($"\nElementos: {n}. Conferir por cálculo estrutural e critérios normativos aplicáveis.");
 }
}
