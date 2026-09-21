using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class ReleaseCommands
{
 [CommandMethod("RFLIBERACAOVIGA")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var pe=ed.GetEntity("\nSelecione a viga: ");if(pe.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();if(tr.GetObject(pe.ObjectId,OpenMode.ForRead) is not Line b||b.ExtensionDictionary.IsNull)return;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS")){ed.WriteMessage("\nExecute RFGRAVARVAOS.");return;}var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();int ne=(a?.Length??1)-1;if(ne<1)return;var pi=ed.GetInteger(new PromptIntegerOptions($"\nBarra/vão [1-{ne}]: "){LowerLimit=1,UpperLimit=ne});if(pi.Status!=PromptStatus.OK)return;var pk=ed.GetKeywords(new PromptKeywordOptions("\nLiberar momento em [Inicio/Fim/Ambos]: ","Inicio Fim Ambos"));if(pk.Status!=PromptStatus.OK)return;Append(b,tr,pi.Value-1,pk.StringResult!="Fim",pk.StringResult!="Inicio");tr.Commit();ed.WriteMessage("\nLiberação registrada. O solver com condensação será usado na próxima análise.");
 }
 static void Append(Entity e,Transaction tr,int el,bool ini,bool fim){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_RELEASES";var vals=new List<TypedValue>();Xrecord x;if(d.Contains(k)){x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);var a=x.Data?.AsArray();if(a!=null)vals.AddRange(a);}else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}vals.Add(new((int)DxfCode.Int16,el));vals.Add(new((int)DxfCode.Int16,ini?1:0));vals.Add(new((int)DxfCode.Int16,fim?1:0));x.Data=new ResultBuffer(vals.ToArray());}
}
