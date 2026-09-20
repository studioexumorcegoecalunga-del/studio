using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class StairCommands
{
 [CommandMethod("RFESCADA")]
 public void Stair(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var p=ed.GetPoint("\nInício da escada: ");if(p.Status!=PromptStatus.OK)return;
  var a=ed.GetAngle(new PromptAngleOptions("\nDireção: "){BasePoint=p.Value,UseBasePoint=true});if(a.Status!=PromptStatus.OK)return;
  var w=ed.GetDouble(new PromptDoubleOptions("\nLargura <0.90>: "){DefaultValue=.90,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(w.Status!=PromptStatus.OK)return;
  var h=ed.GetDouble(new PromptDoubleOptions("\nDesnível total <2.80>: "){DefaultValue=2.80,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(h.Status!=PromptStatus.OK)return;
  var risers=Math.Max(2,(int)Math.Ceiling(h.Value/.18));var rise=h.Value/risers;var tread=Math.Max(.25,Math.Min(.32,.63-2*rise));var v=new Vector3d(Math.Cos(a.Value),Math.Sin(a.Value),0);var n=new Vector3d(-v.Y,v.X,0);
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  for(int i=0;i<=risers;i++){var c=p.Value+v*(i*tread);var ln=new Line(c-n*w.Value/2,c+n*w.Value/2){Layer="RF-ARQ-ESCADA"};ms.AppendEntity(ln);tr.AddNewlyCreatedDBObject(ln,true);StoreyService.Tag(ln,tr,db);}
  tr.Commit();ed.WriteMessage($"\nEscada: {risers} espelhos de {rise:0.000} m e piso aproximado {tread:0.000} m. Dimensionamento deve ser conferido conforme uso e norma aplicável.");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-ARQ-ESCADA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-ARQ-ESCADA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
