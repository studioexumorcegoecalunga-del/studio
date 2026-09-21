namespace RofamaCad.Structural;
internal static class ContinuousEnvelopeService
{
 public sealed record Curve(string Name,List<BeamPostProcessor.Sample> Samples);
 public sealed record EnvelopePoint(double X,double VMin,double VMax,string VMinCase,string VMaxCase,double MMin,double MMax,string MMinCase,string MMaxCase,double DMin,double DMax,string DMinCase,string DMaxCase);
 public static List<EnvelopePoint> Build(IReadOnlyList<Curve> curves,double tol=1e-6){
  var xs=curves.SelectMany(c=>c.Samples.Select(s=>s.GlobalX)).OrderBy(x=>x).Aggregate(new List<double>(),(a,x)=>{if(a.Count==0||Math.Abs(a[^1]-x)>tol)a.Add(x);return a;});var r=new List<EnvelopePoint>();
  foreach(var x in xs){var vals=curves.Select(c=>(c.Name,S:Nearest(c.Samples,x))).ToList();var vmin=vals.MinBy(z=>z.S.V);var vmax=vals.MaxBy(z=>z.S.V);var mmin=vals.MinBy(z=>z.S.M);var mmax=vals.MaxBy(z=>z.S.M);var dmin=vals.MinBy(z=>z.S.Deflection);var dmax=vals.MaxBy(z=>z.S.Deflection);r.Add(new(x,vmin.S.V,vmax.S.V,vmin.Name,vmax.Name,mmin.S.M,mmax.S.M,mmin.Name,mmax.Name,dmin.S.Deflection,dmax.S.Deflection,dmin.Name,dmax.Name));}return r;
 }
 static BeamPostProcessor.Sample Nearest(List<BeamPostProcessor.Sample> s,double x)=>s.MinBy(p=>Math.Abs(p.GlobalX-x))!;
}
