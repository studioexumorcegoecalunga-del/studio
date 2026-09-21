using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class SolverDivergenceCommands
{
 [CommandMethod("RFLISTARDIVERGENCIASV2")]
 public void Run(){var d=AcApp.DocumentManager.MdiActiveDocument;using var tr=d.Database.TransactionManager.StartTransaction();var rows=SolverComparisonReportService.Read(d.Database,tr).Where(x=>!x.Pass).OrderByDescending(x=>Math.Max(x.DR,x.DE)).ToList();d.Editor.WriteMessage($"\n--- DIVERGÊNCIAS V1 x V2 ({rows.Count}) ---");foreach(var r in rows)d.Editor.WriteMessage($"\n{r.Beam}/{r.Combination}: Δu={r.DU:G5}; ΔR={r.DR:G5}; ΔE={r.DE:G5}; {r.Classification}");tr.Commit();}
 [CommandMethod("RFSTATUSSOLVERV2")]
 public void Status(){var d=AcApp.DocumentManager.MdiActiveDocument;using var tr=d.Database.TransactionManager.StartTransaction();var rows=SolverComparisonReportService.Read(d.Database,tr);int pass=rows.Count(x=>x.Pass),fail=rows.Count-pass;d.Editor.WriteMessage($"\nSTATUS SOLVER V2: comparações={rows.Count}; dentro={pass}; divergentes={fail}; promoção automática={(rows.Count>0&&fail==0?"ELEGÍVEL PARA REVISÃO":"BLOQUEADA")}.");tr.Commit();}
}
