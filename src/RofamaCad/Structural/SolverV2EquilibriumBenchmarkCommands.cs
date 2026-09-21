using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class SolverV2EquilibriumBenchmarkCommands
{
 [CommandMethod("RFAUDITARSOLVERV2")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;double EI=25000;var L=new[]{4.0,6.0};var w=new[]{7.0,3.0};var pts=new[]{new UnifiedBeamSolverV2.PointLoad(0,1.5,12),new UnifiedBeamSolverV2.PointLoad(1,4.0,18)};var r=UnifiedBeamSolverV2.Solve(L,new[]{EI,EI},w,new[]{true,true,true},new[]{false,false,false},new[]{new UnifiedBeamSolverV2.Release(false,false),new UnifiedBeamSolverV2.Release(false,false)},pts);var a=SolverV2EquilibriumService.Check(L,w,pts,r.Reaction);ed.WriteMessage($"\nAuditoria Solver V2: carga={a.AppliedVertical:0.###} kN; reação={a.ReactionVertical:0.###} kN; resíduo vertical={a.ResidualVertical:0.######} kN; M aplicado={a.AppliedMomentAboutStart:0.###}; M reação={a.ReactionMomentAboutStart:0.###}; resíduo M={a.ResidualMoment:0.######}.");}
}
