using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class LoadPathReportCommands
{
 [CommandMethod("RFCARGASRELATORIO")]
 public void Report(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);double beam=0,col=0;int nb=0,nc=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-VIGA"){var v=Read(e,tr,"ROFAMA_BEAM_LOAD");if(v.HasValue){beam+=v.Value;nb++;}}else if(e.Layer=="RF-EST-PILAR"){var v=Read(e,tr,"ROFAMA_COLUMN_REACTION");if(v.HasValue){col+=v.Value;nc++;}}}
  tr.Commit();d.Editor.WriteMessage($"\nCARGAS PRELIMINARES\nVigas com carga distribuída: {nb} | soma: {beam:0.00} kN\nPilares com reação acumulada: {nc} | soma: {col:0.00} kN\nValores característicos simplificados, sem combinações ELU/ELS.");
 }
 static double? Read(Entity e,Transaction tr,string key){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(key))return null;var a=((Xrecord)tr.GetObject(d.GetAt(key),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
}
