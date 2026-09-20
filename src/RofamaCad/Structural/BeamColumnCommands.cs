using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class BeamColumnCommands
{
 [CommandMethod("RFPILAR")]
 public void Column(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var p=ed.GetPoint("\nCentro do pilar: ");if(p.Status!=PromptStatus.OK)return;
  var bx=ed.GetDouble(new PromptDoubleOptions("\nDimensão X preliminar <0.19>: "){DefaultValue=.19,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(bx.Status!=PromptStatus.OK)return;
  var by=ed.GetDouble(new PromptDoubleOptions("\nDimensão Y preliminar <0.19>: "){DefaultValue=.19,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(by.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db,"RF-EST-PILAR");var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var q=Rect(p.Value,bx.Value,by.Value);q.Layer="RF-EST-PILAR";ms.AppendEntity(q);tr.AddNewlyCreatedDBObject(q,true);Meta(q,tr,db,"PILAR",bx.Value,by.Value);tr.Commit();
 }
 [CommandMethod("RFVIGA")]
 public void Beam(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var a=ed.GetPoint("\nInício/eixo do apoio: ");if(a.Status!=PromptStatus.OK)return;var b=ed.GetPoint(new PromptPointOptions("\nFim/eixo do apoio: "){BasePoint=a.Value,UseBasePoint=true});if(b.Status!=PromptStatus.OK)return;
  var bw=ed.GetDouble(new PromptDoubleOptions("\nLargura preliminar <0.15>: "){DefaultValue=.15,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(bw.Status!=PromptStatus.OK)return;
  var h=ed.GetDouble(new PromptDoubleOptions("\nAltura preliminar <0.40>: "){DefaultValue=.40,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(h.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db,"RF-EST-VIGA");var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var l=new Line(a.Value,b.Value){Layer="RF-EST-VIGA"};ms.AppendEntity(l);tr.AddNewlyCreatedDBObject(l,true);Meta(l,tr,db,"VIGA",bw.Value,h.Value);tr.Commit();
 }
 static void Meta(Entity e,Transaction tr,Database db,string type,double a,double b){StoreyService.Tag(e,tr,db);if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,type),new TypedValue((int)DxfCode.Real,a),new TypedValue((int)DxfCode.Real,b),new TypedValue((int)DxfCode.Text,"PRELIMINAR_NAO_DIMENSIONADO"))};d.SetAt("ROFAMA_STRUCT",x);tr.AddNewlyCreatedDBObject(x,true);}
 static Polyline Rect(Point3d c,double w,double h){var p=new Polyline(4);p.AddVertexAt(0,new(c.X-w/2,c.Y-h/2),0,0,0);p.AddVertexAt(1,new(c.X+w/2,c.Y-h/2),0,0,0);p.AddVertexAt(2,new(c.X+w/2,c.Y+h/2),0,0,0);p.AddVertexAt(3,new(c.X-w/2,c.Y+h/2),0,0,0);p.Closed=true;return p;}
 static void Ensure(Transaction tr,Database db,string n){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(n))return;lt.UpgradeOpen();var r=new LayerTableRecord{Name=n};lt.Add(r);tr.AddNewlyCreatedDBObject(r,true);}
}
