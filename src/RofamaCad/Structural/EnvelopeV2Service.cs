namespace RofamaCad.Structural;
internal static class EnvelopeV2Service
{
 public static List<ContinuousEnvelopeService.EnvelopePoint> Build(IReadOnlyList<ContinuousEnvelopeService.Curve> curves){
  var r=new List<ContinuousEnvelopeService.EnvelopePoint>();foreach(double x in EnvelopeInterpolationService.CommonGrid(curves)){var v=curves.Select(c=>(c.Name,S:EnvelopeInterpolationService.At(c.Samples,x))).ToList();var v0=v.MinBy(z=>z.S.V)!;var v1=v.MaxBy(z=>z.S.V)!;var m0=v.MinBy(z=>z.S.M)!;var m1=v.MaxBy(z=>z.S.M)!;var d0=v.MinBy(z=>z.S.Deflection)!;var d1=v.MaxBy(z=>z.S.Deflection)!;r.Add(new(x,v0.S.V,v1.S.V,v0.Name,v1.Name,m0.S.M,m1.S.M,m0.Name,m1.Name,d0.S.Deflection,d1.S.Deflection,d0.Name,d1.Name));}return r;
 }
}
