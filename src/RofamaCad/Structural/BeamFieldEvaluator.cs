namespace RofamaCad.Structural;
internal static class BeamFieldEvaluator
{
 public sealed record Value(double X,double V,double M,double D);
 public static Value At(double[] spans,double[] w,double[] u,double[] endForce,IReadOnlyList<BeamPostProcessor.PointLoad> points,double globalX){
  double acc=0;int e=spans.Length-1;for(int i=0;i<spans.Length;i++){if(globalX<=acc+spans[i]+1e-9){e=i;break;}acc+=spans[i];}
  double L=spans[e],x=Math.Clamp(globalX-acc,0,L),t=L>0?x/L:0;double v0=u[2*e],r0=u[2*e+1],v1=u[2*e+2],r1=u[2*e+3];double n1=1-3*t*t+2*t*t*t,n2=L*(t-2*t*t+t*t*t),n3=3*t*t-2*t*t*t,n4=L*(-t*t+t*t*t);double def=n1*v0+n2*r0+n3*v1+n4*r1;double V=endForce[4*e]-w[e]*x;double M=endForce[4*e+1]+endForce[4*e]*x-w[e]*x*x/2;foreach(var p in points)if(p.Element==e&&p.X<=x){V-=p.P;M-=p.P*(x-p.X);}return new(globalX,V,M,def);
 }
 public static List<double> Grid(double[] spans,IEnumerable<BeamPostProcessor.PointLoad> points,int divisionsPerSpan=100){
  var xs=new List<double>();double a=0;for(int e=0;e<spans.Length;e++){double L=spans[e];for(int i=0;i<=divisionsPerSpan;i++)xs.Add(a+L*i/divisionsPerSpan);foreach(var p in points.Where(p=>p.Element==e)){xs.Add(a+p.X);xs.Add(Math.Max(a,a+p.X-1e-7));xs.Add(Math.Min(a+L,a+p.X+1e-7));}a+=L;}return xs.OrderBy(x=>x).DistinctBy(x=>Math.Round(x,8)).ToList();
 }
}
