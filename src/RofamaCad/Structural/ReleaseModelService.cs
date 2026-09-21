namespace RofamaCad.Structural;
internal static class ReleaseModelService
{
 public sealed record Release(int Element,bool StartMoment,bool EndMoment);
 public static List<Release> Parse(Autodesk.AutoCAD.DatabaseServices.Entity e,Autodesk.AutoCAD.DatabaseServices.Transaction tr){
  var r=new List<Release>();if(e.ExtensionDictionary.IsNull)return r;var d=(Autodesk.AutoCAD.DatabaseServices.DBDictionary)tr.GetObject(e.ExtensionDictionary,Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);if(!d.Contains("ROFAMA_RELEASES"))return r;var a=((Autodesk.AutoCAD.DatabaseServices.Xrecord)tr.GetObject(d.GetAt("ROFAMA_RELEASES"),Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)).Data?.AsArray();if(a==null)return r;for(int i=0;i+2<a.Length;i+=3)r.Add(new(Convert.ToInt32(a[i].Value),Convert.ToInt32(a[i+1].Value)!=0,Convert.ToInt32(a[i+2].Value)!=0));return r;
 }
 public static bool[,] Flags(int elements,IEnumerable<Release> releases){var f=new bool[elements,2];foreach(var r in releases)if(r.Element>=0&&r.Element<elements){f[r.Element,0]|=r.StartMoment;f[r.Element,1]|=r.EndMoment;}return f;}
}
