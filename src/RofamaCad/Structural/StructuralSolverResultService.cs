using Autodesk.AutoCAD.DatabaseServices;
namespace RofamaCad.Structural;
internal static class StructuralSolverResultService
{
 public sealed record Stored(string Name,string Solver,double[] U,double[] R,double[] E);
 public static void Write(Entity e,Transaction tr,string key,string name,string solver,double[] u,double[] r,double[] ef){
  if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(key))x=(Xrecord)tr.GetObject(d.GetAt(key),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(key,x);tr.AddNewlyCreatedDBObject(x,true);}var a=new List<TypedValue>{new((int)DxfCode.Text,name),new((int)DxfCode.Text,solver),new((int)DxfCode.Int32,u.Length)};a.AddRange(u.Select(v=>new TypedValue((int)DxfCode.Real,v)));a.AddRange(r.Select(v=>new TypedValue((int)DxfCode.Real,v)));a.AddRange(ef.Select(v=>new TypedValue((int)DxfCode.Real,v)));x.Data=new ResultBuffer(a.ToArray());
 }
 public static Stored? Read(Entity e,Transaction tr,string key){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(key))return null;var a=((Xrecord)tr.GetObject(d.GetAt(key),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<3)return null;string name=Convert.ToString(a[0].Value)??"",solver=Convert.ToString(a[1].Value)??"";int n=Convert.ToInt32(a[2].Value);if(a.Length<3+2*n)return null;return new(name,solver,a.Skip(3).Take(n).Select(x=>Convert.ToDouble(x.Value)).ToArray(),a.Skip(3+n).Take(n).Select(x=>Convert.ToDouble(x.Value)).ToArray(),a.Skip(3+2*n).Select(x=>Convert.ToDouble(x.Value)).ToArray());}
}
