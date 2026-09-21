using Autodesk.AutoCAD.DatabaseServices;
namespace RofamaCad.Structural;
internal static class GeneratedGraphicsService
{
 public static void Tag(Entity e,Transaction tr,string sourceHandle,string kind){if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_GENERATED";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,sourceHandle),new TypedValue((int)DxfCode.Text,kind));}
 public static int EraseFor(BlockTableRecord ms,Transaction tr,string sourceHandle){int n=0;foreach(ObjectId id in ms.Cast<ObjectId>().ToArray()){var e=tr.GetObject(id,OpenMode.ForRead) as Entity;if(e==null||e.ExtensionDictionary.IsNull)continue;var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);if(!d.Contains("ROFAMA_GENERATED"))continue;var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_GENERATED"),OpenMode.ForRead)).Data?.AsArray();if(a?.Length>=1&&Convert.ToString(a[0].Value)==sourceHandle){e.UpgradeOpen();e.Erase();n++;}}return n;}
}
