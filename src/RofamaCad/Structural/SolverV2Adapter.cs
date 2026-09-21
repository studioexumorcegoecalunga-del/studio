namespace RofamaCad.Structural;
internal static class SolverV2Adapter
{
 public static UnifiedBeamSolverV2.Result Solve(double[] spans,double[] ei,double[] q,bool[] fixV,bool[] fixR,ReleaseModelService.ReleaseFlags[] releases,IReadOnlyList<UnifiedBeamSolver.PointLoad> points){
  var rel=releases.Select(x=>new UnifiedBeamSolverV2.Release(x.Start,x.End)).ToArray();var p=points.Select(x=>new UnifiedBeamSolverV2.PointLoad(x.Element,x.X,x.P)).ToArray();return UnifiedBeamSolverV2.Solve(spans,ei,q,fixV,fixR,rel,p);
 }
}
