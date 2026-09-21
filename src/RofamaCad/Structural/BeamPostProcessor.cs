namespace RofamaCad.Structural;
internal static class BeamPostProcessor
{
 public sealed record PointLoad(int Element,double X,double P);
 public sealed record Sample(int Element,double X,double GlobalX,double V,double M,double Deflection);
 public static List<Sample> Sample(double[] L,double[] EI,double[] W,double[] displacement,double[] endForce,IReadOnlyList<PointLoad> points,int divisions=40){
  var r=new List<Sample>();double gx=0;
  for(int e=0;e<L.Length;e++){double l=L[e],v0=displacement[2*e],t0=displacement[2*e+1],v1=displacement[2*e+2],t1=displacement[2*e+3];double shear0=endForce[4*e],moment0=endForce[4*e+1];var xs=Enumerable.Range(0,divisions+1).Select(i=>l*i/(double)divisions).Concat(points.Where(p=>p.Element==e).SelectMany(p=>new[]{Math.Max(0,p.X-1e-7),Math.Min(l,p.X+1e-7)})).Distinct().OrderBy(x=>x);
   foreach(double x in xs){double V=shear0-W[e]*x-points.Where(p=>p.Element==e&&p.X<=x).Sum(p=>p.P);double M=moment0+shear0*x-W[e]*x*x/2-points.Where(p=>p.Element==e&&p.X<=x).Sum(p=>p.P*(x-p.X));double z=x/l,n1=1-3*z*z+2*z*z*z,n2=l*(z-2*z*z+z*z*z),n3=3*z*z-2*z*z*z,n4=l*(-z*z+z*z*z);double def=n1*v0+n2*t0+n3*v1+n4*t1;r.Add(new(e,x,gx+x,V,M,def));}gx+=l;
  }return r;
 }
}
