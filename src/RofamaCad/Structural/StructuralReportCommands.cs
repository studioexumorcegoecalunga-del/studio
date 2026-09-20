using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralReportCommands
{
 [CommandMethod("RFESTRELATORIO")]
 public void Report(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int p=0,v=0,l=0,f=0;double vlen=0,la=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR")p++;else if(e.Layer=="RF-EST-VIGA"){v++;if(e is Line x)vlen+=x.Length;}else if(e.Layer=="RF-EST-LAJE"){l++;if(e is Polyline x&&x.Closed)la+=x.Area;}else if(e.Layer=="RF-EST-FUNDACAO")f++;}
  tr.Commit();d.Editor.WriteMessage($"\nRESUMO ESTRUTURAL PRELIMINAR\nPilares: {p}\nVigas: {v} | comprimento de eixos: {vlen:0.00} m\nLajes: {l} | área geométrica: {la:0.00} m²\nFundações lançadas: {f}\nSem dimensionamento/armaduras.");
 }
}
