using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class LoadCaseCommands
{
 [CommandMethod("RFCASOCARGA")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var p=ed.GetString(new PromptStringOptions("\nNome do caso de carga <PADRAO>: "){AllowSpaces=false});string name=p.Status==PromptStatus.OK&&!string.IsNullOrWhiteSpace(p.StringResult)?p.StringResult.Trim().ToUpperInvariant():"PADRAO";using var tr=d.Database.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(d.Database.NamedObjectsDictionaryId,OpenMode.ForWrite);const string k="ROFAMA_ACTIVE_LOAD_CASE";Xrecord x;if(nod.Contains(k))x=(Xrecord)tr.GetObject(nod.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,name));tr.Commit();ed.WriteMessage($"\nCaso ativo: {name}.");
 }
 public static string Read(Database db,Transaction tr){var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);if(!nod.Contains("ROFAMA_ACTIVE_LOAD_CASE"))return "PADRAO";var a=((Xrecord)tr.GetObject(nod.GetAt("ROFAMA_ACTIVE_LOAD_CASE"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToString(a[0].Value)??"PADRAO":"PADRAO";}
}
