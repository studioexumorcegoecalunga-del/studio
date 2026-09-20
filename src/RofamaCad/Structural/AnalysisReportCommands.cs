using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class AnalysisReportCommands
{
 [CommandMethod("RFANALISERELATORIO")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0;double mm=0,vv=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e||e.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_BEAM_ANALYSIS"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_BEAM_ANALYSIS"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<3)continue;mm=Math.Max(mm,Math.Abs(Convert.ToDouble(a[1].Value)));vv=Math.Max(vv,Math.Abs(Convert.ToDouble(a[2].Value)));n++;}
  tr.Commit();d.Editor.WriteMessage($"\nANÁLISE PRELIMINAR\nVigas analisadas: {n}\nMaior |M|: {mm:0.00} kN.m\nMaior |V|: {vv:0.00} kN\nResultados do modelo biapoiado simplificado, não dimensionamento.");
 }
}
