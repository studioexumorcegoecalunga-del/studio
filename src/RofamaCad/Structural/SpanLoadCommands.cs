using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SpanLoadCommands
{
 [CommandMethod("RFCARGASVAOS")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var pe=ed.GetEntity("\nSelecione a viga: ");if(pe.Status!=PromptStatus.OK)return;using var tr=db.TransactionManager.StartTransaction();if(tr.GetObject(pe.ObjectId,OpenMode.ForRead) is not Line b||b.ExtensionDictionary.IsNull){ed.WriteMessage("\nObjeto não é uma viga válida.");return;}var spans=ReadSpans(b,tr);if(spans==null){ed.WriteMessage("\nExecute RFGRAVARVAOS primeiro.");return;}var loads=new List<double>();for(int i=0;i<spans.Length;i++){var po=new PromptDoubleOptions($"\nCarga distribuída do vão {i+1} (kN/m) <0>: "){DefaultValue=0,UseDefaultValue=true,AllowNegative=false};var pr=ed.GetDouble(po);if(pr.Status!=PromptStatus.OK)return;loads.Add(pr.Value);}Write(b,tr,loads);tr.Commit();ed.WriteMessage($"\nCargas por vão gravadas: {loads.Count}.");
 }
 static double[]? ReadSpans(Entity e,Transaction tr){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_ANALYTICAL_SPANS"))return null;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();return a==null?null:a.Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();}
 static void Write(Entity e,Transaction tr,List<double> q){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_SPAN_LOADS";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Int16,q.Count)};a.AddRange(q.Select(v=>new TypedValue((int)DxfCode.Real,v)));x.Data=new ResultBuffer(a.ToArray());}
}
