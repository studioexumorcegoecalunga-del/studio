using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class PointLoadCommands
{
 [CommandMethod("RFCARGAPONTUAL")]
 public void Set(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var pe=ed.GetEntity("\nSelecione a viga: ");if(pe.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();if(tr.GetObject(pe.ObjectId,OpenMode.ForRead) is not Line b||b.ExtensionDictionary.IsNull)return;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_SPANS")){ed.WriteMessage("\nExecute RFGRAVARVAOS.");return;}var s=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data!.AsArray().Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();var pi=ed.GetInteger(new PromptIntegerOptions($"\nNúmero do vão [1-{s.Length}]: "){LowerLimit=1,UpperLimit=s.Length});if(pi.Status!=PromptStatus.OK)return;int e=pi.Value-1;var px=ed.GetDouble(new PromptDoubleOptions($"\nDistância desde o início do vão (m), 0 a {s[e]:0.###}: "){AllowNegative=false});if(px.Status!=PromptStatus.OK||px.Value>s[e]){ed.WriteMessage("\nPosição fora do vão.");return;}var pp=ed.GetDouble(new PromptDoubleOptions("\nCarga pontual vertical para baixo (kN): "){AllowNegative=false,AllowZero=false});if(pp.Status!=PromptStatus.OK)return;Append(b,tr,e,px.Value,pp.Value);tr.Commit();ed.WriteMessage("\nCarga pontual adicionada.");
 }
 static void Append(Entity e,Transaction tr,int el,double x,double p){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_POINT_LOADS";var vals=new List<TypedValue>();Xrecord xr;if(d.Contains(k)){xr=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);var a=xr.Data?.AsArray();if(a!=null)vals.AddRange(a);}else{xr=new Xrecord();d.SetAt(k,xr);tr.AddNewlyCreatedDBObject(xr,true);}vals.Add(new((int)DxfCode.Int16,el));vals.Add(new((int)DxfCode.Real,x));vals.Add(new((int)DxfCode.Real,p));xr.Data=new ResultBuffer(vals.ToArray());}
}
