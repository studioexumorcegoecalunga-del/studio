using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class AnalyticalModelCommands
{
 [CommandMethod("RFMODELOANALITICO")]
 public void Build(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var nodes=new List<Point3d>();var beams=new List<Line>();foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;nodes.Add(new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add(l);}
  int bars=0;foreach(var b in beams){var xs=nodes.Where(n=>Dist(n,b)<=.25).Select(n=>Along(n,b)).Where(x=>x>=-.01&&x<=b.Length+.01).Append(0).Append(b.Length).DistinctBy(x=>Math.Round(x,3)).OrderBy(x=>x).ToList();var ids=new List<int>();foreach(var x in xs){var p=b.StartPoint+(b.EndPoint-b.StartPoint).GetNormal()*x;ids.Add(Node(nodes,p));}Write(b,tr,ids);bars+=Math.Max(0,ids.Count-1);}
  tr.Commit();d.Editor.WriteMessage($"\nModelo analítico criado: {nodes.Count} nó(s), {bars} barra(s) lógicas. Topologia persistida nas vigas.");
 }
 static int Node(List<Point3d> ns,Point3d p){for(int i=0;i<ns.Count;i++)if(ns[i].DistanceTo(p)<.02)return i+1;ns.Add(p);return ns.Count;}
 static void Write(Entity e,Transaction tr,List<int> ids){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_ANALYTICAL_NODES";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}var vals=ids.Select(i=>new TypedValue((int)DxfCode.Int32,i)).ToArray();x.Data=new ResultBuffer(vals);}
 static double Along(Point3d p,Line l)=>(p-l.StartPoint).DotProduct((l.EndPoint-l.StartPoint).GetNormal());
 static double Dist(Point3d p,Line l){var ab=l.EndPoint-l.StartPoint;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(l.StartPoint);var t=Math.Max(0,Math.Min(1,(p-l.StartPoint).DotProduct(ab)/den));return p.DistanceTo(l.StartPoint+ab*t);}
}
