using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class RoofWatersCommands
{
 [CommandMethod("RFTELHADO2AGUAS")]
 public void TwoWaters(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var o=new PromptEntityOptions("\nSelecione contorno retangular fechado do telhado: ");o.SetRejectMessage("\nSelecione uma polilinha.");o.AddAllowedClass(typeof(Polyline),true);var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  var s=ed.GetDouble(new PromptDoubleOptions("\nInclinação em % <30>: "){DefaultValue=30,UseDefaultValue=true,AllowNegative=false});if(s.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var p=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);if(!p.Closed){ed.WriteMessage("\nContorno deve estar fechado.");return;}
  var ex=p.GeometricExtents;var dx=ex.MaxPoint.X-ex.MinPoint.X;var dy=ex.MaxPoint.Y-ex.MinPoint.Y;var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);Ensure(tr,db);
  Point3d a,b;if(dx>=dy){var y=(ex.MinPoint.Y+ex.MaxPoint.Y)/2;a=new Point3d(ex.MinPoint.X,y,0);b=new Point3d(ex.MaxPoint.X,y,0);}else{var x=(ex.MinPoint.X+ex.MaxPoint.X)/2;a=new Point3d(x,ex.MinPoint.Y,0);b=new Point3d(x,ex.MaxPoint.Y,0);}
  var ridge=new Line(a,b){Layer="RF-ARQ-CUMEEIRA"};ms.AppendEntity(ridge);tr.AddNewlyCreatedDBObject(ridge,true);
  if(ridge.ExtensionDictionary.IsNull)ridge.CreateExtensionDictionary();var dic=(DBDictionary)tr.GetObject(ridge.ExtensionDictionary,OpenMode.ForWrite);var xr=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,"TELHADO_2_AGUAS"),new TypedValue((int)DxfCode.Real,s.Value))};dic.SetAt("ROFAMA_DATA",xr);tr.AddNewlyCreatedDBObject(xr,true);tr.Commit();
  ed.WriteMessage($"\nTelhado de 2 águas registrado com cumeeira central e inclinação {s.Value:0.#}%.");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-ARQ-CUMEEIRA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-ARQ-CUMEEIRA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
