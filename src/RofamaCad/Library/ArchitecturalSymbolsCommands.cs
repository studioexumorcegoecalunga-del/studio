using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Library;
public sealed class ArchitecturalSymbolsCommands
{
 [CommandMethod("RFSIMBOLO")]
 public void Symbol(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var k=new PromptKeywordOptions("\nSímbolo [Chuveiro/Geladeira/Fogao/Vaso/Pia/CamaCasal/CamaSolteiro/Sofa/Mesa6] <Mesa6>: ");
  foreach(var x in new[]{"Chuveiro","Geladeira","Fogao","Vaso","Pia","CamaCasal","CamaSolteiro","Sofa","Mesa6"})k.Keywords.Add(x);k.Keywords.Default="Mesa6";var r=ed.GetKeywords(k);if(r.Status!=PromptStatus.OK)return;
  var p=ed.GetPoint("\nPonto de inserção: ");if(p.Status!=PromptStatus.OK)return;var rot=ed.GetAngle(new PromptAngleOptions("\nRotação <0>: "){DefaultValue=0,UseDefaultValue=true,BasePoint=p.Value});if(rot.Status!=PromptStatus.OK)return;
  var size=r.StringResult switch{"Chuveiro"=>(.80,.80),"Geladeira"=>(.70,.70),"Fogao"=>(.60,.60),"Vaso"=>(.40,.65),"Pia"=>(.60,.50),"CamaCasal"=>(1.40,1.90),"CamaSolteiro"=>(.90,1.90),"Sofa"=>(1.80,.80),_=>(1.80,.90)};
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var pl=Rect(p.Value,size.Item1,size.Item2);pl.TransformBy(Matrix3d.Rotation(rot.Value,Vector3d.ZAxis,p.Value));pl.Layer="RF-BIBLIOTECA";ms.AppendEntity(pl);tr.AddNewlyCreatedDBObject(pl,true);
  var tx=new DBText{Position=p.Value,AlignmentPoint=p.Value,HorizontalMode=TextHorizontalMode.TextCenter,TextString=r.StringResult.ToUpperInvariant(),Height=.10,Layer="RF-BIBLIOTECA",Rotation=rot.Value};ms.AppendEntity(tx);tr.AddNewlyCreatedDBObject(tx,true);tr.Commit();
 }
 static Polyline Rect(Point3d c,double w,double h){var p=new Polyline(4);p.AddVertexAt(0,new Point2d(c.X-w/2,c.Y-h/2),0,0,0);p.AddVertexAt(1,new Point2d(c.X+w/2,c.Y-h/2),0,0,0);p.AddVertexAt(2,new Point2d(c.X+w/2,c.Y+h/2),0,0,0);p.AddVertexAt(3,new Point2d(c.X-w/2,c.Y+h/2),0,0,0);p.Closed=true;return p;}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-BIBLIOTECA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-BIBLIOTECA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
