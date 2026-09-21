using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class StructuralIntegrationAuditCommands
{
 [CommandMethod("RFAUDITARESTRUTURAL")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;using var tr=d.Database.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(d.Database),OpenMode.ForRead);int beams=0,noId=0,noSpan=0,noBC=0,noPre=0,noComb=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA")continue;beams++;if(b.ExtensionDictionary.IsNull){noId++;noSpan++;noBC++;noPre++;continue;}var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_STRUCT_ID"))noId++;if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS"))noSpan++;if(!dic.Contains("ROFAMA_BOUNDARY"))noBC++;if(!dic.Contains("ROFAMA_PRESIZE"))noPre++;if(!dic.Cast<DBDictionaryEntry>().Any(x=>x.Key.StartsWith("ROFAMA_COMB_RESULT_")))noComb++;}
  d.Editor.WriteMessage($"\nAUDITORIA ESTRUTURAL: vigas={beams}; sem ID={noId}; sem vãos={noSpan}; sem apoios={noBC}; sem pré-dim={noPre}; sem resultados de combinação={noComb}.");tr.Commit();
 }
}
