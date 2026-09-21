using Autodesk.AutoCAD.DatabaseServices;
namespace RofamaCad.Structural;
internal static class StructuralIdService
{
 public static string Get(Entity e,Transaction tr,string fallback){
  if(e.ExtensionDictionary.IsNull)return fallback;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_STRUCT_ID"))return fallback;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_STRUCT_ID"),OpenMode.ForRead)).Data?.AsArray();return a?.Length>0?Convert.ToString(a[0].Value)??fallback:fallback;
 }
}
