namespace RofamaCad.Structural;
internal static class BeamStiffnessSolver
{
 public sealed record Result(double[] Rotation,double[] Reaction,double[] EndMoment);
 public static Result Solve(double[] L,double[] EI,double[] W){
  int n=L.Length+1;if(EI.Length!=L.Length||W.Length!=L.Length)throw new ArgumentException("Arrays incompatíveis.");
  var K=new double[n,n];var F=new double[n];var fixedEnd=new double[2*L.Length];
  for(int e=0;e<L.Length;e++){double l=L[e],k=EI[e]/l,m=W[e]*l*l/12.0;K[e,e]+=4*k;K[e,e+1]+=2*k;K[e+1,e]+=2*k;K[e+1,e+1]+=4*k;fixedEnd[2*e]=-m;fixedEnd[2*e+1]=m;F[e]-=fixedEnd[2*e];F[e+1]-=fixedEnd[2*e+1];}
  var theta=Gauss(K,F);var end=new double[2*L.Length];var reaction=new double[n];
  for(int e=0;e<L.Length;e++){double l=L[e],k=EI[e]/l;double ml=4*k*theta[e]+2*k*theta[e+1]+fixedEnd[2*e];double mr=2*k*theta[e]+4*k*theta[e+1]+fixedEnd[2*e+1];end[2*e]=ml;end[2*e+1]=mr;double rl=W[e]*l/2+(ml+mr)/l,rr=W[e]*l-rl;reaction[e]+=rl;reaction[e+1]+=rr;}
  return new(theta,reaction,end);
 }
 static double[] Gauss(double[,] a,double[] b){int n=b.Length;var m=(double[,])a.Clone();var x=(double[])b.Clone();for(int k=0;k<n;k++){int p=k;for(int i=k+1;i<n;i++)if(Math.Abs(m[i,k])>Math.Abs(m[p,k]))p=i;if(Math.Abs(m[p,k])<1e-12)continue;if(p!=k){for(int j=k;j<n;j++){var t=m[k,j];m[k,j]=m[p,j];m[p,j]=t;}var tb=x[k];x[k]=x[p];x[p]=tb;}for(int i=k+1;i<n;i++){var f=m[i,k]/m[k,k];for(int j=k;j<n;j++)m[i,j]-=f*m[k,j];x[i]-=f*x[k];}}var r=new double[n];for(int i=n-1;i>=0;i--){var s=x[i];for(int j=i+1;j<n;j++)s-=m[i,j]*r[j];r[i]=Math.Abs(m[i,i])<1e-12?0:s/m[i,i];}return r;}
}
