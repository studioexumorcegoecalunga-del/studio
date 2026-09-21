using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralCommands
{
 const string Col="RF-EST-PILAR",Beam="RF-EST-VIGA",SlabLayer="RF-EST-LAJE";
 [CommandMethod("RFESTRUTURA")]
 public void Generate(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var sel=ed.GetSelection(new SelectionFilter(new[]{new TypedValue((int)DxfCode.Start,"LWPOLYLINE"),new TypedValue((int)DxfCode.LayerName,"RF-ARQ-PAREDE")}));
  if(sel.Status!=PromptStatus.OK){ed.WriteMessage("\nSelecione as paredes arquitetônicas a analisar.");return;}
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db,Col);Ensure(tr,db,Beam);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var pts=new List<Point3d>();var axes=new List<(Point3d A,Point3d B)>();
  foreach(var id in sel.Value.GetObjectIds()){if(tr.GetObject(id,OpenMode.ForRead) is not Polyline p||p.NumberOfVertices<4)continue;var ex=p.GeometricExtents;var dx=ex.MaxPoint.X-ex.MinPoint.X;var dy=ex.MaxPoint.Y-ex.MinPoint.Y;Point3d a,b;if(dx>=dy){var y=(ex.MinPoint.Y+ex.MaxPoint.Y)/2;a=new(ex.MinPoint.X,y,0);b=new(ex.MaxPoint.X,y,0);}else{var x=(ex.MinPoint.X+ex.MaxPoint.X)/2;a=new(x,ex.MinPoint.Y,0);b=new(x,ex.MaxPoint.Y,0);}axes.Add((a,b));pts.Add(a);pts.Add(b);}
  foreach(var x in axes)foreach(var y in axes){if(TryIntersection(x,y,out var q))pts.Add(q);}
  var nodes=pts.GroupBy(p=>(Math.Round(p.X,2),Math.Round(p.Y,2))).Select(g=>g.First()).ToList();int pi=1,vi=1;
  foreach(var p in nodes){var q=Rect(p,.19,.19);q.Layer=Col;ms.AppendEntity(q);tr.AddNewlyCreatedDBObject(q,true);Tag(q,tr,db,"PILAR",$"P{pi++:00}");}
  foreach(var x in axes){var l=new Line(x.A,x.B){Layer=Beam};ms.AppendEntity(l);tr.AddNewlyCreatedDBObject(l,true);Tag(l,tr,db,"VIGA",$"V{vi++:00}");}
  tr.Commit();ed.WriteMessage($"\nLançamento estrutural preliminar: {nodes.Count} candidato(s) a pilar e {axes.Count} eixo(s) de viga. Exige revisão e dimensionamento estrutural.");
 }
 [CommandMethod("RFLAJEEST")]
 public void Slab(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var o=new PromptEntityOptions("\nSelecione contorno fechado da laje: ");o.SetRejectMessage("\nSelecione polilinha.");o.AddAllowedClass(typeof(Polyline),true);var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var p=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);if(!p.Closed){ed.WriteMessage("\nContorno deve estar fechado.");return;}Ensure(tr,db,SlabLayer);var cp=(Polyline)p.Clone();cp.Layer=SlabLayer;var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);ms.AppendEntity(cp);tr.AddNewlyCreatedDBObject(cp,true);Tag(cp,tr,db,"LAJE","L01");tr.Commit();
 }
 static void Tag(Entity e,Transaction tr,Database db,string type,string code){StoreyService.Tag(e,tr,db);if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,type),new TypedValue((int)DxfCode.Text,code),new TypedValue((int)DxfCode.Text,"PRELIMINAR_NAO_DIMENSIONADO"))};d.SetAt("ROFAMA_STRUCT",x);tr.AddNewlyCreatedDBObject(x,true);}
 static bool TryIntersection((Point3d A,Point3d B)a,(Point3d A,Point3d B)b,out Point3d p){p=Point3d.Origin;var x1=a.A.X;double y1=a.A.Y,x2=a.B.X,y2=a.B.Y,x3=b.A.X,y3=b.A.Y,x4=b.B.X,y4=b.B.Y;var den=(x1-x2)*(y3-y4)-(y1-y2)*(x3-x4);if(Math.Abs(den)<1e-9)return false;var px=((x1*y2-y1*x2)*(x3-x4)-(x1-x2)*(x3*y4-y3*x4))/den;var py=((x1*y2-y1*x2)*(y3-y4)-(y1-y2)*(x3*y4-y3*x4))/den;if(px<Math.Min(x1,x2)-.01||px>Math.Max(x1,x2)+.01||py<Math.Min(y1,y2)-.01||py>Math.Max(y1,y2)+.01)return false;if(px<Math.Min(x3,x4)-.01||px>Math.Max(x3,x4)+.01||py<Math.Min(y3,y4)-.01||py>Math.Max(y3,y4)+.01)return false;p=new Point3d(px,py,0);return true;}
 static Polyline Rect(Point3d c,double w,double h){var p=new Polyline(4);p.AddVertexAt(0,new(c.X-w/2,c.Y-h/2),0,0,0);p.AddVertexAt(1,new(c.X+w/2,c.Y-h/2),0,0,0);p.AddVertexAt(2,new(c.X+w/2,c.Y+h/2),0,0,0);p.AddVertexAt(3,new(c.X-w/2,c.Y+h/2),0,0,0);p.Closed=true;return p;}
 static void Ensure(Transaction tr,Database db,string name){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(name))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name=name};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
