using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Core;
public sealed class ProjectSettings
{
 const string Key="ROFAMA_PROJECT_SETTINGS";
 [CommandMethod("RFCONFIG")]
 public void Configure(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var db=d.Database;
  var wh=ed.GetDouble(new Autodesk.AutoCAD.EditorInput.PromptDoubleOptions("\nAltura padrão de parede <2.80>: "){DefaultValue=2.80,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(wh.Status!=Autodesk.AutoCAD.EditorInput.PromptStatus.OK)return;
  var wt=ed.GetDouble(new Autodesk.AutoCAD.EditorInput.PromptDoubleOptions("\nEspessura padrão de parede <0.15>: "){DefaultValue=.15,UseDefaultValue=true,AllowZero=false,AllowNegative=false});if(wt.Status!=Autodesk.AutoCAD.EditorInput.PromptStatus.OK)return;
  var city=ed.GetKeywords(new Autodesk.AutoCAD.EditorInput.PromptKeywordOptions("\nPerfil municipal [PortoFerreira/SantaRita/Descalvado/Tambau/Generico] <Generico>: "){AllowNone=true});
  var profile=city.Status==Autodesk.AutoCAD.EditorInput.PromptStatus.OK?city.StringResult:"Generico";
  using var tr=db.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);
  Xrecord x;if(nod.Contains(Key)){x=(Xrecord)tr.GetObject(nod.GetAt(Key),OpenMode.ForWrite);}else{x=new Xrecord();nod.SetAt(Key,x);tr.AddNewlyCreatedDBObject(x,true);}
  x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Real,wh.Value),new TypedValue((int)DxfCode.Real,wt.Value),new TypedValue((int)DxfCode.Text,profile));tr.Commit();
  ed.WriteMessage($"\nROFAMA configurado: parede {wt.Value:0.00} m, altura {wh.Value:0.00} m, perfil {profile}.");
 }
}
