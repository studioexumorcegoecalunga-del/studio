using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public static class SolverComparisonSettingsCommands
{
 public sealed record Settings(double U,double R,double E);
 public static Settings Read(Database db,Transaction tr){var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);if(!nod.Contains("ROFAMA_SOLVER_COMPARE_SETTINGS"))return new(1e-6,1e-4,1e-4);var a=((Xrecord)tr.GetObject(nod.GetAt("ROFAMA_SOLVER_COMPARE_SETTINGS"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=3?new(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value),Convert.ToDouble(a[2].Value)):new(1e-6,1e-4,1e-4);}
 [CommandMethod("RFCONFIGCOMPARARSOLVER")]
 public static void Configure(){var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;using var tr=d.Database.TransactionManager.StartTransaction();var cur=Read(d.Database,tr);double u=Ask(ed,"Tolerância deslocamento",cur.U),r=Ask(ed,"Tolerância reação",cur.R),e=Ask(ed,"Tolerância esforço",cur.E);var nod=(DBDictionary)tr.GetObject(d.Database.NamedObjectsDictionaryId,OpenMode.ForWrite);Xrecord x;if(nod.Contains("ROFAMA_SOLVER_COMPARE_SETTINGS"))x=(Xrecord)tr.GetObject(nod.GetAt("ROFAMA_SOLVER_COMPARE_SETTINGS"),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt("ROFAMA_SOLVER_COMPARE_SETTINGS",x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,u),new TypedValue((int)DxfCode.Real,r),new TypedValue((int)DxfCode.Real,e));tr.Commit();}
 static double Ask(Editor ed,string msg,double def){var o=new PromptDoubleOptions($"\n{msg} <{def:G}>: "){AllowNone=true,AllowNegative=false,AllowZero=true};var p=ed.GetDouble(o);return p.Status==PromptStatus.OK?p.Value:def;}
}
