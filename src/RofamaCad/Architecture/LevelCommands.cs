using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class LevelCommands
{
 const string Layer="RF-ARQ-NIVEL";
 [CommandMethod("RFNIVEL")]
 public void Level(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var p=ed.GetPoint("\nPonto do nível: ");if(p.Status!=PromptStatus.OK)return;
  var z=ed.GetDouble(new PromptDoubleOptions("\nCota do nível em metros <0.00>: "){DefaultValue=0,UseDefaultValue=true});if(z.Status!=PromptStatus.OK)return;
  var n=ed.GetString(new PromptStringOptions("\nNome do pavimento <TÉRREO>: "){AllowSpaces=true,DefaultValue="TÉRREO",UseDefaultValue=true});if(n.Status!=PromptStatus.OK)return;
  using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  var line=new Line(p.Value+new Vector3d(-.35,0,0),p.Value+new Vector3d(.35,0,0)){Layer=Layer};ms.AppendEntity(line);tr.AddNewlyCreatedDBObject(line,true);
  var tx=new MText{Location=p.Value+new Vector3d(.40,.05,0),TextHeight=.15,Contents=$"{levelName}  {z.Value:+0.00;-0.00;0.00}",Layer=Layer};ms.AppendEntity(tx);tr.AddNewlyCreatedDBObject(tx,true);
  tr.Commit();}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has(Layer))return;lt.UpgradeOpen();var r=new LayerTableRecord{Name=Layer};lt.Add(r);tr.AddNewlyCreatedDBObject(r,true);}
}
