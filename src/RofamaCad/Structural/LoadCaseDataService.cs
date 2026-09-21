using Autodesk.AutoCAD.DatabaseServices;
namespace RofamaCad.Structural;
internal static class LoadCaseDataService
{
 static string Key(string prefix,string loadCase)=>$"{prefix}_{San(loadCase)}";
 static string San(string s)=>new string(s.ToUpperInvariant().Where(c=>char.IsLetterOrDigit(c)||c=='_').ToArray());
 public static void WriteSpans(Entity e,Transaction tr,string lc,IReadOnlyList<double> q)=>Write(e,tr,Key("ROFAMA_SPAN_LOADS_CASE",lc),q.Select(v=>new TypedValue((int)DxfCode.Real,v)).ToArray());
 public static double[]? ReadSpans(Entity e,Transaction tr,string lc){var a=Read(e,tr,Key("ROFAMA_SPAN_LOADS_CASE",lc));return a?.Select(x=>Convert.ToDouble(x.Value)).ToArray();}
 public static void WritePoints(Entity e,Transaction tr,string lc,IReadOnlyList<UnifiedBeamSolver.PointLoad> p){var a=new List<TypedValue>();foreach(var z in p){a.Add(new((int)DxfCode.Int16,z.Element));a.Add(new((int)DxfCode.Real,z.X));a.Add(new((int)DxfCode.Real,z.P));}Write(e,tr,Key("ROFAMA_POINT_LOADS_CASE",lc),a.ToArray());}
 public static List<UnifiedBeamSolver.PointLoad> ReadPoints(Entity e,Transaction tr,string lc){var r=new List<UnifiedBeamSolver.PointLoad>();var a=Read(e,tr,Key("ROFAMA_POINT_LOADS_CASE",lc));if(a!=null)for(int i=0;i+2<a.Length;i+=3)r.Add(new(Convert.ToInt32(a[i].Value),Convert.ToDouble(a[i+1].Value),Convert.ToDouble(a[i+2].Value)));return r;}
 static TypedValue[]? Read(Entity e,Transaction tr,string k){if(e.ExtensionDictionary.IsNull)return null;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains(k))return null;return ((Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForRead)).Data?.AsArray();}
 static void Write(Entity e,Transaction tr,string k,TypedValue[] a){e.UpgradeOpen();if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(a);}
}
