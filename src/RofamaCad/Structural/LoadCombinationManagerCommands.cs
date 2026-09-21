using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class LoadCombinationManagerCommands
{
 [CommandMethod("RFCOMBINACAOCARGA")]
 public void Create(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var ed=d.Editor;var pn=ed.GetString(new PromptStringOptions("\nNome da combinação: "){AllowSpaces=false});if(pn.Status!=PromptStatus.OK||string.IsNullOrWhiteSpace(pn.StringResult))return;var pc=ed.GetInteger(new PromptIntegerOptions("\nQuantidade de casos: "){LowerLimit=1,UpperLimit=20});if(pc.Status!=PromptStatus.OK)return;var terms=new List<LoadCombinationService.Term>();
  for(int i=0;i<pc.Value;i++){var pcase=ed.GetString(new PromptStringOptions($"\nCaso {i+1}: "){AllowSpaces=false});if(pcase.Status!=PromptStatus.OK)return;var pf=ed.GetDouble(new PromptDoubleOptions($"\nFator para {pcase.StringResult}: "){AllowNegative=true,AllowZero=true});if(pf.Status!=PromptStatus.OK)return;terms.Add(new(pcase.StringResult.Trim().ToUpperInvariant(),pf.Value));}
  using var tr=d.Database.TransactionManager.StartTransaction();LoadCombinationService.Write(d.Database,tr,pn.StringResult.Trim().ToUpperInvariant(),terms);tr.Commit();ed.WriteMessage($"\nCombinação {pn.StringResult.ToUpperInvariant()} gravada com {terms.Count} caso(s).");
 }
}
