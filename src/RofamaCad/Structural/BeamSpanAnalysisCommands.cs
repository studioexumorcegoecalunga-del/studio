using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class BeamSpanAnalysisCommands
{
 [CommandMethod("RFANALISARVAOS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var cols=new List<Point3d>();var beams=new List<Line>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add(new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add(l);}
  int spans=0;double maxM=0,maxV=0;foreach(var b in beams){var total=Read(b,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY")??0;if(total<=0||b.Length<=0)continue;var w=total/b.Length;var xs=cols.Where(c=>Dist(c,b)<=.25).Select(c=>Along(c,b)).Where(x=>x>=-.01&&x<=b.Length+.01).Append(0).Append(b.Length).DistinctBy(x=>Math.Round(x,3)).OrderBy(x=>x).ToList();double bm=0,bv=0;for(int i=1;i<xs.Count;i++){var L=xs[i]-xs[i-1];if(L<.05)continue;var m=w*L*L/8.0;var v=w*L/2.0;bm=Math.Max(bm,m);bv=Math.Max(bv,v);maxM=Math.Max(maxM,m);maxV=Math.Max(maxV,v);spans++;}Write(b,tr,bm,bv,xs.Count-1);}
  tr.Commit();d.Editor.WriteMessage($"\nAnálise por vãos concluída: {spans} vão(s). Máx. M={maxM:0.00} kN.m; Máx. V={maxV:0.00} kN. Cada vão ainda é tratado como biapoiado independente.");
 }
 static double? Read(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,double m,double v,int n){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_SPAN_ANALYSIS";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Int16,n),new TypedValue((int)DxfCode.Real,m),new TypedValue((int)DxfCode.Real,v),new TypedValue((int)DxfCode.Text,"VAOS_BIAPOIADOS_INDEPENDENTES"));}
 static double Along(Point3d p,Line l)=>(p-l.StartPoint).DotProduct((l.EndPoint-l.StartPoint).GetNormal());
 static double Dist(Point3d p,Line l){var ab=l.EndPoint-l.StartPoint;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(l.StartPoint);var t=Math.Max(0,Math.Min(1,(p-l.StartPoint).DotProduct(ab)/den));return p.DistanceTo(l.StartPoint+ab*t);}
}
