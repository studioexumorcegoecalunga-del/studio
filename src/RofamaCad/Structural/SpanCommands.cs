using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SpanCommands
{
 [CommandMethod("RFVAOS")]
 public void List(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);var cols=new List<Point3d>();var beams=new List<Line>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add(new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add(l);}
  int spans=0;double max=0;foreach(var b in beams){var pts=cols.Where(c=>Dist(c,b)<=.25).Select(c=>Along(c,b)).Where(x=>x>=0&&x<=b.Length).OrderBy(x=>x).ToList();for(int i=1;i<pts.Count;i++){var v=pts[i]-pts[i-1];if(v>.05){spans++;max=Math.Max(max,v);}}}
  tr.Commit();d.Editor.WriteMessage($"\nVÃOS GEOMÉTRICOS\nVãos entre pilares detectados: {spans}\nMaior vão: {max:0.00} m\nSem classificação de apoio ou dimensionamento.");
 }
 static double Along(Point3d p,Line l){var dir=(l.EndPoint-l.StartPoint).GetNormal();return (p-l.StartPoint).DotProduct(dir);}
 static double Dist(Point3d p,Line l){var ab=l.EndPoint-l.StartPoint;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(l.StartPoint);var t=Math.Max(0,Math.Min(1,(p-l.StartPoint).DotProduct(ab)/den));return p.DistanceTo(l.StartPoint+ab*t);}
}
