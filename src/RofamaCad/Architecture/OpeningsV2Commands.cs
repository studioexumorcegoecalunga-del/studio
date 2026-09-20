using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;

public sealed class OpeningsV2Commands
{
 const string DoorLayer="RF-ARQ-PORTA",WindowLayer="RF-ARQ-JANELA";

 [CommandMethod("RFPORTA2")]
 public void Door()
 {
  var doc=AcApp.DocumentManager.MdiActiveDocument;var ed=doc.Editor;var db=doc.Database;
  var p=ed.GetPoint("\nPonto da dobradiça: ");if(p.Status!=PromptStatus.OK)return;
  var a=ed.GetAngle(new PromptAngleOptions("\nDireção da parede: "){BasePoint=p.Value,UseBasePoint=true});if(a.Status!=PromptStatus.OK)return;
  var w=ed.GetDouble(new PromptDoubleOptions("\nLargura <0.80>: "){DefaultValue=.80,UseDefaultValue=true,AllowNegative=false,AllowZero=false});if(w.Status!=PromptStatus.OK)return;
  var h=ed.GetDouble(new PromptDoubleOptions("\nAltura <2.10>: "){DefaultValue=2.10,UseDefaultValue=true,AllowNegative=false,AllowZero=false});if(h.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();EnsureLayer(tr,db,DoorLayer);var ms=Model(tr,db);
  var code=NextCode(tr,ms,DoorLayer,"P");var v=new Vector3d(Math.Cos(a.Value),Math.Sin(a.Value),0);var n=new Vector3d(-v.Y,v.X,0);
  var jamb1=new Line(p.Value-n*.075,p.Value+n*.075){Layer=DoorLayer};Add(ms,tr,jamb1);
  var end=p.Value+v*w.Value;var jamb2=new Line(end-n*.075,end+n*.075){Layer=DoorLayer};Add(ms,tr,jamb2);
  var leaf=new Line(p.Value,p.Value+n*w.Value){Layer=DoorLayer};Add(ms,tr,leaf);
  var arc=new Arc(p.Value,w.Value,a.Value,a.Value+Math.PI/2){Layer=DoorLayer};Add(ms,tr,arc);
  var marker=new DBText{Position=p.Value+v*(w.Value/2)+n*.18,TextString=code,Height=.15,Layer=DoorLayer};Add(ms,tr,marker);
  SetData(jamb1,tr,"PORTA",w.Value,h.Value,0,code,a.Value);tr.Commit();ed.WriteMessage($"\n{code} criada: {w.Value:0.00} x {h.Value:0.00} m.");
 }

 [CommandMethod("RFJANELA2")]
 public void Window()
 {
  var doc=AcApp.DocumentManager.MdiActiveDocument;var ed=doc.Editor;var db=doc.Database;
  var p=ed.GetPoint("\nCentro da janela: ");if(p.Status!=PromptStatus.OK)return;
  var a=ed.GetAngle(new PromptAngleOptions("\nDireção da parede: "){BasePoint=p.Value,UseBasePoint=true});if(a.Status!=PromptStatus.OK)return;
  var w=ed.GetDouble(new PromptDoubleOptions("\nLargura <1.20>: "){DefaultValue=1.20,UseDefaultValue=true,AllowNegative=false,AllowZero=false});if(w.Status!=PromptStatus.OK)return;
  var h=ed.GetDouble(new PromptDoubleOptions("\nAltura <1.20>: "){DefaultValue=1.20,UseDefaultValue=true,AllowNegative=false,AllowZero=false});if(h.Status!=PromptStatus.OK)return;
  var s=ed.GetDouble(new PromptDoubleOptions("\nPeitoril <1.00>: "){DefaultValue=1.00,UseDefaultValue=true,AllowNegative=false});if(s.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();EnsureLayer(tr,db,WindowLayer);var ms=Model(tr,db);var code=NextCode(tr,ms,WindowLayer,"J");
  var v=new Vector3d(Math.Cos(a.Value),Math.Sin(a.Value),0);var n=new Vector3d(-v.Y,v.X,0);var a1=p.Value-v*w.Value/2;var b1=p.Value+v*w.Value/2;
  foreach(var off in new[]{-.05,0.0,.05}){var ln=new Line(a1+n*off,b1+n*off){Layer=WindowLayer};Add(ms,tr,ln);if(off==0)SetData(ln,tr,"JANELA",w.Value,h.Value,s.Value,code,a.Value);}
  var marker=new DBText{Position=p.Value+n*.18,TextString=code,Height=.15,Layer=WindowLayer};Add(ms,tr,marker);tr.Commit();ed.WriteMessage($"\n{code} criada: {w.Value:0.00} x {h.Value:0.00} m, peitoril {s.Value:0.00} m.");
 }

 static string NextCode(Transaction tr,BlockTableRecord ms,string layer,string prefix)
 {
  int max=0;
  foreach(ObjectId id in ms)if(tr.GetObject(id,OpenMode.ForRead) is Entity e&&e.Layer==layer&&!e.ExtensionDictionary.IsNull){var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_DATA"))continue;var x=(Xrecord)tr.GetObject(d.GetAt("ROFAMA_DATA"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a==null||a.Length<5)continue;var c=a[4].Value?.ToString()??"";if(c.StartsWith(prefix)&&int.TryParse(c[1..],out var n))max=Math.Max(max,n);}
  return $"{prefix}{max+1:00}";
 }
 static void SetData(Entity e,Transaction tr,string type,double w,double h,double sill,string code,double angle){if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,type),new TypedValue((int)DxfCode.Real,w),new TypedValue((int)DxfCode.Real,h),new TypedValue((int)DxfCode.Real,sill),new TypedValue((int)DxfCode.Text,code),new TypedValue((int)DxfCode.Text,Guid.NewGuid().ToString("N")),new TypedValue((int)DxfCode.Real,angle))};d.SetAt("ROFAMA_DATA",x);tr.AddNewlyCreatedDBObject(x,true);}
 static void EnsureLayer(Transaction tr,Database db,string n){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(n))return;lt.UpgradeOpen();var r=new LayerTableRecord{Name=n};lt.Add(r);tr.AddNewlyCreatedDBObject(r,true);}
 static BlockTableRecord Model(Transaction tr,Database db)=>(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
 static void Add(BlockTableRecord ms,Transaction tr,Entity e){ms.AppendEntity(e);tr.AddNewlyCreatedDBObject(e,true);}
}
