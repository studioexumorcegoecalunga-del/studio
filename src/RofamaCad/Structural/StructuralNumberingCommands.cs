using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class StructuralNumberingCommands
{
 [CommandMethod("RFESTNUMERAR")]
 public void Number(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;int p=1,v=1,l=1;
  using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Entity e)continue;string? code=e.Layer switch{"RF-EST-PILAR"=>$"P{p++:00}","RF-EST-VIGA"=>$"V{v++:00}","RF-EST-LAJE"=>$"L{l++:00}",_=>null};if(code==null)continue;SetCode(e,tr,code);}
  tr.Commit();d.Editor.WriteMessage($"\nNumeração estrutural: {p-1} pilar(es), {v-1} viga(s), {l-1} laje(s).");
 }
 static void SetCode(Entity e,Transaction tr,string code){if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var dic=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(dic.Contains("ROFAMA_STRUCT_ID"))x=(Xrecord)tr.GetObject(dic.GetAt("ROFAMA_STRUCT_ID"),OpenMode.ForWrite);else{x=new Xrecord();dic.SetAt("ROFAMA_STRUCT_ID",x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,code));}
}
