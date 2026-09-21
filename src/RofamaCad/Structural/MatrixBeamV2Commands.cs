using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MatrixBeamV2Commands
{
 [CommandMethod("RFMATRIZVIGA2")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var mat=MaterialCommands.Read(db,tr);if(!mat.HasValue){d.Editor.WriteMessage("\nDefina o material primeiro com RFMATERIAL.");return;}var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var spans=Spans(b,tr);if(spans==null||spans.Length==0)continue;var sec=Pair(b,tr,"ROFAMA_PRESIZE");double bw=sec?.A??.15; double h=sec?.B??.40; double I=bw*Math.Pow(h,3)/12.0; double E=mat.Value.E*1000.0;var total=Read(b,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY")??0; var w=b.Length>0?total/b.Length:0;var ei=spans.Select(_=>E*I).ToArray();var ws=spans.Select(_=>w).ToArray();var r=BeamStiffnessSolver.Solve(spans,ei,ws);Write(b,tr,r,mat.Value.Fck,mat.Value.E);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nSolver matricial V2 executado em {n} viga(s), usando vãos reais e E informado no projeto. Ainda requer benchmarks antes de dimensionamento.");
 }
 static double[]? Spans(Entity e,Transaction tr){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_ANALYTICAL_SPANS"))return null;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_ANALYTICAL_SPANS"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<2)return null;return a.Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();}
 static(double A,double B)? Pair(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static double? Read(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,BeamStiffnessSolver.Result r,double fck,double emp){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_MATRIX_ANALYSIS_V2";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var v=new List<TypedValue>{new((int)DxfCode.Text,"VAOS_REAIS_E_INFORMADO"),new((int)DxfCode.Real,fck),new((int)DxfCode.Real,emp),new((int)DxfCode.Int16,r.Rotation.Length)};v.AddRange(r.Rotation.Select(z=>new TypedValue((int)DxfCode.Real,z)));v.AddRange(r.Reaction.Select(z=>new TypedValue((int)DxfCode.Real,z)));v.AddRange(r.EndMoment.Select(z=>new TypedValue((int)DxfCode.Real,z)));x.Data=new ResultBuffer(v.ToArray());}
}
