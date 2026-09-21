using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class FrameSolverBenchmarkCommands
{
 [CommandMethod("RFTESTARFRAME")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=0;
  void T(string name,double[] L,double[] EI,double[] W,bool[] fv,bool[] fr,Func<FrameBeamSolver.Result,bool> check){total++;try{var r=FrameBeamSolver.Solve(L,EI,W,fv,fr);bool p=check(r);if(p)ok++;ed.WriteMessage($"\n{name}: {(p?"OK":"FALHOU")} | Rv=[{string.Join(", ",Enumerable.Range(0,fv.Length).Select(i=>r.Reaction[2*i].ToString("0.000")))}]");}catch(Exception ex){ed.WriteMessage($"\n{name}: ERRO {ex.Message}");}}
  T("Simples UDL",new[]{6d},new[]{1d},new[]{10d},new[]{true,true},new[]{false,false},r=>Eq(r.Reaction[0],30)&&Eq(r.Reaction[2],30));
  T("Balanço UDL",new[]{4d},new[]{1d},new[]{5d},new[]{true,false},new[]{true,false},r=>Eq(r.Reaction[0],20)&&Eq(Math.Abs(r.Reaction[1]),40));
  T("Engastada-engastada UDL",new[]{4d},new[]{1d},new[]{6d},new[]{true,true},new[]{true,true},r=>Eq(r.Reaction[0],12)&&Eq(r.Reaction[2],12)&&Eq(Math.Abs(r.Reaction[1]),8)&&Eq(Math.Abs(r.Reaction[3]),8));
  T("2 vãos contínuos",new[]{5d,5d},new[]{1d,1d},new[]{10d,10d},new[]{true,true,true},new[]{false,false,false},r=>Eq(r.Reaction[0]+r.Reaction[2]+r.Reaction[4],100)&&Eq(r.Reaction[0],r.Reaction[4]));
  ed.WriteMessage($"\nFRAME BENCHMARK: {ok}/{total}. Casos clássicos de reação/momento de apoio incluídos; validar também deslocamentos e sinais com referência independente.");
 }
 static bool Eq(double a,double b)=>Math.Abs(a-b)<=Math.Max(1e-6,Math.Abs(b)*1e-6);
}
