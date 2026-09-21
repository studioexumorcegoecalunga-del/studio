namespace RofamaCad.Structural;
internal static class ReleasedBeamSolver
{
 public sealed record Result(double[] Displacement,double[] Reaction,double[] EndForce);
 public static Result Solve(double[] L,double[] EI,double[] W,bool[] fixV,bool[] fixR,bool[,] rel){
  int ne=L.Length,nn=ne+1,nd=2*nn;if(rel.GetLength(0)!=ne)throw new ArgumentException("Liberações incompatíveis.");
  var K=new double[nd,nd];var F=new double[nd];var localK=new double[ne][,];var localF=new double[ne][];
  for(int e=0;e<ne;e++){var k=K0(L[e],EI[e]);var f=new[]{-W[e]*L[e]/2,-W[e]*L[e]*L[e]/12,-W[e]*L[e]/2,W[e]*L[e]*L[e]/12};Condense(k,f,rel[e,0],rel[e,1]);localK[e]=k;localF[e]=f;for(int i=0;i<4;i++){F[2*e+i]+=f[i];for(int j=0;j<4;j++)K[2*e+i,2*e+j]+=k[i,j];}}
  var fixedDofs=new HashSet<int>();for(int i=0;i<nn;i++){if(fixV[i])fixedDofs.Add(2*i);if(fixR[i])fixedDofs.Add(2*i+1);}var free=Enumerable.Range(0,nd).Where(i=>!fixedDofs.Contains(i)).ToArray();var Kr=new double[free.Length,free.Length];var Fr=new double[free.Length];for(int i=0;i<free.Length;i++){Fr[i]=F[free[i]];for(int j=0;j<free.Length;j++)Kr[i,j]=K[free[i],free[j]];}var ur=Gauss(Kr,Fr);var u=new double[nd];for(int i=0;i<free.Length;i++)u[free[i]]=ur[i];var R=new double[nd];for(int i=0;i<nd;i++){for(int j=0;j<nd;j++)R[i]+=K[i,j]*u[j];R[i]-=F[i];}var ef=new double[4*ne];for(int e=0;e<ne;e++)for(int i=0;i<4;i++){for(int j=0;j<4;j++)ef[4*e+i]+=localK[e][i,j]*u[2*e+j];ef[4*e+i]-=localF[e][i];}return new(u,R,ef);
 }
 static double[,] K0(double l,double ei){double k=ei/Math.Pow(l,3);return new double[,]{{12*k,6*l*k,-12*k,6*l*k},{6*l*k,4*l*l*k,-6*l*k,2*l*l*k},{-12*k,-6*l*k,12*k,-6*l*k},{6*l*k,2*l*l*k,-6*l*k,4*l*l*k}};}
 static void Condense(double[,] k,double[] f,bool rs,bool re){var q=new List<int>();if(rs)q.Add(1);if(re)q.Add(3);foreach(int r in q){double d=k[r,r];if(Math.Abs(d)<1e-14)continue;for(int i=0;i<4;i++)if(i!=r){double a=k[i,r]/d;f[i]-=a*f[r];for(int j=0;j<4;j++)if(j!=r)k[i,j]-=a*k[r,j];}for(int i=0;i<4;i++){k[i,r]=0;k[r,i]=0;}f[r]=0;}}
 static double[] Gauss(double[,] a,double[] b){int n=b.Length;if(n==0)return Array.Empty<double>();var m=(double[,])a.Clone();var x=(double[])b.Clone();for(int k=0;k<n;k++){int p=k;for(int i=k+1;i<n;i++)if(Math.Abs(m[i,k])>Math.Abs(m[p,k]))p=i;if(Math.Abs(m[p,k])<1e-12)throw new InvalidOperationException("Mecanismo/instabilidade detectado: matriz singular.");if(p!=k){for(int j=k;j<n;j++){var t=m[k,j];m[k,j]=m[p,j];m[p,j]=t;}var tb=x[k];x[k]=x[p];x[p]=tb;}for(int i=k+1;i<n;i++){var f=m[i,k]/m[k,k];for(int j=k;j<n;j++)m[i,j]-=f*m[k,j];x[i]-=f*x[k];}}var r=new double[n];for(int i=n-1;i>=0;i--){var s=x[i];for(int j=i+1;j<n;j++)s-=m[i,j]*r[j];r[i]=s/m[i,i];}return r;}
}
