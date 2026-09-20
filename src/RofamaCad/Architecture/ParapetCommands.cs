using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using RofamaCad.Core;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class ParapetCommands
{
 [CommandMethod("RFPLATIBANDA")]
 public void Parapet(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;var o=new PromptEntityOptions("\nSelecione o contorno fechado da cobertura: ");o.SetRejectMessage("\nSelecione uma polilinha.");o.AddAllowedClass(typeof(Polyline),true);var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
  var h=ed.GetDouble(new PromptDoubleOptions("\nAltura da platibanda <0.80>: "){DefaultValue=.80,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(h.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();var src=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);if(!src.Closed){ed.WriteMessage("\nContorno deve estar fechado.");return;}Ensure(tr,db);
  var cp=(Polyline)src.Clone();cp.Layer="RF-ARQ-PLATIBANDA";var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);ms.AppendEntity(cp);tr.AddNewlyCreatedDBObject(cp,true);StoreyService.Tag(cp,tr,db);
  if(cp.ExtensionDictionary.IsNull)cp.CreateExtensionDictionary();var dic=(DBDictionary)tr.GetObject(cp.ExtensionDictionary,OpenMode.ForWrite);var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,"PLATIBANDA"),new TypedValue((int)DxfCode.Real,h.Value),new TypedValue((int)DxfCode.Real,.15))};dic.SetAt("ROFAMA_DATA",x);tr.AddNewlyCreatedDBObject(x,true);tr.Commit();ed.WriteMessage($"\nPlatibanda registrada com altura {h.Value:0.00} m.");
 }
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-ARQ-PLATIBANDA"))return;lt.UpgradeOpen();var x=new LayerTableRecord{Name="RF-ARQ-PLATIBANDA"};lt.Add(x);tr.AddNewlyCreatedDBObject(x,true);}
}
