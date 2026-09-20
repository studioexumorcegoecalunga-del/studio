using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class AnalyticalSpanCommands
{
 [CommandMethod("RFGRAVARVAOS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var cols=new List<Point3d>();var beams=new List<Line>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add(new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add(l);}
  int n=0;foreach(var b in beams){var xs=cols.Where(c=>Dist(c,b)<=.25).Select(c=>Along(c,b)).Where(x=>x>-.01&&x<b.Length+.01).Append(0).Append(b.Length).DistinctBy(x=>Math.Round(x,3)).OrderBy(x=>x).ToList();var spans=new List<double>();for(int i=1;i<xs.Count;i++)if(xs[i]-xs[i-1]>.05)spans.Add(xs[i]-xs[i-1]);if(spans.Count==0)continue;Write(b,tr,spans);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nComprimentos reais de vãos gravados em {n} viga(s).");
 }
 static void Write(Entity e,Transaction tr,List<double> s){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_ANALYTICAL_SPANS";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Int16,s.Count)};a.AddRange(s.Select(v=>new TypedValue((int)DxfCode.Real,v)));x.Data=new ResultBuffer(a.ToArray());}
 static double Along(Point3d p,Line l)=>(p-l.StartPoint).DotProduct((l.EndPoint-l.StartPoint).GetNormal());
 static double Dist(Point3d p,Line l){var ab=l.EndPoint-l.StartPoint;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(l.StartPoint);var t=Math.Max(0,Math.Min(1,(p-l.StartPoint).DotProduct(ab)/den));return p.DistanceTo(l.StartPoint+ab*t);}
}
