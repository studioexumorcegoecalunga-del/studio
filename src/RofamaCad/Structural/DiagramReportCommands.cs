using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class DiagramReportCommands
{
 [CommandMethod("RFRELATORIOV7")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0;double maxR=0,maxM=0,maxD=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V7"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V7"),OpenMode.ForRead)).Data?.AsArray();if(a==null)continue;int nd=Convert.ToInt32(a[1].Value);var u=a.Skip(2).Take(nd).Select(x=>Math.Abs(Convert.ToDouble(x.Value))).ToArray();var r=a.Skip(2+nd).Take(nd).Select(x=>Math.Abs(Convert.ToDouble(x.Value))).ToArray();var ef=a.Skip(2+2*nd).Select(x=>Math.Abs(Convert.ToDouble(x.Value))).ToArray();maxD=Math.Max(maxD,u.Where((_,i)=>i%2==0).DefaultIfEmpty().Max());maxR=Math.Max(maxR,r.Where((_,i)=>i%2==0).DefaultIfEmpty().Max());maxM=Math.Max(maxM,ef.Where((_,i)=>i%2==1).DefaultIfEmpty().Max());n++;}
  tr.Commit();d.Editor.WriteMessage($"\nRelatório V7: vigas={n}; |R|max={maxR:0.###} kN; |M extremo|max={maxM:0.###} kN.m; |v nodal|max={maxD:0.######} m.");
 }
}
