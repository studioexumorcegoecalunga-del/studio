using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class LoadCaseDataCommands
{
 [CommandMethod("RFCARGASVAOSCASO")]
 public void Spans(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var pe=ed.GetEntity("\nSelecione a viga: ");if(pe.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();if(tr.GetObject(pe.ObjectId,OpenMode.ForRead) is not Line b||b.ExtensionDictionary.IsNull)return;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS"))return;int n=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data!.AsArray().Length-1;var q=new List<double>();for(int i=0;i<n;i++){var p=ed.GetDouble(new PromptDoubleOptions($"\nCaso {LoadCaseCommands.Read(d.Database,tr)} - q vão {i+1} (kN/m) <0>: "){DefaultValue=0,UseDefaultValue=true,AllowNegative=false});if(p.Status!=PromptStatus.OK)return;q.Add(p.Value);}var lc=LoadCaseCommands.Read(d.Database,tr);LoadCaseDataService.WriteSpans(b,tr,lc,q);tr.Commit();ed.WriteMessage($"\nCargas distribuídas gravadas no caso {lc}.");
 }
 [CommandMethod("RFCARGAPONTUALCASO")]
 public void Point(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var pe=ed.GetEntity("\nSelecione a viga: ");if(pe.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();if(tr.GetObject(pe.ObjectId,OpenMode.ForRead) is not Line b||b.ExtensionDictionary.IsNull)return;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS"))return;var s=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data!.AsArray().Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();var pi=ed.GetInteger(new PromptIntegerOptions($"\nVão [1-{s.Length}]: "){LowerLimit=1,UpperLimit=s.Length});if(pi.Status!=PromptStatus.OK)return;int e=pi.Value-1;var px=ed.GetDouble(new PromptDoubleOptions($"\nPosição no vão (0 a {s[e]:0.###} m): "){AllowNegative=false});if(px.Status!=PromptStatus.OK||px.Value>s[e]){ed.WriteMessage("\nPosição fora do vão.");return;}var pp=ed.GetDouble(new PromptDoubleOptions("\nCarga P (kN): "){AllowNegative=false,AllowZero=false});if(pp.Status!=PromptStatus.OK)return;var lc=LoadCaseCommands.Read(d.Database,tr);var pts=LoadCaseDataService.ReadPoints(b,tr,lc);pts.Add(new(e,px.Value,pp.Value));LoadCaseDataService.WritePoints(b,tr,lc,pts);tr.Commit();ed.WriteMessage($"\nCarga pontual gravada no caso {lc}.");
 }
}
