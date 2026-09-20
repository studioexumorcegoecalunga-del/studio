using Autodesk.AutoCAD.DatabaseServices;

namespace RofamaCad.Core;
public static class StoreyService
{
 const string Storeys="ROFAMA_STOREYS",Active="ROFAMA_ACTIVE_STOREY";
 public static (string Id,string Name,double Elevation,double Height)? GetActive(Transaction tr,Database db){
  var nod=(DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId,OpenMode.ForRead);if(!nod.Contains(Active)||!nod.Contains(Storeys))return null;
  var ar=(Xrecord)tr.GetObject(nod.GetAt(Active),OpenMode.ForRead);var av=ar.Data?.AsArray();if(av==null||av.Length<2)return null;var id=av[0].Value?.ToString()??"";
  var sd=(DBDictionary)tr.GetObject(nod.GetAt(Storeys),OpenMode.ForRead);if(!sd.Contains(id))return null;var sr=(Xrecord)tr.GetObject(sd.GetAt(id),OpenMode.ForRead);var a=sr.Data?.AsArray();if(a==null||a.Length<3)return null;
  return(id,a[0].Value?.ToString()??"",Convert.ToDouble(a[1].Value),Convert.ToDouble(a[2].Value));
 }
 public static void Tag(Entity e,Transaction tr,Database db){
  var s=GetActive(tr,db);if(s==null)return;if(e.ExtensionDictionary.IsNull)e.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);
  if(d.Contains("ROFAMA_STOREY")){var old=(Xrecord)tr.GetObject(d.GetAt("ROFAMA_STOREY"),OpenMode.ForWrite);old.Data=Data(s.Value);}
  else{var x=new Xrecord{Data=Data(s.Value)};d.SetAt("ROFAMA_STOREY",x);tr.AddNewlyCreatedDBObject(x,true);}
 }
 static ResultBuffer Data((string Id,string Name,double Elevation,double Height)s)=>new(new TypedValue((int)DxfCode.Text,s.Id),new TypedValue((int)DxfCode.Text,s.Name),new TypedValue((int)DxfCode.Real,s.Elevation),new TypedValue((int)DxfCode.Real,s.Height));
}
