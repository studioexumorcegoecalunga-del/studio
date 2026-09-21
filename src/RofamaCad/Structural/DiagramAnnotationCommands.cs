using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class DiagramAnnotationCommands
{
 [CommandMethod("RFANOTARDIAGRAMAS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var cfg=DiagramSettingsCommands.Read(db,tr);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var ids=ms.Cast<ObjectId>().ToArray();int n=0;
  foreach(var id in ids){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V7")||!dic.Contains("ROFAMA_ANALYTICAL_SPANS")||!dic.Contains("ROFAMA_SPAN_LOADS"))continue;var s=Vals(dic,tr,"ROFAMA_ANALYTICAL_SPANS");var q=Vals(dic,tr,"ROFAMA_SPAN_LOADS");var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V7"),OpenMode.ForRead)).Data!.AsArray();int nd=Convert.ToInt32(a[1].Value);var u=a.Skip(2).Take(nd).Select(x=>Convert.ToDouble(x.Value)).ToArray();var ef=a.Skip(2+2*nd).Select(x=>Convert.ToDouble(x.Value)).ToArray();var sec=Pair(dic,tr,"ROFAMA_PRESIZE");var mat=MaterialCommands.Read(db,tr);if(!mat.HasValue)continue;double bw=sec?.A??.15,h=sec?.B??.40,EI=mat.Value.E*1000*bw*Math.Pow(h,3)/12;var sm=BeamPostProcessor.Sample(s,s.Select(_=>EI).ToArray(),q,u,ef,Points(dic,tr));Add(ms,tr,b,sm.OrderByDescending(x=>Math.Abs(x.V)).First(),$"V={sm.OrderByDescending(x=>Math.Abs(x.V)).First().V:0.##} kN",cfg.VScale,cfg.TextHeight);Add(ms,tr,b,sm.OrderByDescending(x=>Math.Abs(x.M)).First(),$"M={sm.OrderByDescending(x=>Math.Abs(x.M)).First().M:0.##} kN.m",cfg.MScale,cfg.TextHeight);Add(ms,tr,b,sm.OrderByDescending(x=>Math.Abs(x.Deflection)).First(),$"v={sm.OrderByDescending(x=>Math.Abs(x.Deflection)).First().Deflection*1000:0.##} mm",cfg.DScale,cfg.TextHeight);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nAnotações de máximos inseridas em {n} viga(s).");
 }
 static void Add(BlockTableRecord ms,Transaction tr,Line b,BeamPostProcessor.SamplePoint s,string txt,double scale,double h){var dir=(b.EndPoint-b.StartPoint).GetNormal();var normal=new Vector3d(-dir.Y,dir.X,0);var p=b.StartPoint+dir*s.GlobalX+normal*.25;var t=new DBText{Position=p,Height=h,TextString=txt,Layer="RF-EST-RESULTADOS"};ms.AppendEntity(t);tr.AddNewlyCreatedDBObject(t,true);}
 static double[] Vals(DBDictionary d,Transaction tr,string k)=>((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data!.AsArray().Skip(1).Select(x=>Convert.ToDouble(x.Value)).ToArray();
 static(double A,double B)? Pair(DBDictionary d,Transaction tr,string k){if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static List<BeamPostProcessor.PointLoad> Points(DBDictionary d,Transaction tr){var z=new List<BeamPostProcessor.PointLoad>();if(!d.Contains("ROFAMA_POINT_LOADS"))return z;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_POINT_LOADS"),OpenMode.ForRead)).Data?.AsArray();if(a!=null)for(int i=0;i+2<a.Length;i+=3)z.Add(new(Convert.ToInt32(a[i].Value),Convert.ToDouble(a[i+1].Value),Convert.ToDouble(a[i+2].Value)));return z;}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-RESULTADOS"))return;lt.UpgradeOpen();var l=new LayerTableRecord{Name="RF-EST-RESULTADOS"};lt.Add(l);tr.AddNewlyCreatedDBObject(l,true);}
}
