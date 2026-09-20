using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class LoadAuditCommands
{
 [CommandMethod("RFEQUILIBRIO")]
 public void Audit(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);
  double input=0,beams=0,cols=0;int unloadedSlabs=0,unsupportedBeams=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;
   if(e.Layer=="RF-EST-LAJE"&&e is Polyline p){var q=Pair(e,tr,"ROFAMA_LOAD");if(q.HasValue)input+=(q.Value.A+q.Value.B)*p.Area;else unloadedSlabs++;}
   else if(e.Layer=="RF-EST-VIGA"){var x=Single(e,tr,"ROFAMA_BEAM_LOAD");if(x.HasValue)beams+=x.Value;else unsupportedBeams++;}
   else if(e.Layer=="RF-EST-PILAR"){var x=Single(e,tr,"ROFAMA_COLUMN_REACTION");if(x.HasValue)cols+=x.Value;}
  }tr.Commit();var dbal=input==0?0:Math.Abs(input-cols)/input*100;d.Editor.WriteMessage($"\nAUDITORIA DO CAMINHO DE CARGAS\nEntrada nas lajes: {input:0.00} kN\nTransferido às vigas: {beams:0.00} kN\nAcumulado nos pilares: {cols:0.00} kN\nDiferença entrada/pilares: {dbal:0.00}%\nLajes sem carga: {unloadedSlabs}\nVigas sem carga registrada: {unsupportedBeams}\nAuditoria geométrica preliminar.");
 }
 static(double A,double B)? Pair(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static double? Single(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
}
