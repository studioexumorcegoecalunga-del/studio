using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class DiagramSettingsCommands
{
 public sealed record Settings(double VScale,double MScale,double DScale,double TextHeight);
 [CommandMethod("RFCONFIGDIAGRAMAS")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;double Ask(string msg,double def){var p=ed.GetDouble(new PromptDoubleOptions(msg){DefaultValue=def,UseDefaultValue=true,AllowNegative=false,AllowZero=false});return p.Status==PromptStatus.OK?p.Value:def;}
  var s=new Settings(Ask("\nEscala cortante <0.15>: ",.15),Ask("\nEscala momento <0.10>: ",.10),Ask("\nAmplificação deformada <50>: ",50),Ask("\nAltura do texto <0.12>: ",.12));using var tr=d.Database.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(d.Database.NamedObjectsDictionaryId,OpenMode.ForWrite);const string k="ROFAMA_DIAGRAM_SETTINGS";Xrecord x;if(nod.Contains(k))x=(Xrecord)tr.GetObject(nod.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,s.VScale),new TypedValue((int)DxfCode.Real,s.MScale),new TypedValue((int)DxfCode.Real,s.DScale),new TypedValue((int)DxfCode.Real,s.TextHeight));tr.Commit();ed.WriteMessage("\nConfiguração dos diagramas gravada.");
 }
 public static Settings Read(Database db,Transaction tr){var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);if(!nod.Contains("ROFAMA_DIAGRAM_SETTINGS"))return new(.15,.10,50,.12);var a=((Xrecord)tr.GetObject(nod.GetAt("ROFAMA_DIAGRAM_SETTINGS"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=4?new(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value),Convert.ToDouble(a[2].Value),Convert.ToDouble(a[3].Value)):new(.15,.10,50,.12);}
}
