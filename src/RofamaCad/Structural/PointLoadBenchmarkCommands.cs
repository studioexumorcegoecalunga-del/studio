using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class PointLoadBenchmarkCommands
{
 [CommandMethod("RFTESTARCARGAPONTUAL")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=0;
  void T(string name,double L,double P,double x,bool[] fv,bool[] fr,Func<FrameBeamSolver.Result,bool> check){total++;try{var r=PointLoadBeamSolver.Solve(new[]{L},new[]{1d},new[]{0d},fv,fr,new[]{new PointLoadBeamSolver.PointLoad(0,x,P)});bool p=check(r);if(p)ok++;ed.WriteMessage($"\n{name}: {(p?"OK":"FALHOU")} | Rv=[{r.Reaction[0]:0.000}, {r.Reaction[2]:0.000}] | M0={r.Reaction[1]:0.000}");}catch(System.Exception ex){ed.WriteMessage($"\n{name}: ERRO {ex.Message}");}}
  T("Simples P no meio",6,20,3,new[]{true,true},new[]{false,false},r=>Eq(r.Reaction[0],10)&&Eq(r.Reaction[2],10));
  T("Simples P assimétrica",10,30,4,new[]{true,true},new[]{false,false},r=>Eq(r.Reaction[0],18)&&Eq(r.Reaction[2],12));
  T("Balanço P na ponta",4,15,4,new[]{true,false},new[]{true,false},r=>Eq(r.Reaction[0],15)&&Eq(Math.Abs(r.Reaction[1]),60));
  ed.WriteMessage($"\nBENCHMARK CARGA PONTUAL: {ok}/{total}.");
 }
 static bool Eq(double a,double b)=>Math.Abs(a-b)<=Math.Max(1e-6,Math.Abs(b)*1e-6);
}
