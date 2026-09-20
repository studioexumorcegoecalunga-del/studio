using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.ThreeD;
public sealed class HipRoof3dCommands
{
 [CommandMethod("RFTELHADO4AGUAS3D")]
 public void Build(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var o=new PromptEntityOptions("\nSelecione contorno retangular fechado: ");o.SetRejectMessage("\nSelecione uma polilinha.");o.AddAllowedClass(typeof(Polyline),true);var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  var s=ed.GetDouble(new PromptDoubleOptions("\nInclinação em % <30>: "){DefaultValue=30,UseDefaultValue=true,AllowNegative=false});if(s.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var p=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);if(!p.Closed){ed.WriteMessage("\nContorno precisa estar fechado.");return;}Ensure(tr,db);
  var e=p.GeometricExtents;double dx=e.MaxPoint.X-e.MinPoint.X,dy=e.MaxPoint.Y-e.MinPoint.Y,z=p.Elevation;var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  if(dx>=dy){double half=dy/2,rise=half*s.Value/100,x1=e.MinPoint.X+half,x2=e.MaxPoint.X-half,ym=(e.MinPoint.Y+e.MaxPoint.Y)/2;var r1=new Point3d(x1,ym,z+rise);var r2=new Point3d(x2,ym,z+rise);
   Face(ms,tr,new Point3d(e.MinPoint.X,e.MinPoint.Y,z),new Point3d(e.MaxPoint.X,e.MinPoint.Y,z),r2,r1);Face(ms,tr,r1,r2,new Point3d(e.MaxPoint.X,e.MaxPoint.Y,z),new Point3d(e.MinPoint.X,e.MaxPoint.Y,z));Face(ms,tr,new Point3d(e.MinPoint.X,e.MinPoint.Y,z),r1,new Point3d(e.MinPoint.X,e.MaxPoint.Y,z),new Point3d(e.MinPoint.X,e.MaxPoint.Y,z));Face(ms,tr,r2,new Point3d(e.MaxPoint.X,e.MinPoint.Y,z),new Point3d(e.MaxPoint.X,e.MaxPoint.Y,z),new Point3d(e.MaxPoint.X,e.MaxPoint.Y,z));}
  else{double half=dx/2,rise=half*s.Value/100,y1=e.MinPoint.Y+half,y2=e.MaxPoint.Y-half,xm=(e.MinPoint.X+e.MaxPoint.X)/2;var r1=new Point3d(xm,y1,z+rise);var r2=new Point3d(xm,y2,z+rise);
   Face(ms,tr,new Point3d(e.MinPoint.X,e.MinPoint.Y,z),r1,r2,new Point3d(e.MinPoint.X,e.MaxPoint.Y,z));Face(ms,tr,r1,new Point3d(e.MaxPoint.X,e.MinPoint.Y,z),new Point3d(e.MaxPoint.X,e.MaxPoint.Y,z),r2);Face(ms,tr,new Point3d(e.MinPoint.X,e.MinPoint.Y,z),new Point3d(e.MaxPoint.X,e.MinPoint.Y,z),r1,r1);Face(ms,tr,r2,new Point3d(e.MaxPoint.X,e.MaxPoint.Y,z),new Point3d(e.MinPoint.X,e.MaxPoint.Y,z),r2);}
  tr.Commit();ed.WriteMessage("\nTelhado 3D de quatro águas gerado para contorno retangular.");}
 static void Face(BlockTableRecord ms,Transaction tr,Point3d a,Point3d b,Point3d c,Point3d d){var f=new Face(a,b,c,d,true,true,true,true){Layer="RF-3D-COBERTURA"};ms.AppendEntity(f);tr.AddNewlyCreatedDBObject(f,true);}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-3D-COBERTURA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-3D-COBERTURA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
