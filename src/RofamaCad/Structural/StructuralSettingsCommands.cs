using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralSettingsCommands
{
 const string Key="ROFAMA_STRUCT_SETTINGS";
 [CommandMethod("RFESTCONFIG")]
 public void Configure(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var fc=ed.GetDouble(new PromptDoubleOptions("\nfck do concreto (MPa) <25>: "){DefaultValue=25,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(fc.Status!=PromptStatus.OK)return;
  var cover=ed.GetDouble(new PromptDoubleOptions("\nCobrimento nominal preliminar (cm) <3.0>: "){DefaultValue=3,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(cover.Status!=PromptStatus.OK)return;
  var live=ed.GetDouble(new PromptDoubleOptions("\nCarga variável de referência (kN/m²) <2.0>: "){DefaultValue=2,UseDefaultValue=true,AllowNegative=false});if(live.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);Xrecord x;if(nod.Contains(Key))x=(Xrecord)tr.GetObject(nod.GetAt(Key),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt(Key,x);tr.AddNewlyCreatedDBObject(x,true);}
  x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,fc.Value),new TypedValue((int)DxfCode.Real,cover.Value),new TypedValue((int)DxfCode.Real,live.Value),new TypedValue((int)DxfCode.Text,"PARAMETROS_PRELIMINARES"));tr.Commit();
  ed.WriteMessage("\nParâmetros estruturais preliminares salvos. Não constituem dimensionamento normativo.");
 }
}
