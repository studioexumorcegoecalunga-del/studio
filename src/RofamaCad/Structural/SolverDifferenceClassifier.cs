namespace RofamaCad.Structural;
internal static class SolverDifferenceClassifier
{
 public static string Classify(SolverComparisonService.Difference d,double tu,double tr,double te,bool hasRelease,bool hasPoint){
  if(double.IsInfinity(d.MaxU)||double.IsInfinity(d.MaxR)||double.IsInfinity(d.MaxE))return "DIMENSAO_RESULTADO";
  if(hasRelease&&(d.MaxE>te||d.MaxU>tu))return "REVISAR_LIBERACOES";
  if(hasPoint&&(d.MaxR>tr||d.MaxE>te))return "REVISAR_CARGA_PONTUAL";
  if(d.MaxU>tu&&d.MaxR<=tr&&d.MaxE<=te)return "DESLOCAMENTO";
  if(d.MaxR>tr||d.MaxE>te)return "EQUILIBRIO/SINAL";
  return "OK";
 }
}
