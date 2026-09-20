using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Documentation;
public sealed class QuantityCommands
{
 [CommandMethod("RFQUANTITATIVO")]
 public void Quantity(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;double wallArea=0,rooms=0,floors=0,slabs=0;int doors=0,windows=0;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;
   if(e.Layer=="RF-ARQ-PORTA")doors++;else if(e.Layer=="RF-ARQ-JANELA")windows++;
   if(e is Polyline p&&p.Closed){if(e.Layer=="RF-ARQ-PAREDE")wallArea+=p.Area;if(e.Layer=="RF-ARQ-PISO")floors+=p.Area;if(e.Layer=="RF-ARQ-LAJE")slabs+=p.Area;}
   if(e is MText mt&&e.Layer=="RF-ARQ-AMBIENTE"){var s=mt.Contents;var ix=s.LastIndexOf('P');if(ix>=0){var n=new string(s[(ix+1)..].TakeWhile(c=>char.IsDigit(c)||c=='.'||c==',').ToArray()).Replace(',','.');if(double.TryParse(n,System.Globalization.NumberStyles.Any,System.Globalization.CultureInfo.InvariantCulture,out var a))rooms+=a;}}
  }tr.Commit();d.Editor.WriteMessage($"\nQUANTITATIVO ROFAMA | área em planta de paredes={wallArea:0.00} m² | ambientes={rooms:0.00} m² | pisos={floors:0.00} m² | lajes={slabs:0.00} m² | portas={doors} | janelas={windows}.");}
}
