using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralContinuityCommands
{
 [CommandMethod("RFCONTINUIDADE")]
 public void Check(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForRead);var beams=new List<Line>();
  foreach(ObjectId id in ms)if(tr.GetObject(id,OpenMode.ForRead) is Line l&&l.Layer=="RF-EST-VIGA")beams.Add(l);
  int connected=0,isolated=0;for(int i=0;i<beams.Count;i++){bool hit=false;for(int j=0;j<beams.Count;j++){if(i==j)continue;if(Near(beams[i].StartPoint,beams[j],.15)||Near(beams[i].EndPoint,beams[j],.15)){hit=true;break;}}if(hit)connected++;else isolated++;}
  tr.Commit();d.Editor.WriteMessage($"\nCONTINUIDADE GEOMÉTRICA\nVigas conectadas a outra viga: {connected}\nVigas isoladas: {isolated}\nResultado geométrico; não representa continuidade estrutural de cálculo.");
 }
 static bool Near(Point3d p,Line l,double t){var ab=l.EndPoint-l.StartPoint;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(l.StartPoint)<=t;var u=Math.Max(0,Math.Min(1,(p-l.StartPoint).DotProduct(ab)/den));return p.DistanceTo(l.StartPoint+ab*u)<=t;}
}
