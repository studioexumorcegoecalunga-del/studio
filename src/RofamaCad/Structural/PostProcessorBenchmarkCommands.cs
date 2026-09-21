using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class PostProcessorBenchmarkCommands
{
 [CommandMethod("RFTESTARPOSPROCESSADOR")]
 public void Run(){
  var ed=AcApp.DocumentManager.MdiActiveDocument.Editor;int ok=0,total=3;
  var L=6.0;var s=new[]{L};var q=new[]{10.0};var u=new double[4];var ef=new[]{30.0,0.0,30.0,0.0};var a=BeamPostProcessor.Sample(s,new[]{1.0},q,u,ef,new List<BeamPostProcessor.PointLoad>(),120);var mid=a.MinBy(x=>Math.Abs(x.GlobalX-3));if(mid!=null&&Near(mid.M,45,.5))ok++;
  var p=new List<BeamPostProcessor.PointLoad>{new(0,3,20)};var b=BeamPostProcessor.Sample(s,new[]{1.0},new[]{0.0},u,new[]{10.0,0.0,10.0,0.0},p,120);var ml=b.MinBy(x=>Math.Abs(x.GlobalX-3));if(ml!=null&&Near(ml.M,30,.5))ok++;
  if(a.First().V>29.9&&a.Last().V<-29.9)ok++;
  ed.WriteMessage($"\nPós-processador: {ok}/{total} verificações de referência aprovadas. Valores esperados: UDL Mmid≈45, P central Mmid≈30, V: +30→-30.");}
 static bool Near(double a,double b,double tol)=>Math.Abs(a-b)<=tol;
}
