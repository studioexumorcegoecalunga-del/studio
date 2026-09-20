using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralGraphCommands
{
 [CommandMethod("RFESTGRAFO")]
 public void Build(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);
  var cols=new List<(ObjectId Id,Point3d C)>();var beams=new List<(ObjectId Id,Line L)>();var slabs=new List<(ObjectId Id,Extents3d E)>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add((id,Mid(x)));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add((id,l));else if(e.Layer=="RF-EST-LAJE"&&e is Polyline s)slabs.Add((id,s.GeometricExtents));}
  int bc=0,sb=0;foreach(var b in beams)foreach(var c in cols)if(DistToSegment(c.C,b.L.StartPoint,b.L.EndPoint)<=.25)bc++;foreach(var s in slabs)foreach(var b in beams)if(Overlap(s.E,b.L.GeometricExtents,.10))sb++;
  tr.Commit();d.Editor.WriteMessage($"\nGrafo estrutural: {slabs.Count} laje(s), {beams.Count} viga(s), {cols.Count} pilar(es), {sb} relação(ões) laje-viga e {bc} relação(ões) viga-pilar detectadas.");
 }
 static Point3d Mid(Extents3d e)=>new((e.MinPoint.X+e.MaxPoint.X)/2,(e.MinPoint.Y+e.MaxPoint.Y)/2,0);
 static bool Overlap(Extents3d a,Extents3d b,double t)=>a.MinPoint.X<=b.MaxPoint.X+t&&a.MaxPoint.X>=b.MinPoint.X-t&&a.MinPoint.Y<=b.MaxPoint.Y+t&&a.MaxPoint.Y>=b.MinPoint.Y-t;
 static double DistToSegment(Point3d p,Point3d a,Point3d b){var ab=b-a,ap=p-a;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(a);var t=Math.Max(0,Math.Min(1,ap.DotProduct(ab)/den));return p.DistanceTo(a+ab*t);}
}
