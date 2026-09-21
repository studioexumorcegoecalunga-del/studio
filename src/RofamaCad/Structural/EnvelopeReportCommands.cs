using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class EnvelopeReportCommands
{
 [CommandMethod("RFRELATORIOENVELOPE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;using var tr=d.Database.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(d.Database),OpenMode.ForRead);int k=0;d.Editor.WriteMessage("\n--- ENVELOPES ROFAMA ---");
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_COMB_ENVELOPE"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_COMB_ENVELOPE"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<7)continue;string name=StructuralIdService.Get(b,tr,$"V{++k:00}");double[] v=a.Take(6).Select(x=>Convert.ToDouble(x.Value)).ToArray();d.Editor.WriteMessage($"\n{name}: v=[{v[0]*1000:0.###},{v[1]*1000:0.###}] mm | R=[{v[2]:0.###},{v[3]:0.###}] kN | Mext=[{v[4]:0.###},{v[5]:0.###}] kN.m | comb={Convert.ToInt32(Convert.ToDouble(a[6].Value))}");}tr.Commit();
 }
}
