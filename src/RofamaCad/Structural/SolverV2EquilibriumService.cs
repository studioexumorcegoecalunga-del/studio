namespace RofamaCad.Structural;
internal static class SolverV2EquilibriumService
{
 public sealed record Audit(double AppliedVertical,double ReactionVertical,double ResidualVertical,double AppliedMomentAboutStart,double ReactionMomentAboutStart,double ResidualMoment);
 public static Audit Check(double[] spans,double[] w,IReadOnlyList<UnifiedBeamSolverV2.PointLoad> points,double[] reactions){
  double applied=0,ma=0,x0=0;for(int e=0;e<spans.Length;e++){double W=w[e]*spans[e];double xc=x0+spans[e]/2;applied+=W;ma+=W*xc;x0+=spans[e];}x0=0;foreach(var p in points){double x=spans.Take(p.Element).Sum()+p.X;applied+=p.P;ma+=p.P*x;}double rr=0,mr=0;double xnode=0;for(int i=0;i<spans.Length+1;i++){double rv=reactions[2*i];double rm=reactions[2*i+1];rr+=rv;mr+=rv*xnode+rm;if(i<spans.Length)xnode+=spans[i];}return new(applied,rr,rr-applied,ma,mr,mr-ma);
 }
}
