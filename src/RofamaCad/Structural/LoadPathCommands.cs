using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class LoadPathCommands
{
 [CommandMethod("RFDISTRIBUIRCARGAS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var beams=new List<(Entity E,Line L,double Load)>();var cols=new List<(Entity E,Point3d C,double Reaction)>();var slabs=new List<(Polyline P,double G,double Q)>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add((e,l,0));else if(e.Layer=="RF-EST-PILAR"&&e is Polyline p)cols.Add((e,Mid(p.GeometricExtents),0));else if(e.Layer=="RF-EST-LAJE"&&e is Polyline s){var(g,q)=Loads(s,tr);slabs.Add((s,g,q));}}
  double total=0;for(int si=0;si<slabs.Count;si++){var s=slabs[si];var load=(s.G+s.Q)*s.P.Area;total+=load;var near=new List<int>();for(int i=0;i<beams.Count;i++)if(Overlap(s.P.GeometricExtents,beams[i].L.GeometricExtents,.10))near.Add(i);if(near.Count==0)continue;var share=load/near.Count;foreach(var i in near){var b=beams[i];beams[i]=(b.E,b.L,b.Load+share);}}
  for(int i=0;i<beams.Count;i++){var b=beams[i];Write(b.E,tr,"ROFAMA_BEAM_LOAD",b.Load);var supports=new List<int>();for(int j=0;j<cols.Count;j++)if(Dist(cols[j].C,b.L.StartPoint,b.L.EndPoint)<=.25)supports.Add(j);if(supports.Count>0){var r=b.Load/supports.Count;foreach(var j in supports){var c=cols[j];cols[j]=(c.E,c.C,c.Reaction+r);}}}
  foreach(var c in cols)Write(c.E,tr,"ROFAMA_COLUMN_REACTION",c.Reaction);tr.Commit();d.Editor.WriteMessage($"\nCaminho de cargas preliminar processado. Carga característica cadastrada distribuída: {total:0.00} kN. Método simplificado por vigas adjacentes; não é análise estrutural.");
 }
 static(double,double) Loads(Entity e,Transaction tr){if(e.ExtensionDictionary.IsNull)return(0,0);var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_LOAD"))return(0,0);var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_LOAD"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>=2?(Convert.ToDouble(a[0].Value),Convert.ToDouble(a[1].Value)):(0,0);}
 static void Write(Entity e,Transaction tr,string key,double v){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(key))x=(Xrecord)tr.GetObject(d.GetAt(key),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(key,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,v),new TypedValue((int)DxfCode.Text,"SIMPLIFICADO_NAO_DIMENSIONADO"));}
 static Point3d Mid(Extents3d e)=>new((e.MinPoint.X+e.MaxPoint.X)/2,(e.MinPoint.Y+e.MaxPoint.Y)/2,0);
 static bool Overlap(Extents3d a,Extents3d b,double t)=>a.MinPoint.X<=b.MaxPoint.X+t&&a.MaxPoint.X>=b.MinPoint.X-t&&a.MinPoint.Y<=b.MaxPoint.Y+t&&a.MaxPoint.Y>=b.MinPoint.Y-t;
 static double Dist(Point3d p,Point3d a,Point3d b){var ab=b-a;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(a);var t=Math.Max(0,Math.Min(1,(p-a).DotProduct(ab)/den));return p.DistanceTo(a+ab*t);}
}
