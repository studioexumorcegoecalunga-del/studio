using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class WallLoadCommands
{
 [CommandMethod("RFCARGAPAREDE")]
 public void Add(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var o=new PromptEntityOptions("\nSelecione o eixo/viga que recebe a parede: ");o.AddAllowedClass(typeof(Line),true);o.SetRejectMessage("\nSelecione uma linha de viga.");var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  var q=ed.GetDouble(new PromptDoubleOptions("\nCarga linear característica da parede (kN/m) <3.0>: "){DefaultValue=3,UseDefaultValue=true,AllowNegative=false});if(q.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var e=(Entity)tr.GetObject(r.ObjectId,OpenMode.ForWrite);if(e.Layer!="RF-EST-VIGA"){ed.WriteMessage("\nO objeto não é uma viga estrutural.");return;}if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(dic.Contains("ROFAMA_WALL_LOAD"))x=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_WALL_LOAD"),OpenMode.ForWrite);else{x=new Xrecord();dic.SetAt("ROFAMA_WALL_LOAD",x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,q.Value),new TypedValue((int)DxfCode.Text,"CARACTERISTICA_INFORMADA"));tr.Commit();ed.WriteMessage("\nCarga linear de parede registrada separadamente da carga de laje.");
 }
}
