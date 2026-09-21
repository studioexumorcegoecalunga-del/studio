using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MatrixBeamV8Commands
{
 [CommandMethod("RFMATRIZVIGA8")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var mat=MaterialCommands.Read(db,tr);if(!mat.HasValue){d.Editor.WriteMessage("\nDefina RFMATERIAL.");return;}var lc=LoadCaseCommands.Read(db,tr);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int ok=0,miss=0,bad=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;try{var s=Spans(b,tr);var q=LoadCaseDataService.ReadSpans(b,tr,lc);var bc=BC(b,tr);if(s==null||q==null||bc==null||s.Length!=q.Length){miss++;continue;}var sec=Pair(b,tr,"ROFAMA_PRESIZE");double bw=sec?.A??.15,h=sec?.B??.40,I=bw*Math.Pow(h,3)/12,E=mat.Value.E*1000;var rel=ReleaseModelService.Flags(s.Length,ReleaseModelService.Parse(b,tr));var r=UnifiedBeamSolver.Solve(s,s.Select(_=>E*I).ToArray(),q,bc.Value.V,bc.Value.R,rel,LoadCaseDataService.ReadPoints(b,tr,lc));Write(b,tr,lc,r);ok++;}catch(InvalidOperationException){bad++;}}
  tr.Commit();d.Editor.WriteMessage($"\nSolver V8 caso {lc}: {ok} analisada(s), {miss} sem cargas completas, {bad} instável(is).");
 }
 static double[]? Spans(Entity e,Transaction tr){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_ANALYTICAL_SPANS"))return null;return ((Xrecord)tr.GetObject(d.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data!.AsArray().Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();}
 static(bool[] V,bool[] R)? BC(Entity e,Transaction tr){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_BOUNDARY"))return null;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_BOUNDARY"),OpenMode.ForRead)).Data?.AsArray();if(a==null)return null;int n=Convert.ToInt32(a[0].Value);var v=new bool[n];var r=new bool[n];for(int i=0;i<n;i++){v[i]=Convert.ToInt32(a[1+2*i].Value)!=0;r[i]=Convert.ToInt32(a[2+2*i].Value)!=0;}return(v,r);}
 static(double A,double B)? Pair(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static void Write(Entity e,Transaction tr,string lc,UnifiedBeamSolver.Result r){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);string k="ROFAMA_MATRIX_CASE_"+new string(lc.Where(c=>char.IsLetterOrDigit(c)||c=='_').ToArray());Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Text,lc),new((int)DxfCode.Int32,r.Displacement.Length)};a.AddRange(r.Displacement.Select(z=>new TypedValue((int)DxfCode.Real,z)));a.AddRange(r.Reaction.Select(z=>new TypedValue((int)DxfCode.Real,z)));a.AddRange(r.EndForce.Select(z=>new TypedValue((int)DxfCode.Real,z)));x.Data=new ResultBuffer(a.ToArray());}
}
