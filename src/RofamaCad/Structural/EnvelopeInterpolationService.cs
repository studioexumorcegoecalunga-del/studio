namespace RofamaCad.Structural;
internal static class EnvelopeInterpolationService
{
 public static BeamPostProcessor.SamplePoint At(IReadOnlyList<BeamPostProcessor.SamplePoint> s,double x){
  if(s.Count==0)throw new ArgumentException("Amostras vazias.");if(x<=s[0].GlobalX)return s[0];if(x>=s[^1].GlobalX)return s[^1];
  for(int i=0;i<s.Count-1;i++){var a=s[i];var b=s[i+1];if(x<a.GlobalX||x>b.GlobalX)continue;double dx=b.GlobalX-a.GlobalX;if(Math.Abs(dx)<1e-12)return Math.Abs(x-a.GlobalX)<=Math.Abs(x-b.GlobalX)?a:b;double t=(x-a.GlobalX)/dx;return new BeamPostProcessor.SamplePoint(a.Element,a.X+(b.X-a.X)*t,x,a.V+(b.V-a.V)*t,a.M+(b.M-a.M)*t,a.Deflection+(b.Deflection-a.Deflection)*t);}
  return s.MinBy(p=>Math.Abs(p.GlobalX-x))!;
 }
 public static List<double> CommonGrid(IReadOnlyList<ContinuousEnvelopeService.Curve> curves,int divisions=200){
  double end=curves.Min(c=>c.Samples[^1].GlobalX);var xs=Enumerable.Range(0,divisions+1).Select(i=>end*i/divisions).ToList();
  xs.AddRange(curves.SelectMany(c=>c.Samples.Select(s=>s.GlobalX)));return xs.OrderBy(x=>x).Aggregate(new List<double>(),(a,x)=>{if(a.Count==0||Math.Abs(a[^1]-x)>1e-8)a.Add(x);return a;});
 }
}
