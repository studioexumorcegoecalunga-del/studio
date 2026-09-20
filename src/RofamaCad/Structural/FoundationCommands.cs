using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class FoundationCommands
{
 [CommandMethod("RFSAPATA")]
 public void Footing(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var p=ed.GetPoint("\nCentro da sapata preliminar: ");if(p.Status!=PromptStatus.OK)return;
  var a=ed.GetDouble(new PromptDoubleOptions("\nDimensão A <1.00>: "){DefaultValue=1,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(a.Status!=PromptStatus.OK)return;var b=ed.GetDouble(new PromptDoubleOptions("\nDimensão B <1.00>: "){DefaultValue=1,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(b.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var q=new Polyline(4);q.AddVertexAt(0,new(p.Value.X-a.Value/2,p.Value.Y-b.Value/2),0,0,0);q.AddVertexAt(1,new(p.Value.X+a.Value/2,p.Value.Y-b.Value/2),0,0,0);q.AddVertexAt(2,new(p.Value.X+a.Value/2,p.Value.Y+b.Value/2),0,0,0);q.AddVertexAt(3,new(p.Value.X-a.Value/2,p.Value.Y+b.Value/2),0,0,0);q.Closed=true;q.Layer="RF-EST-FUNDACAO";var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);ms.AppendEntity(q);tr.AddNewlyCreatedDBObject(q,true);StoreyService.Tag(q,tr,db);tr.Commit();ed.WriteMessage("\nSapata geométrica preliminar criada. Dimensões dependem das reações e investigação geotécnica.");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-FUNDACAO"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-EST-FUNDACAO"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
