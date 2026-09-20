using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Core;
public sealed class ActiveStoreyCommands
{
 const string Storeys="ROFAMA_STOREYS",Active="ROFAMA_ACTIVE_STOREY";
 [CommandMethod("RFPAVIMENTOATIVO")]
 public void SetActive(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);
  if(!nod.Contains(Storeys)){ed.WriteMessage("\nCadastre um pavimento com RFPAVIMENTO.");return;}
  var sd=(DBDictionary)tr.GetObject(nod.GetAt(Storeys),OpenMode.ForRead);var map=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
  foreach(DBDictionaryEntry e in sd){var x=(Xrecord)tr.GetObject(e.Value,OpenMode.ForRead);var a=x.Data?.AsArray();if(a?.Length>=3)map[a[0].Value?.ToString()??e.Key]=e.Key;}
  ed.WriteMessage("\nPavimentos: "+string.Join(", ",map.Keys));
  var r=ed.GetString(new PromptStringOptions("\nDigite o nome exato do pavimento ativo: "){AllowSpaces=true});if(r.Status!=PromptStatus.OK)return;
  if(!map.TryGetValue(r.StringResult,out var id)){ed.WriteMessage("\nPavimento não encontrado.");return;}
  Xrecord rec;if(nod.Contains(Active))rec=(Xrecord)tr.GetObject(nod.GetAt(Active),OpenMode.ForWrite);else{rec=new Xrecord();nod.SetAt(Active,rec);tr.AddNewlyCreatedDBObject(rec,true);}
  rec.Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,id),new TypedValue((int)DxfCode.Text,r.StringResult));tr.Commit();ed.WriteMessage($"\nPavimento ativo: {r.StringResult}.");
 }
 [CommandMethod("RFMOSTRARPAVIMENTOATIVO")]
 public void Show(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);
  if(!nod.Contains(Active)){d.Editor.WriteMessage("\nNenhum pavimento ativo.");return;}var x=(Xrecord)tr.GetObject(nod.GetAt(Active),OpenMode.ForRead);var a=x.Data?.AsArray();d.Editor.WriteMessage($"\nPavimento ativo: {(a?.Length>1?a[1].Value:"não definido")}.");tr.Commit();
 }
}
