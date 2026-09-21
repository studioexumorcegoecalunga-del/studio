using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class SolverV2AdvancedBenchmarkCommands
{
 [CommandMethod("RFTESTARSOLVERV2AVANCADO")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=5;double EI=25000;
  // Dois vãos iguais, carga simétrica: reações extremas devem ser iguais.
  var a=UnifiedBeamSolverV2.Solve(new[]{5.0,5.0},new[]{EI,EI},new[]{10.0,10.0},new[]{true,true,true},new[]{false,false,false},new[]{new UnifiedBeamSolverV2.Release(false,false),new UnifiedBeamSolverV2.Release(false,false)},Array.Empty<UnifiedBeamSolverV2.PointLoad>());
  if(Near(a.Reaction[0],a.Reaction[4],1e-6))ok++;
  if(Near(a.Reaction[0]+a.Reaction[2]+a.Reaction[4],100,1e-6))ok++;
  // Carga pontual central em viga simples: equilíbrio vertical.
  var b=UnifiedBeamSolverV2.Solve(new[]{6.0},new[]{EI},new[]{0.0},new[]{true,true},new[]{false,false},new[]{new UnifiedBeamSolverV2.Release(false,false)},new[]{new UnifiedBeamSolverV2.PointLoad(0,3,20)});
  if(Near(b.Reaction[0]+b.Reaction[2],20,1e-6))ok++;
  // Rótula na extremidade direita do primeiro vão: momento recuperado deve ser zero.
  var c=UnifiedBeamSolverV2.Solve(new[]{4.0,4.0},new[]{EI,EI},new[]{8.0,8.0},new[]{true,true,true},new[]{false,false,false},new[]{new UnifiedBeamSolverV2.Release(false,true),new UnifiedBeamSolverV2.Release(true,false)},Array.Empty<UnifiedBeamSolverV2.PointLoad>());
  if(Near(c.EndForce[3],0,1e-6)&&Near(c.EndForce[5],0,1e-6))ok++;
  if(Near(c.Reaction[0]+c.Reaction[2]+c.Reaction[4],64,1e-6))ok++;
  ed.WriteMessage($"\nSolver V2 avançado: {ok}/{total}. Simetria, equilíbrio, carga pontual e rótula interna.");
 }
 static bool Near(double a,double b,double t)=>Math.Abs(a-b)<=t;
}
