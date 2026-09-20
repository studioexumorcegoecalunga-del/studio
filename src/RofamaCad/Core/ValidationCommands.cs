using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Core;
public sealed class ValidationCommands
{
 [CommandMethod("RFVALIDAR")]
 public void Validate(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;int open=0,noMeta=0,dup=0;var codes=new HashSet<string>();var seen=new HashSet<string>();
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;
   if(e is Polyline p&&e.Layer.StartsWith("RF-ARQ-")&&!p.Closed&&(e.Layer.Contains("PAREDE")||e.Layer.Contains("LAJE")||e.Layer.Contains("PISO")||e.Layer.Contains("COBERTURA")))open++;
   if((e.Layer=="RF-ARQ-PAREDE"||e.Layer=="RF-ARQ-PORTA"||e.Layer=="RF-ARQ-JANELA")&&e.ExtensionDictionary.IsNull)noMeta++;
   if(!e.ExtensionDictionary.IsNull){var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(dic.Contains("ROFAMA_DATA")){var x=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_DATA"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a!=null&&a.Length>4){var c=a[4].Value?.ToString()??"";if(c.StartsWith("P")||c.StartsWith("J")){if(!codes.Add(c))seen.Add(c);}}}}
  }tr.Commit();dup=seen.Count;d.Editor.WriteMessage($"\nRFVALIDAR: contornos abertos={open}; elementos sem metadados={noMeta}; códigos duplicados={dup}.");}
}
