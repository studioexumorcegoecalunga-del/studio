using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class CombinationEnvelopeCommands
{
 [CommandMethod("RFENVELOPE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);int n=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);var results=new List<(string Name,double[] U,double[] R,double[] E)>();foreach(DBDictionaryEntry x in dic)if(x.Key.StartsWith("ROFAMA_COMB_RESULT_")){var a=((Xrecord)tr.GetObject(x.Value,OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<2)continue;string name=Convert.ToString(a[0].Value)??x.Key;int nd=Convert.ToInt32(a[1].Value);if(a.Length<2+2*nd)continue;var u=a.Skip(2).Take(nd).Select(z=>Convert.ToDouble(z.Value)).ToArray();var r=a.Skip(2+nd).Take(nd).Select(z=>Convert.ToDouble(z.Value)).ToArray();var e=a.Skip(2+2*nd).Select(z=>Convert.ToDouble(z.Value)).ToArray();results.Add((name,u,r,e));}if(results.Count==0)continue;double umax=results.SelectMany(x=>x.U.Where((_,i)=>i%2==0)).DefaultIfEmpty().Max();double umin=results.SelectMany(x=>x.U.Where((_,i)=>i%2==0)).DefaultIfEmpty().Min();double rmax=results.SelectMany(x=>x.R.Where((_,i)=>i%2==0)).DefaultIfEmpty().Max();double rmin=results.SelectMany(x=>x.R.Where((_,i)=>i%2==0)).DefaultIfEmpty().Min();double mmax=results.SelectMany(x=>x.E.Where((_,i)=>i%2==1)).DefaultIfEmpty().Max();double mmin=results.SelectMany(x=>x.E.Where((_,i)=>i%2==1)).DefaultIfEmpty().Min();Write(b,tr,new[]{umin,umax,rmin,rmax,mmin,mmax,results.Count});n++;}
  tr.Commit();d.Editor.WriteMessage($"\nEnvelope criado para {n} viga(s): deslocamento, reação e momento de extremidade.");
 }
 static void Write(Entity e,Transaction tr,double[] v){e.UpgradeOpen();var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForWrite);const string k="ROFAMA_COMB_ENVELOPE";Xrecord x;if(d.Contains(k))x=(Xrecord)tr.GetObject(d.GetAt(k),OpenMode.ForWrite);else{x=new Xrecord();d.SetAt(k,x);tr.AddNewlyCreatedDBObject(x,true);}x.Data=new ResultBuffer(v.Select(z=>new TypedValue((int)DxfCode.Real,z)).ToArray());}
}
