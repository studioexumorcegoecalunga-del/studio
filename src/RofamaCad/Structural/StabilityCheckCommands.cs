using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StabilityCheckCommands
{
 [CommandMethod("RFESTABILIDADE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);int n=0,warn=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;n++;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_BOUNDARY")){warn++;continue;}var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_BOUNDARY"),OpenMode.ForRead)).Data?.AsArray();if(a==null){warn++;continue;}int nn=Convert.ToInt32(a[0].Value),fv=0,fr=0;for(int i=0;i<nn;i++){if(Convert.ToInt32(a[1+2*i].Value)!=0)fv++;if(Convert.ToInt32(a[2+2*i].Value)!=0)fr++;}bool hasRel=dic.Contains("ROFAMA_RELEASES");if(fv==0||(fv==1&&fr==0)||hasRel&&fv<2)warn++;}
  tr.Commit();d.Editor.WriteMessage($"\nPré-checagem de estabilidade: {n} viga(s), {warn} configuração(ões) para revisar. É uma triagem topológica; singularidade real é confirmada pelo solver.");
 }
}
