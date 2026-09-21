using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class ReleaseBenchmarkV2Commands
{
 [CommandMethod("RFTESTARLIBERACOES2")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=4;double L=6,EI=25000,w=10;
  var a=ReleasedBeamElement.Build(L,EI,w,true,true);var ua=ReleasedBeamElement.Recover(a,new[]{0.0,0.0});var qa=ReleasedBeamElement.EndForce(a,ua);if(Near(qa[1],0)&&Near(qa[3],0))ok++;
  if(Near(qa[0],30,1e-6)&&Near(qa[2],30,1e-6))ok++;
  var b=ReleasedBeamElement.Build(L,EI,w,true,false);var ub=ReleasedBeamElement.Recover(b,new[]{0.0,0.0,0.0});var qb=ReleasedBeamElement.EndForce(b,ub);if(Near(qb[1],0))ok++;
  var c=ReleasedBeamElement.Build(L,EI,w,false,false);var qc=ReleasedBeamElement.EndForce(c,new double[4]);if(Near(Math.Abs(qc[1]),w*L*L/12,1e-6)&&Near(Math.Abs(qc[3]),w*L*L/12,1e-6))ok++;
  ed.WriteMessage($"\nLiberações V2: {ok}/{total}. Verifica momentos liberados, reações de viga biapoiada e fixed-end moments.");
 }
 static bool Near(double a,double b,double t=1e-8)=>Math.Abs(a-b)<=t;
}
