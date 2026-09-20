using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MaterialCommands
{
 const string Key="ROFAMA_STRUCT_MATERIAL";
 [CommandMethod("RFMATERIAL")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var f=ed.GetDouble(new PromptDoubleOptions("\nfck do concreto (MPa) <25>: "){DefaultValue=25,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(f.Status!=PromptStatus.OK)return;
  var e=ed.GetDouble(new PromptDoubleOptions("\nMódulo E para análise (MPa) <28000>: "){DefaultValue=28000,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(e.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);Xrecord x;if(nod.Contains(Key))x=(Xrecord)tr.GetObject(nod.GetAt(Key),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt(Key,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,f.Value),new TypedValue((int)DxfCode.Real,e.Value),new TypedValue((int)DxfCode.Text,"INFORMADO_PELO_USUARIO"));tr.Commit();ed.WriteMessage("\nMaterial estrutural salvo no DWG.");
 }
 public static (double Fck,double E)? Read(Database db,Transaction tr){var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);if(!nod.Contains(Key))return null;var a=((Xrecord)tr.GetObject(nod.GetAt(Key),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
}
