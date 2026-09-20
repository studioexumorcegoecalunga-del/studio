using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class SolverBenchmarkCommands
{
 [CommandMethod("RFTESTARSOLVER")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=0;
  void Test(string name,double[] L,double[] W,Func<BeamStiffnessSolver.Result,bool> check){
   total++;var ei=Enumerable.Repeat(1.0,L.Length).ToArray();var r=BeamStiffnessSolver.Solve(L,ei,W);var pass=check(r);if(pass)ok++;ed.WriteMessage($"\n{name}: {(pass?"OK":"FALHOU")} | ΣR={r.Reaction.Sum():0.000} | ΣW={L.Zip(W,(l,w)=>l*w).Sum():0.000}");
  }
  Test("1 vão simples",new[]{5.0},new[]{10.0},r=>Near(r.Reaction.Sum(),50)&&Near(r.Reaction[0],25)&&Near(r.Reaction[1],25));
  Test("2 vãos simétricos",new[]{5.0,5.0},new[]{10.0,10.0},r=>Near(r.Reaction.Sum(),100)&&Near(r.Reaction[0],r.Reaction[2]));
  Test("3 vãos simétricos",new[]{4.0,4.0,4.0},new[]{8.0,8.0,8.0},r=>Near(r.Reaction.Sum(),96)&&Near(r.Reaction[0],r.Reaction[3])&&Near(r.Reaction[1],r.Reaction[2]));
  ed.WriteMessage($"\nBENCHMARK SOLVER: {ok}/{total} teste(s) aprovados. {(ok==total?"Base passou nos testes atuais.":"Há falhas: não usar o solver para dimensionamento.")}");
 }
 static bool Near(double a,double b)=>Math.Abs(a-b)<=Math.Max(1e-6,Math.Abs(b)*1e-6);
}
