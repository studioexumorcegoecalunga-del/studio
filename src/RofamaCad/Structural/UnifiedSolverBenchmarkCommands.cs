using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class UnifiedSolverBenchmarkCommands
{
 [CommandMethod("RFTESTARV7")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=0;
  void T(string name,Func<bool> f){total++;try{bool p=f();if(p)ok++;ed.WriteMessage($"\n{name}: {(p?"OK":"FALHOU")}");}catch(System.Exception ex){ed.WriteMessage($"\n{name}: ERRO {ex.Message}");}}
  T("Simples UDL+P centro",()=>{var rel=new bool[1,2];var r=UnifiedBeamSolver.Solve(new[]{6d},new[]{1d},new[]{10d},new[]{true,true},new[]{false,false},rel,new[]{new UnifiedBeamSolver.PointLoad(0,3,20)});return Eq(r.Reaction[0],40)&&Eq(r.Reaction[2],40);});
  T("Balanço UDL+P ponta",()=>{var rel=new bool[1,2];var r=UnifiedBeamSolver.Solve(new[]{4d},new[]{1d},new[]{5d},new[]{true,false},new[]{true,false},rel,new[]{new UnifiedBeamSolver.PointLoad(0,4,15)});return Eq(r.Reaction[0],35)&&Eq(Math.Abs(r.Reaction[1]),100);});
  T("Biapoiada por liberações + P",()=>{var rel=new bool[1,2];rel[0,0]=rel[0,1]=true;var r=UnifiedBeamSolver.Solve(new[]{6d},new[]{1d},new[]{0d},new[]{true,true},new[]{true,true},rel,new[]{new UnifiedBeamSolver.PointLoad(0,3,20)});return Eq(r.Reaction[0],10)&&Eq(r.Reaction[2],10)&&Eq(r.EndForce[1],0)&&Eq(r.EndForce[3],0);});
  ed.WriteMessage($"\nBENCHMARK V7: {ok}/{total}.");
 }
 static bool Eq(double a,double b)=>Math.Abs(a-b)<=Math.Max(1e-6,Math.Abs(b)*1e-6);
}
