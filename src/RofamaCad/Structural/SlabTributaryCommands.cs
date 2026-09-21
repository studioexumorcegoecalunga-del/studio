using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SlabTributaryCommands
{
 [CommandMethod("RFAREAINFLUENCIA")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var beams=new List<(Entity E,Line L,double Load)>();var slabs=new List<Polyline>();foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add((e,l,0));else if(e.Layer=="RF-EST-LAJE"&&e is Polyline p&&p.Closed)slabs.Add(p);}
  int processed=0;double total=0;
  foreach(var s in slabs){var load=SlabLoad(s,tr);if(load<=0)continue;var ex=s.GeometricExtents;double dx=ex.MaxPoint.X-ex.MinPoint.X; double dy=ex.MaxPoint.Y-ex.MinPoint.Y;bool oneWay=Math.Max(dx,dy)/Math.Max(.001,Math.Min(dx,dy))>=2.0;var candidates=new List<(int I,double W)>();for(int i=0;i<beams.Count;i++){var b=beams[i].L;if(!Overlap(ex,b.GeometricExtents,.12))continue;var vx=Math.Abs(b.EndPoint.X-b.StartPoint.X); var vy=Math.Abs(b.EndPoint.Y-b.StartPoint.Y);double w=oneWay?((dx>=dy&&vx>=vy)||(dy>dx&&vy>vx)?0.25:1.0):1.0;candidates.Add((i,w));}
   if(candidates.Count==0)continue;var area=s.Area;var force=load*area;total+=force;var sw=candidates.Sum(x=>x.W);foreach(var c in candidates){var b=beams[c.I];beams[c.I]=(b.E,b.L,b.Load+force*c.W/sw);}Write(s,tr,"ROFAMA_SLAB_SYSTEM",oneWay?1:2);processed++;}
  foreach(var b in beams)Write(b.E,tr,"ROFAMA_BEAM_TRIBUTARY_LOAD",b.Load);tr.Commit();d.Editor.WriteMessage($"\nÁreas de influência preliminares: {processed} laje(s), {total:0.00} kN distribuídos. Classificação geométrica 1D/2D por razão de lados; exige revisão.");
 }
 static double SlabLoad(Entity e,Transaction tr){if(e.ExtensionDictionary.IsNull)return 0;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_LOAD"))return 0;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_LOAD"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?Convert.ToDouble(a[0].Value)+Convert.ToDouble(a[1].Value):0;}
 static void Write(Entity e,Transaction tr,string k,double v){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,v),new TypedValue((int)DxfCode.Text,"TRIBUTARIO_GEOMETRICO_PRELIMINAR"));}
 static bool Overlap(Extents3d a,Extents3d b,double t)=>a.MinPoint.X<=b.MaxPoint.X+t&&a.MaxPoint.X>=b.MinPoint.X-t&&a.MinPoint.Y<=b.MaxPoint.Y+t&&a.MaxPoint.Y>=b.MinPoint.Y-t;
}
