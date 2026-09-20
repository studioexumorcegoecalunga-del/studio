using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MatrixResultReportCommands
{
 [CommandMethod("RFRESULTADOSMATRIZ")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0;double maxR=0,maxM=0,maxRot=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e||e.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V2"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V2"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<5)continue;int nodes=Convert.ToInt32(a[3].Value);var rot=a.Skip(4).Take(nodes).Select(x=>Convert.ToDouble(x.Value));var rea=a.Skip(4+nodes).Take(nodes).Select(x=>Convert.ToDouble(x.Value));var mom=a.Skip(4+2*nodes).Select(x=>Convert.ToDouble(x.Value));maxRot=Math.Max(maxRot,rot.Select(Math.Abs).DefaultIfEmpty().Max());maxR=Math.Max(maxR,rea.Select(Math.Abs).DefaultIfEmpty().Max());maxM=Math.Max(maxM,mom.Select(Math.Abs).DefaultIfEmpty().Max());n++;}
  tr.Commit();d.Editor.WriteMessage($"\nRESULTADOS MATRICIAIS V2\nVigas: {n}\nMáx |rotação|: {maxRot:0.000000}\nMáx |reação|: {maxR:0.000} kN\nMáx |momento de extremidade|: {maxM:0.000} kN.m\nResultados experimentais; validar antes de dimensionar.");
 }
}
