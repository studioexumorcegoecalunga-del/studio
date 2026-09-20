using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class BoundaryConditionCommands
{
 [CommandMethod("RFAPOIOSVIGA")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var pe=ed.GetEntity("\nSelecione a viga: ");if(pe.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();if(tr.GetObject(pe.ObjectId,OpenMode.ForRead) is not Line b||b.ExtensionDictionary.IsNull){ed.WriteMessage("\nViga inválida.");return;}var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS")){ed.WriteMessage("\nExecute RFGRAVARVAOS.");return;}var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();int nn=(a?.Length??1);var v=new List<bool>();var r=new List<bool>();for(int i=0;i<nn;i++){var k=ed.GetKeywords(new PromptKeywordOptions($"\nNó {i+1} [Simples/Engaste/Livre] <Simples>: ","Simples Engaste Livre"){AllowNone=true});var s=k.Status==PromptStatus.OK?k.StringResult:"Simples";v.Add(s!="Livre");r.Add(s=="Engaste");}Write(b,tr,v,r);tr.Commit();ed.WriteMessage("\nCondições de contorno gravadas.");
 }
 static void Write(Entity e,Transaction tr,List<bool> v,List<bool> r){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_BOUNDARY";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Int16,v.Count)};for(int i=0;i<v.Count;i++){a.Add(new((int)DxfCode.Int16,v[i]?1:0));a.Add(new((int)DxfCode.Int16,r[i]?1:0));}x.Data=new ResultBuffer(a.ToArray());}
}
