using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class IntegratedLoadPathCommands
{
 [CommandMethod("RFCARGASINTEGRADAS")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var beams=new List<(Entity E,Line L,double Slab,double Wall,double Self)>();var cols=new List<(Entity E,Point3d C,double R)>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add((e,l,Read(e,tr,"ROFAMA_BEAM_LOAD")??0,Read(e,tr,"ROFAMA_WALL_LOAD")??0,Read(e,tr,"ROFAMA_SELF_WEIGHT")??0));else if(e.Layer=="RF-EST-PILAR"&&e is Polyline p)cols.Add((e,Mid(p.GeometricExtents),0));}
  double total=0;for(int i=0;i<beams.Count;i++){var b=beams[i];var linear=(b.Wall+b.Self)*b.L.Length;var load=b.Slab+linear;total+=load;Write(b.E,tr,"ROFAMA_BEAM_TOTAL",load);var sup=new List<int>();for(int j=0;j<cols.Count;j++)if(Dist(cols[j].C,b.L.StartPoint,b.L.EndPoint)<=.25)sup.Add(j);if(sup.Count==0)continue;var r=load/sup.Count;foreach(var j in sup){var c=cols[j];cols[j]=(c.E,c.C,c.R+r);}}
  foreach(var c in cols)Write(c.E,tr,"ROFAMA_COLUMN_TOTAL",c.R);tr.Commit();d.Editor.WriteMessage($"\nCargas integradas preliminares processadas: {total:0.00} kN (lajes + paredes + peso próprio de vigas). Sem combinações ELU/ELS.");
 }
 static double? Read(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,string k,double v){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,v),new TypedValue((int)DxfCode.Text,"CARACTERISTICO_PRELIMINAR"));}
 static Point3d Mid(Extents3d e)=>new((e.MinPoint.X+e.MaxPoint.X)/2,(e.MinPoint.Y+e.MaxPoint.Y)/2,0);
 static double Dist(Point3d p,Point3d a,Point3d b){var ab=b-a,ap=p-a;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(a);var t=Math.Max(0,Math.Min(1,ap.DotProduct(ab)/den));return p.DistanceTo(a+ab*t);}
}
