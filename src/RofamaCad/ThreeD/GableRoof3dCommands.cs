using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.ThreeD;
public sealed class GableRoof3dCommands
{
 [CommandMethod("RFTELHADO2AGUAS3D")]
 public void Build(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var o=new PromptEntityOptions("\nSelecione contorno retangular fechado do telhado: ");o.SetRejectMessage("\nSelecione uma polilinha.");o.AddAllowedClass(typeof(Polyline),true);var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  var s=ed.GetDouble(new PromptDoubleOptions("\nInclinação em % <30>: "){DefaultValue=30,UseDefaultValue=true,AllowNegative=false});if(s.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var p=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);if(!p.Closed){ed.WriteMessage("\nContorno precisa estar fechado.");return;}
  var ex=p.GeometricExtents;double dx=ex.MaxPoint.X-ex.MinPoint.X,dy=ex.MaxPoint.Y-ex.MinPoint.Y;double z=p.Elevation;var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);Ensure(tr,db);
  if(dx>=dy){double ym=(ex.MinPoint.Y+ex.MaxPoint.Y)/2,rise=dy*.5*s.Value/100.0;AddFace(ms,tr,new Point3d(ex.MinPoint.X,ex.MinPoint.Y,z),new Point3d(ex.MaxPoint.X,ex.MinPoint.Y,z),new Point3d(ex.MaxPoint.X,ym,z+rise),new Point3d(ex.MinPoint.X,ym,z+rise));AddFace(ms,tr,new Point3d(ex.MinPoint.X,ym,z+rise),new Point3d(ex.MaxPoint.X,ym,z+rise),new Point3d(ex.MaxPoint.X,ex.MaxPoint.Y,z),new Point3d(ex.MinPoint.X,ex.MaxPoint.Y,z));}
  else{double xm=(ex.MinPoint.X+ex.MaxPoint.X)/2,rise=dx*.5*s.Value/100.0;AddFace(ms,tr,new Point3d(ex.MinPoint.X,ex.MinPoint.Y,z),new Point3d(xm,ex.MinPoint.Y,z+rise),new Point3d(xm,ex.MaxPoint.Y,z+rise),new Point3d(ex.MinPoint.X,ex.MaxPoint.Y,z));AddFace(ms,tr,new Point3d(xm,ex.MinPoint.Y,z+rise),new Point3d(ex.MaxPoint.X,ex.MinPoint.Y,z),new Point3d(ex.MaxPoint.X,ex.MaxPoint.Y,z),new Point3d(xm,ex.MaxPoint.Y,z+rise));}
  tr.Commit();ed.WriteMessage("\nTelhado 3D de duas águas gerado com planos inclinados.");
 }
 static void AddFace(BlockTableRecord ms,Transaction tr,Point3d a,Point3d b,Point3d c,Point3d d){var f=new Face(a,b,c,d,true,true,true,true){Layer="RF-3D-COBERTURA"};ms.AppendEntity(f);tr.AddNewlyCreatedDBObject(f,true);}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-3D-COBERTURA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-3D-COBERTURA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
