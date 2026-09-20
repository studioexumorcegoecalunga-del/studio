using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Library;
public sealed class LibraryCommands
{
 [CommandMethod("RFBIBLIOTECA")]
 public void Library(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;
  var k=new PromptKeywordOptions("\nCategoria [Portas/Janelas/Mobiliario/Loucas/Pias/Eletrica/Hidraulica/HVAC/Incendio] <Mobiliario>: ");
  foreach(var x in new[]{"Portas","Janelas","Mobiliario","Loucas","Pias","Eletrica","Hidraulica","HVAC","Incendio"})k.Keywords.Add(x);k.Keywords.Default="Mobiliario";
  var r=ed.GetKeywords(k);if(r.Status!=PromptStatus.OK)return;
  ed.WriteMessage($"\nBiblioteca ROFAMA: categoria {r.StringResult}. Catálogo DWG externo será carregado nesta interface na próxima etapa.");
 }
}
