namespace RofamaCad.Structural;
internal static class ReleasedBeamElement
{
 public sealed record Condensed(double[,] K,double[] F,double[,] FullK,double[] FullF,int[] Retained,int[] Released);
 public static Condensed Build(double L,double EI,double w,bool releaseStart,bool releaseEnd){
  var k=FullK(L,EI);var f=new[]{-w*L/2,-w*L*L/12,-w*L/2,w*L*L/12};var rel=new List<int>();if(releaseStart)rel.Add(1);if(releaseEnd)rel.Add(3);if(rel.Count==0)return new(k,(double[])f.Clone(),k,(double[])f.Clone(),new[]{0,1,2,3},Array.Empty<int>());
  var ret=Enumerable.Range(0,4).Except(rel).ToArray();var khh=Sub(k,rel.ToArray(),rel.ToArray());var inv=Inverse(khh);var krh=Sub(k,ret,rel.ToArray());var khr=Sub(k,rel.ToArray(),ret);var krr=Sub(k,ret,ret);var fr=ret.Select(i=>f[i]).ToArray();var fh=rel.Select(i=>f[i]).ToArray();var kc=Subtract(krr,Mul(Mul(krh,inv),khr));var fc=Subtract(fr,Mul(Mul(krh,inv),fh));return new(kc,fc,k,f,ret,rel.ToArray());
 }
 public static double[] Recover(Condensed c,double[] retainedU){
  var full=new double[4];for(int i=0;i<c.Retained.Length;i++)full[c.Retained[i]]=retainedU[i];if(c.Released.Length>0){var khh=Sub(c.FullK,c.Released,c.Released);var khr=Sub(c.FullK,c.Released,c.Retained);var fh=c.Released.Select(i=>c.FullF[i]).ToArray();var uh=Mul(Inverse(khh),Subtract(fh,Mul(khr,retainedU)));for(int i=0;i<c.Released.Length;i++)full[c.Released[i]]=uh[i];}return full;
 }
 public static double[] EndForce(Condensed c,double[] fullU)=>Subtract(Mul(c.FullK,fullU),c.FullF);
 static double[,] FullK(double L,double EI){double a=EI/(L*L*L);return new[,]{{12*a,6*L*a,-12*a,6*L*a},{6*L*a,4*L*L*a,-6*L*a,2*L*L*a},{-12*a,-6*L*a,12*a,-6*L*a},{6*L*a,2*L*L*a,-6*L*a,4*L*L*a}};}
 static double[,] Sub(double[,] a,int[] r,int[] c){var z=new double[r.Length,c.Length];for(int i=0;i<r.Length;i++)for(int j=0;j<c.Length;j++)z[i,j]=a[r[i],c[j]];return z;}
 static double[] Mul(double[,] a,double[] x){var z=new double[a.GetLength(0)];for(int i=0;i<z.Length;i++)for(int j=0;j<x.Length;j++)z[i]+=a[i,j]*x[j];return z;}
 static double[,] Mul(double[,] a,double[,] b){var z=new double[a.GetLength(0),b.GetLength(1)];for(int i=0;i<z.GetLength(0);i++)for(int j=0;j<z.GetLength(1);j++)for(int k=0;k<a.GetLength(1);k++)z[i,j]+=a[i,k]*b[k,j];return z;}
 static double[] Subtract(double[] a,double[] b)=>a.Zip(b,(x,y)=>x-y).ToArray();static double[,] Subtract(double[,] a,double[,] b){var z=(double[,])a.Clone();for(int i=0;i<z.GetLength(0);i++)for(int j=0;j<z.GetLength(1);j++)z[i,j]-=b[i,j];return z;}
 static double[,] Inverse(double[,] a){int n=a.GetLength(0);var x=new double[n,2*n];for(int i=0;i<n;i++){for(int j=0;j<n;j++)x[i,j]=a[i,j];x[i,n+i]=1;}for(int k=0;k<n;k++){int p=k;for(int i=k+1;i<n;i++)if(Math.Abs(x[i,k])>Math.Abs(x[p,k]))p=i;if(Math.Abs(x[p,k])<1e-12)throw new InvalidOperationException("Condensação singular.");if(p!=k)for(int j=0;j<2*n;j++)(x[k,j],x[p,j])=(x[p,j],x[k,j]);double d=x[k,k];for(int j=0;j<2*n;j++)x[k,j]/=d;for(int i=0;i<n;i++)if(i!=k){double m=x[i,k];for(int j=0;j<2*n;j++)x[i,j]-=m*x[k,j];}}var r=new double[n,n];for(int i=0;i<n;i++)for(int j=0;j<n;j++)r[i,j]=x[i,n+j];return r;}
}
