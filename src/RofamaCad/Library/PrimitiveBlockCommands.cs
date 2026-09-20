using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Library;
public sealed class PrimitiveBlockCommands
{
 [CommandMethod("RFBLOCO")]
 public void Insert(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var k=new PromptKeywordOptions("\nSímbolo [Vaso/Pia/Cama/Sofa/Mesa] <Mesa>: ");foreach(var x in new[]{"Vaso","Pia","Cama","Sofa","Mesa"})k.Keywords.Add(x);k.Keywords.Default="Mesa";var r=ed.GetKeywords(k);if(r.Status!=PromptStatus.OK)return;
  var p=ed.GetPoint("\nPonto de inserção: ");if(p.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);Ensure(tr,db);
  var size=r.StringResult switch{"Vaso"=>(.40,.65),"Pia"=>(.60,.50),"Cama"=>(1.40,1.90),"Sofa"=>(1.80,.80),_=>(1.20,.80)};
  var q=new Polyline(4);q.AddVertexAt(0,new Point2d(p.Value.X-size.Item1/2,p.Value.Y-size.Item2/2),0,0,0);q.AddVertexAt(1,new Point2d(p.Value.X+size.Item1/2,p.Value.Y-size.Item2/2),0,0,0);q.AddVertexAt(2,new Point2d(p.Value.X+size.Item1/2,p.Value.Y+size.Item2/2),0,0,0);q.AddVertexAt(3,new Point2d(p.Value.X-size.Item1/2,p.Value.Y+size.Item2/2),0,0,0);q.Closed=true;q.Layer="RF-BIBLIOTECA";ms.AppendEntity(q);tr.AddNewlyCreatedDBObject(q,true);
  var tx=new DBText{Position=p.Value,TextString=r.StringResult.ToUpperInvariant(),Height=.12,Layer="RF-BIBLIOTECA",HorizontalMode=TextHorizontalMode.TextCenter,AlignmentPoint=p.Value};ms.AppendEntity(tx);tr.AddNewlyCreatedDBObject(tx,true);tr.Commit();
  ed.WriteMessage($"\nSímbolo paramétrico {r.StringResult} inserido. Esta é a biblioteca nativa inicial; blocos DWG detalhados serão adicionados separadamente.");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-BIBLIOTECA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-BIBLIOTECA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
