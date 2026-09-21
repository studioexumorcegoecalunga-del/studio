using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class SolverV2BenchmarkCommands
{
 [CommandMethod("RFTESTARSOLVERV2")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=4;double EI=25000;
  var a=UnifiedBeamSolverV2.Solve(new[]{6.0},new[]{EI},new[]{10.0},new[]{true,true},new[]{false,false},new[]{new UnifiedBeamSolverV2.Release(false,false)},Array.Empty<UnifiedBeamSolverV2.PointLoad>());if(Near(a.Reaction[0],30,1e-6)&&Near(a.Reaction[2],30,1e-6))ok++;
  var b=UnifiedBeamSolverV2.Solve(new[]{6.0},new[]{EI},new[]{10.0},new[]{true,true},new[]{false,false},new[]{new UnifiedBeamSolverV2.Release(true,true)},Array.Empty<UnifiedBeamSolverV2.PointLoad>());if(Near(b.EndForce[1],0,1e-6)&&Near(b.EndForce[3],0,1e-6))ok++;
  var c=UnifiedBeamSolverV2.Solve(new[]{4.0},new[]{EI},new[]{5.0},new[]{true,false},new[]{true,false},new[]{new UnifiedBeamSolverV2.Release(false,false)},Array.Empty<UnifiedBeamSolverV2.PointLoad>());if(Near(c.Reaction[0],20,1e-6)&&Near(Math.Abs(c.Reaction[1]),40,1e-6))ok++;
  bool mechanism=false;try{UnifiedBeamSolverV2.Solve(new[]{4.0},new[]{EI},new[]{0.0},new[]{false,false},new[]{false,false},new[]{new UnifiedBeamSolverV2.Release(true,true)},new[]{new UnifiedBeamSolverV2.PointLoad(0,2,10)});}catch(InvalidOperationException){mechanism=true;}if(mechanism)ok++;
  ed.WriteMessage($"\nSolver V2: {ok}/{total} benchmarks. Biapoiada UDL, liberações, balanço e mecanismo.");
 }
 static bool Near(double a,double b,double t)=>Math.Abs(a-b)<=t;
}
