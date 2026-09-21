using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class ReleaseBenchmarkCommands
{
 [CommandMethod("RFTESTARLIBERACOES")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=0;
  void T(string name,Func<bool> f){total++;try{bool p=f();if(p)ok++;ed.WriteMessage($"\n{name}: {(p?"OK":"FALHOU")}");}catch(Exception ex){ed.WriteMessage($"\n{name}: ERRO {ex.Message}");}}
  T("Biapoiada via liberações",()=>{var rel=new bool[1,2];rel[0,0]=rel[0,1]=true;var r=ReleasedBeamSolver.Solve(new[]{6d},new[]{1d},new[]{10d},new[]{true,true},new[]{true,true},rel);return Eq(r.Reaction[0],30)&&Eq(r.Reaction[2],30)&&Eq(r.EndForce[1],0)&&Eq(r.EndForce[3],0);});
  T("Mecanismo livre",()=>{try{var rel=new bool[1,2];rel[0,0]=rel[0,1]=true;ReleasedBeamSolver.Solve(new[]{5d},new[]{1d},new[]{1d},new[]{false,false},new[]{false,false},rel);return false;}catch(InvalidOperationException){return true;}});
  ed.WriteMessage($"\nBENCHMARK LIBERAÇÕES: {ok}/{total}.");
 }
 static bool Eq(double a,double b)=>Math.Abs(a-b)<=Math.Max(1e-6,Math.Abs(b)*1e-6);
}
