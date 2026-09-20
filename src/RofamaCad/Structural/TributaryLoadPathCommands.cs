using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class TributaryLoadPathCommands
{
 [CommandMethod("RFCAMINHOTRIBUTARIO")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var cols=new List<(Entity E,Point3d C,double R)>();var beams=new List<(Entity E,Line L)>();
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;if(e.Layer=="RF-EST-PILAR"&&e is Polyline p){var x=p.GeometricExtents;cols.Add((e,new((x.MinPoint.X+x.MaxPoint.X)/2,(x.MinPoint.Y+x.MaxPoint.Y)/2,0),0));}else if(e.Layer=="RF-EST-VIGA"&&e is Line l)beams.Add((e,l));}
  double total=0;foreach(var b in beams){double slab=Read(b.E,tr,"ROFAMA_BEAM_TRIBUTARY_LOAD")??0,wall=(Read(b.E,tr,"ROFAMA_WALL_LOAD")??0)*b.L.Length,self=(Read(b.E,tr,"ROFAMA_SELF_WEIGHT")??0)*b.L.Length,load=slab+wall+self;total+=load;Write(b.E,tr,"ROFAMA_BEAM_TOTAL_TRIBUTARY",load);var ids=Enumerable.Range(0,cols.Count).Where(i=>Dist(cols[i].C,b.L)<=.25).ToList();if(ids.Count==0)continue;var r=load/ids.Count;foreach(var i in ids){var c=cols[i];cols[i]=(c.E,c.C,c.R+r);}}
  foreach(var c in cols)Write(c.E,tr,"ROFAMA_COLUMN_TRIBUTARY_REACTION",c.R);tr.Commit();d.Editor.WriteMessage($"\nCaminho tributário processado: {total:0.00} kN. Lajes por área de influência preliminar + paredes + peso próprio de vigas.");
 }
 static double? Read(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;var a=((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToDouble(a[0].Value):null;}
 static void Write(Entity e,Transaction tr,string k,double v){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,v),new TypedValue((int)DxfCode.Text,"TRIBUTARIO_PRELIMINAR"));}
 static double Dist(Point3d p,Line l){var ab=l.EndPoint-l.StartPoint;var den=ab.DotProduct(ab);if(den<1e-12)return p.DistanceTo(l.StartPoint);var t=Math.Max(0,Math.Min(1,(p-l.StartPoint).DotProduct(ab)/den));return p.DistanceTo(l.StartPoint+ab*t);}
}
