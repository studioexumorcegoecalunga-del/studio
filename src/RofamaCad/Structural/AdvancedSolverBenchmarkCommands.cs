using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class AdvancedSolverBenchmarkCommands
{
 [CommandMethod("RFTESTARSOLVER2")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=0;
  void T(string name,double[] L,double[] W,Func<BeamStiffnessSolver.Result,bool> check){total++;var r=BeamStiffnessSolver.Solve(L,Enumerable.Repeat(1.0,L.Length).ToArray(),W);bool p=check(r);if(p)ok++;ed.WriteMessage($"\n{name}: {(p?"OK":"FALHOU")} | R=[{string.Join(", ",r.Reaction.Select(x=>x.ToString("0.000")))}] | M=[{string.Join(", ",r.EndMoment.Select(x=>x.ToString("0.000")))}]");}
  T("2 vãos iguais UDL",new[]{5d,5d},new[]{10d,10d},r=>Eq(r.Reaction.Sum(),100)&&Eq(r.EndMoment[1],-r.EndMoment[2]));
  T("2 vãos desiguais",new[]{4d,6d},new[]{8d,12d},r=>Eq(r.Reaction.Sum(),104));
  T("3 vãos cargas distintas",new[]{3d,5d,4d},new[]{5d,11d,7d},r=>Eq(r.Reaction.Sum(),98));
  T("Escala EI uniforme",new[]{5d,5d},new[]{10d,10d},r=>r.Rotation.All(double.IsFinite)&&r.EndMoment.All(double.IsFinite));
  ed.WriteMessage($"\nBENCHMARK AVANÇADO: {ok}/{total}. Testes de equilíbrio/sanidade; ainda não substituem comparação externa independente.");
 }
 static bool Eq(double a,double b)=>Math.Abs(a-b)<=Math.Max(1e-7,Math.Abs(b)*1e-7);
}
