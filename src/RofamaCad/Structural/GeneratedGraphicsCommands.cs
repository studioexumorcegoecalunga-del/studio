using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class GeneratedGraphicsCommands
{
 [CommandMethod("RFLIMPARESTRUTURAL")]
 public void Run(){var d=AcApp.DocumentManager.MdiActiveDocument;var pe=d.Editor.GetEntity("\nSelecione a viga para limpar gráficos gerados: ");if(pe.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();var e=tr.GetObject(pe.ObjectId,OpenMode.ForRead) as Entity;if(e==null)return;var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(d.Database),OpenMode.ForWrite);int n=GeneratedGraphicsService.EraseFor(ms,tr,e.Handle.ToString());tr.Commit();d.Editor.WriteMessage($"\n{n} gráfico(s)/anotação(ões) associativo(s) removido(s).");}
}
