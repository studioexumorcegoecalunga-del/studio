using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class StoreyManagerCommands
{
 const string Key="ROFAMA_STOREYS";
 [CommandMethod("RFPAVIMENTO")]
 public void Add(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var n=ed.GetString(new PromptStringOptions("\nNome do pavimento: "){AllowSpaces=true});if(n.Status!=PromptStatus.OK)return;
  var z=ed.GetDouble(new PromptDoubleOptions("\nElevação em metros: "));if(z.Status!=PromptStatus.OK)return;
  var h=ed.GetDouble(new PromptDoubleOptions("\nPé-direito <2.80>: "){DefaultValue=2.80,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(h.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);DBDictionary dic;
  if(nod.Contains(Key))dic=(DBDictionary)tr.GetObject(nod.GetAt(Key),OpenMode.ForWrite);else{dic=new DBDictionary();nod.SetAt(Key,dic);tr.AddNewlyCreatedDBObject(dic,true);}
  var id=Guid.NewGuid().ToString("N");var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,n.StringResult),new TypedValue((int)DxfCode.Real,z.Value),new TypedValue((int)DxfCode.Real,h.Value))};dic.SetAt(id,x);tr.AddNewlyCreatedDBObject(x,true);tr.Commit();
  ed.WriteMessage($"\nPavimento '{n.StringResult}' criado em {z.Value:0.00} m, pé-direito {h.Value:0.00} m.");
 }
 [CommandMethod("RFLISTARPAVIMENTOS")]
 public void List(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);
  if(!nod.Contains(Key)){d.Editor.WriteMessage("\nNenhum pavimento cadastrado.");return;}var dic=(DBDictionary)tr.GetObject(nod.GetAt(Key),OpenMode.ForRead);
  var list=new List<(string,double,double)>();foreach(DBDictionaryEntry e in dic){var x=(Xrecord)tr.GetObject(e.Value,OpenMode.ForRead);var a=x.Data?.AsArray();if(a?.Length>=3)list.Add((a[0].Value?.ToString()??"",Convert.ToDouble(a[1].Value),Convert.ToDouble(a[2].Value)));}
  foreach(var x in list.OrderBy(x=>x.Item2))d.Editor.WriteMessage($"\n{x.Item1}: nível {x.Item2:0.00} m | pé-direito {x.Item3:0.00} m");tr.Commit();
 }
}
