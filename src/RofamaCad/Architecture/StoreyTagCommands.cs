using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class StoreyTagCommands
{
 [CommandMethod("RFVINCULARPAVIMENTO")]
 public void Tag(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var psr=ed.GetSelection();if(psr.Status!=PromptStatus.OK)return;int n=0;
  using var tr=db.TransactionManager.StartTransaction();if(StoreyService.GetActive(tr,db)==null){ed.WriteMessage("\nDefina o pavimento ativo com RFPAVIMENTOATIVO.");return;}
  foreach(var id in psr.Value.GetObjectIds()){if(tr.GetObject(id,OpenMode.ForWrite) is Entity e){StoreyService.Tag(e,tr,db);n++;}}tr.Commit();ed.WriteMessage($"\n{n} elemento(s) vinculado(s) ao pavimento ativo.");
 }
}
