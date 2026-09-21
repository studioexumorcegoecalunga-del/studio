namespace RofamaCad.Structural;
internal static class DisconnectedDofService
{
 public sealed record Reduction(int[] Active,int[] Inactive);
 public static Reduction Detect(double[,] k,double[] f,double tol=1e-10){
  var active=new List<int>();var inactive=new List<int>();for(int i=0;i<f.Length;i++){double row=0;for(int j=0;j<f.Length;j++)row=Math.Max(row,Math.Abs(k[i,j]));if(row<=tol){if(Math.Abs(f[i])>tol)throw new InvalidOperationException($"Mecanismo: DOF {i} sem rigidez e com carga.");inactive.Add(i);}else active.Add(i);}return new(active.ToArray(),inactive.ToArray());
 }
}
