namespace RofamaCad.Structural;
internal static class FrameBeamSolver
{
 public sealed record Result(double[] Displacement,double[] Reaction,double[] EndForce);
 public static Result Solve(double[] L,double[] EI,double[] W,bool[] fixV,bool[] fixR){
  int ne=L.Length,nn=ne+1,nd=2*nn;if(EI.Length!=ne||W.Length!=ne||fixV.Length!=nn||fixR.Length!=nn)throw new ArgumentException("Dados incompatíveis.");
  var K=new double[nd,nd];var F=new double[nd];var fe=new double[4*ne];
  for(int e=0;e<ne;e++){double l=L[e],k=EI[e]/(l*l*l),w=W[e];double[,] a={{12*k,6*l*k,-12*k,6*l*k},{6*l*k,4*l*l*k,-6*l*k,2*l*l*k},{-12*k,-6*l*k,12*k,-6*l*k},{6*l*k,2*l*l*k,-6*l*k,4*l*l*k}};double[] q={-w*l/2,-w*l*l/12,-w*l/2,w*l*l/12};for(int i=0;i<4;i++){fe[4*e+i]=q[i];int gi=2*e+i;F[gi]+=q[i];for(int j=0;j<4;j++)K[gi,2*e+j]+=a[i,j];}}
  var fixedDofs=new HashSet<int>();for(int i=0;i<nn;i++){if(fixV[i])fixedDofs.Add(2*i);if(fixR[i])fixedDofs.Add(2*i+1);}var free=Enumerable.Range(0,nd).Where(i=>!fixedDofs.Contains(i)).ToArray();var Kr=new double[free.Length,free.Length];var Fr=new double[free.Length];for(int i=0;i<free.Length;i++){Fr[i]=F[free[i]];for(int j=0;j<free.Length;j++)Kr[i,j]=K[free[i],free[j]];}var ur=Gauss(Kr,Fr);var u=new double[nd];for(int i=0;i<free.Length;i++)u[free[i]]=ur[i];var reac=new double[nd];for(int i=0;i<nd;i++){double s=0;for(int j=0;j<nd;j++)s+=K[i,j]*u[j];reac[i]=s-F[i];}var ef=new double[4*ne];for(int e=0;e<ne;e++){double l=L[e],k=EI[e]/(l*l*l);double[,] a={{12*k,6*l*k,-12*k,6*l*k},{6*l*k,4*l*l*k,-6*l*k,2*l*l*k},{-12*k,-6*l*k,12*k,-6*l*k},{6*l*k,2*l*l*k,-6*l*k,4*l*l*k}};for(int i=0;i<4;i++){double s=0;for(int j=0;j<4;j++)s+=a[i,j]*u[2*e+j];ef[4*e+i]=s-fe[4*e+i];}}return new(u,reac,ef);
 }
 static double[] Gauss(double[,] a,double[] b){int n=b.Length;if(n==0)return Array.Empty<double>();var m=(double[,])a.Clone();var x=(double[])b.Clone();for(int k=0;k<n;k++){int p=k;for(int i=k+1;i<n;i++)if(Math.Abs(m[i,k])>Math.Abs(m[p,k]))p=i;if(Math.Abs(m[p,k])<1e-12)throw new InvalidOperationException("Matriz singular: revise os apoios.");if(p!=k){for(int j=k;j<n;j++){var t=m[k,j];m[k,j]=m[p,j];m[p,j]=t;}var tb=x[k];x[k]=x[p];x[p]=tb;}for(int i=k+1;i<n;i++){var f=m[i,k]/m[k,k];for(int j=k;j<n;j++)m[i,j]-=f*m[k,j];x[i]-=f*x[k];}}var r=new double[n];for(int i=n-1;i>=0;i--){var s=x[i];for(int j=i+1;j<n;j++)s-=m[i,j]*r[j];r[i]=s/m[i,i];}return r;}
}
