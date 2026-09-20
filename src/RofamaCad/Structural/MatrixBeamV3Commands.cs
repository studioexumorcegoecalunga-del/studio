using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MatrixBeamV3Commands
{
 [CommandMethod("RFMATRIZVIGA3")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var mat=MaterialCommands.Read(db,tr);if(!mat.HasValue){d.Editor.WriteMessage("\nDefina RFMATERIAL.");return;}var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0,skip=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var spans=Arr(b,tr,"ROFAMA_ANALYTICAL_SPANS",true);var loads=Arr(b,tr,"ROFAMA_SPAN_LOADS",true);if(spans==null||loads==null||spans.Length!=loads.Length){skip++;continue;}var sec=Pair(b,tr,"ROFAMA_PRESIZE");double bw=sec?.A??.15,h=sec?.B??.40,I=bw*Math.Pow(h,3)/12.0,E=mat.Value.E*1000.0;var r=BeamStiffnessSolver.Solve(spans,spans.Select(_=>E*I).ToArray(),loads);Write(b,tr,r);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nSolver V3: {n} viga(s) analisadas com carga individual por vão; {skip} ignorada(s) por dados incompletos.");
 }
 static double[]? Arr(Entity e,Transaction tr,string k,bool skipCount){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a==null?null:a.Skip(skipCount?1:0).Select(x=>Convert.ToDouble(x.Value)).ToArray();}
 static(double A,double B)? Pair(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static void Write(Entity e,Transaction tr,BeamStiffnessSolver.Result r){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_MATRIX_ANALYSIS_V3";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Text,"CARGA_POR_VAO"),new((int)DxfCode.Int16,r.Rotation.Length)};a.AddRange(r.Rotation.Select(v=>new TypedValue((int)DxfCode.Real,v)));a.AddRange(r.Reaction.Select(v=>new TypedValue((int)DxfCode.Real,v)));a.AddRange(r.EndMoment.Select(v=>new TypedValue((int)DxfCode.Real,v)));x.Data=new ResultBuffer(a.ToArray());}
}
