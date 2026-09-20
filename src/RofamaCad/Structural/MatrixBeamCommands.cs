using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class MatrixBeamCommands
{
 [CommandMethod("RFMATRIZVIGA")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int done=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_ANALYTICAL_NODES"))continue;var na=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_ANALYTICAL_NODES"),OpenMode.ForRead)).Data?.AsArray();if(na==null||na.Length<2)continue;int spans=na.Length-1;double L=b.Length/spans;var pre=Pair(b,tr,"ROFAMA_PRESIZE");double bw=pre?.A??.15,h=pre?.B??.40;double I=bw*Math.Pow(h,3)/12.0,E=25e6;double total=Read(b,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY")??0,w=b.Length>0?total/b.Length:0;var ls=Enumerable.Repeat(L,spans).ToArray();var eis=Enumerable.Repeat(E*I,spans).ToArray();var ws=Enumerable.Repeat(w,spans).ToArray();var r=BeamStiffnessSolver.Solve(ls,eis,ws);Write(b,tr,r);done++;}
  tr.Commit();d.Editor.WriteMessage($"\nMatriz de rigidez experimental executada em {done} viga(s). E é provisório e os vãos ainda são uniformizados; resultados exigem validação independente.");
 }
 static(double A,double B)? Pair(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static double? Read(Entity e,Transaction tr,string k){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,BeamStiffnessSolver.Result r){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_MATRIX_ANALYSIS";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var vals=new List<TypedValue>{new((int)DxfCode.Text,"MATRIZ_ROTACIONAL_EXPERIMENTAL"),new((int)DxfCode.Int16,r.Rotation.Length)};vals.AddRange(r.Rotation.Select(v=>new TypedValue((int)DxfCode.Real,v)));vals.AddRange(r.Reaction.Select(v=>new TypedValue((int)DxfCode.Real,v)));vals.AddRange(r.EndMoment.Select(v=>new TypedValue((int)DxfCode.Real,v)));x.Data=new ResultBuffer(vals.ToArray());}
}
