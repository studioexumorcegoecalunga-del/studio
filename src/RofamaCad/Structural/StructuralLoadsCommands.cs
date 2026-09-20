using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralLoadsCommands
{
 [CommandMethod("RFCARGALAJE")]
 public void SlabLoad(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var o=new PromptEntityOptions("\nSelecione a laje estrutural: ");o.AddAllowedClass(typeof(Polyline),true);o.SetRejectMessage("\nSelecione polilinha.");var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  var g=ed.GetDouble(new PromptDoubleOptions("\nCarga permanente adicional gk (kN/m²) <1.0>: "){DefaultValue=1,UseDefaultValue=true,AllowNegative=false});if(g.Status!=PromptStatus.OK)return;
  var q=ed.GetDouble(new PromptDoubleOptions("\nCarga variável qk (kN/m²) <2.0>: "){DefaultValue=2,UseDefaultValue=true,AllowNegative=false});if(q.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var e=(Entity)tr.GetObject(r.ObjectId,OpenMode.ForWrite);if(e.Layer!="RF-EST-LAJE"){ed.WriteMessage("\nObjeto não pertence à layer de laje estrutural.");return;}if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,g.Value),new TypedValue((int)DxfCode.Real,q.Value),new TypedValue((int)DxfCode.Text,"CARREGAMENTO_CARACTERISTICO"))};if(dic.Contains("ROFAMA_LOAD")){var old=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_LOAD"),OpenMode.ForWrite);old.Data=x.Data;x.Dispose();}else{dic.SetAt("ROFAMA_LOAD",x);tr.AddNewlyCreatedDBObject(x,true);}tr.Commit();ed.WriteMessage("\nCarregamento característico registrado; combinações e dimensionamento ainda não executados.");
 }
}
