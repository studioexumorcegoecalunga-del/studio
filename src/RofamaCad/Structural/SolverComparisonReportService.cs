using Autodesk.AutoCAD.DatabaseServices;
namespace RofamaCad.Structural;
internal static class SolverComparisonReportService
{
 public sealed record Row(string Beam,string Combination,double DU,double DR,double DE,string Classification,bool Pass);
 public static void Write(Database db,Transaction tr,IReadOnlyList<Row> rows){
  var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);const string key="ROFAMA_SOLVER_V1_V2_REPORT";Xrecord x;if(nod.Contains(key))x=(Xrecord)tr.GetObject(nod.GetAt(key),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt(key,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Int32,rows.Count)};foreach(var r in rows){a.Add(new((int)DxfCode.Text,r.Beam));a.Add(new((int)DxfCode.Text,r.Combination));a.Add(new((int)DxfCode.Real,r.DU));a.Add(new((int)DxfCode.Real,r.DR));a.Add(new((int)DxfCode.Real,r.DE));a.Add(new((int)DxfCode.Text,r.Classification));a.Add(new((int)DxfCode.Int16,r.Pass?1:0));}x.Data=new ResultBuffer(a.ToArray());
 }
 public static List<Row> Read(Database db,Transaction tr){var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);var r=new List<Row>();if(!nod.Contains("ROFAMA_SOLVER_V1_V2_REPORT"))return r;var a=((Xrecord)tr.GetObject(nod.GetAt("ROFAMA_SOLVER_V1_V2_REPORT"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length==0)return r;int n=Convert.ToInt32(a[0].Value);for(int i=0;i<n;i++){int j=1+7*i;if(j+6>=a.Length)break;r.Add(new(Convert.ToString(a[j].Value)??"",Convert.ToString(a[j+1].Value)??"",Convert.ToDouble(a[j+2].Value),Convert.ToDouble(a[j+3].Value),Convert.ToDouble(a[j+4].Value),Convert.ToString(a[j+5].Value)??"",Convert.ToInt32(a[j+6].Value)==1));}return r;}
}
