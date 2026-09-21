using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class BeamDiagramV7Commands
{
 [CommandMethod("RFDIAGRAMASV7")]
 public void Draw(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db,"RF-EST-DIAGRAMA-V");Ensure(tr,db,"RF-EST-DIAGRAMA-M");Ensure(tr,db,"RF-EST-DEFORMADA");var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var ids=ms.Cast<ObjectId>().ToArray();int n=0;
  foreach(var id in ids){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V7")||!dic.Contains("ROFAMA_ANALYTICAL_SPANS")||!dic.Contains("ROFAMA_SPAN_LOADS"))continue;var s=Vals(dic,tr,"ROFAMA_ANALYTICAL_SPANS",true);var q=Vals(dic,tr,"ROFAMA_SPAN_LOADS",true);var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V7"),OpenMode.ForRead)).Data!.AsArray();int nd=Convert.ToInt32(a[1].Value);var u=a.Skip(2).Take(nd).Select(x=>Convert.ToDouble(x.Value)).ToArray();var ef=a.Skip(2+2*nd).Select(x=>Convert.ToDouble(x.Value)).ToArray();var sec=Pair(dic,tr,"ROFAMA_PRESIZE");double bw=sec?.A??.15,h=sec?.B??.40;var mat=MaterialCommands.Read(db,tr);if(!mat.HasValue)continue;double EI=mat.Value.E*1000*bw*Math.Pow(h,3)/12;var pts=Points(dic,tr);var sm=BeamPostProcessor.Sample(s,s.Select(_=>EI).ToArray(),q,u,ef,pts);DrawSet(ms,tr,b,sm.Select(x=>(x.GlobalX,x.V)).ToList(),"RF-EST-DIAGRAMA-V",.15);DrawSet(ms,tr,b,sm.Select(x=>(x.GlobalX,x.M)).ToList(),"RF-EST-DIAGRAMA-M",.10);DrawSet(ms,tr,b,sm.Select(x=>(x.GlobalX,x.Deflection)).ToList(),"RF-EST-DEFORMADA",50);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nDiagramas V7 gerados para {n} viga(s): cortante, momento e deformada.");
 }
 static void DrawSet(BlockTableRecord ms,Transaction tr,Line b,List<(double x,double y)> p,string layer,double scale){if(p.Count<2)return;var dir=(b.EndPoint-b.StartPoint).GetNormal();var normal=new Vector3d(-dir.Y,dir.X,0);var pl=new Polyline();for(int i=0;i<p.Count;i++){var pt=b.StartPoint+dir*p[i].x+normal*(p[i].y*scale);pl.AddVertexAt(i,new Point2d(pt.X,pt.Y),0,0,0);}pl.Layer=layer;ms.AppendEntity(pl);tr.AddNewlyCreatedDBObject(pl,true);}
 static double[] Vals(DBDictionary d,Transaction tr,string k,bool skip){var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data!.AsArray();return a.Skip(skip?1:0).Select(x=>Convert.ToDouble(x.Value)).ToArray();}
 static(double A,double B)? Pair(DBDictionary d,Transaction tr,string k){if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):null;}
 static List<BeamPostProcessor.PointLoad> Points(DBDictionary d,Transaction tr){var z=new List<BeamPostProcessor.PointLoad>();if(!d.Contains("ROFAMA_POINT_LOADS"))return z;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_POINT_LOADS"),OpenMode.ForRead)).Data?.AsArray();if(a!=null)for(int i=0;i+2<a.Length;i+=3)z.Add(new(Convert.ToInt32(a[i].Value),Convert.ToDouble(a[i+1].Value),Convert.ToDouble(a[i+2].Value)));return z;}
 static void Ensure(Transaction tr,Database db,string name){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(name))return;lt.UpgradeOpen();var l=new LayerTableRecord{Name=name};lt.Add(l);tr.AddNewlyCreatedDBObject(l,true);}
}
