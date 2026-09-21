using Autodesk.AutoCAD.DatabaseServices;
namespace RofamaCad.Structural;
internal static class LoadCombinationService
{
 public sealed record Term(string Case,double Factor);
 static string San(string s)=>new string(s.ToUpperInvariant().Where(c=>char.IsLetterOrDigit(c)||c=='_').ToArray());
 public static void Write(Database db,Transaction tr,string name,IReadOnlyList<Term> terms){
  var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForWrite);string k="ROFAMA_COMB_"+San(name);Xrecord x;if(nod.Contains(k))x=(Xrecord)tr.GetObject(nod.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();nod.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}
  var a=new List<TypedValue>{new((int)DxfCode.Text,name),new((int)DxfCode.Int32,terms.Count)};foreach(var t in terms){a.Add(new((int)DxfCode.Text,t.Case));a.Add(new((int)DxfCode.Real,t.Factor));}x.Data=new ResultBuffer(a.ToArray());
 }
 public static List<Term>? Read(Database db,Transaction tr,string name){
  var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);string k="ROFAMA_COMB_"+San(name);if(!nod.Contains(k))return null;var a=((Xrecord)tr.GetObject(nod.GetAt(k),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<2)return null;int n=Convert.ToInt32(a[1].Value);var r=new List<Term>();for(int i=0;i<n&&2+2*i+1<a.Length;i++)r.Add(new(Convert.ToString(a[2+2*i].Value)??"",Convert.ToDouble(a[3+2*i].Value)));return r;
 }
}
