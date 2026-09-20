using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SupportCheckCommands
{
 [CommandMethod("RFVERIFICARAPOIOS")]
 public void Check(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);var cols=new List<Point3d>();var beams=new List<Line>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add(new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add(l);}
  int unsupported=0,one=0;foreach(var b in beams){int n=0;foreach(var c in cols)if(Dist(c,b.StartPoint,b.EndPoint)<=.25)n++;if(n==0)unsupported++;else if(n==1)one++;}
  tr.Commit();d.Editor.WriteMessage($"\nVERIFICAÇÃO GEOMÉTRICA DE APOIOS\nVigas: {beams.Count}\nSem pilar associado: {unsupported}\nCom apenas um pilar associado: {one}\nA associação é geométrica e deve ser revisada pelo projetista.");
 }
 static double Dist(Point3d p,Point3d a,Point3d b){var ab=b-a;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(a);var t=Math.Max(0,Math.Min(1,(p-a).DotProduct(ab)/den));return p.DistanceTo(a+ab*t);}
}
