namespace RofamaCad.Structural;
internal static class SolverComparisonService
{
 public sealed record Difference(double MaxU,double MaxR,double MaxE,int UIndex,int RIndex,int EIndex);
 public static Difference Compare(double[] u1,double[] r1,double[] e1,double[] u2,double[] r2,double[] e2){
  var u=Diff(u1,u2);var r=Diff(r1,r2);var e=Diff(e1,e2);return new(u.v,r.v,e.v,u.i,r.i,e.i);
 }
 static(double v,int i) Diff(double[] a,double[] b){int n=Math.Min(a.Length,b.Length),ix=-1;double m=0;for(int i=0;i<n;i++){double d=Math.Abs(a[i]-b[i]);if(d>m){m=d;ix=i;}}if(a.Length!=b.Length)m=double.PositiveInfinity;return(m,ix);}
}
