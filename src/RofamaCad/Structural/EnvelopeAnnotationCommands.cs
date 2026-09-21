using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class EnvelopeAnnotationCommands
{
 [CommandMethod("RFANOTARENVELOPE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;using var tr=db.TransactionManager.StartTransaction();Ensure(tr,db);var cfg=DiagramSettingsCommands.Read(db,tr);var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var ids=ms.Cast<ObjectId>().ToArray();int n=0;
  foreach(var id in ids){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_CONTINUOUS_ENVELOPE"))continue;var pts=Read(dic,tr);if(pts.Count==0)continue;var mv=pts.OrderByDescending(p=>Math.Max(Math.Abs(p.VMin),Math.Abs(p.VMax))).First();var mm=pts.OrderByDescending(p=>Math.Max(Math.Abs(p.MMin),Math.Abs(p.MMax))).First();var md=pts.OrderByDescending(p=>Math.Max(Math.Abs(p.DMin),Math.Abs(p.DMax))).First();Add(ms,tr,b,mv.X,$"V {Pick(mv.VMin,mv.VMax,mv.VMinCase,mv.VMaxCase)} kN",cfg.TextHeight,.25);Add(ms,tr,b,mm.X,$"M {Pick(mm.MMin,mm.MMax,mm.MMinCase,mm.MMaxCase)} kN.m",cfg.TextHeight,.45);Add(ms,tr,b,md.X,$"v {PickD(md.DMin,md.DMax,md.DMinCase,md.DMaxCase)}",cfg.TextHeight,.65);n++;}
  tr.Commit();d.Editor.WriteMessage($"\nEnvelope anotado em {n} viga(s).");
 }
 static string Pick(double a,double b,string ca,string cb)=>Math.Abs(a)>=Math.Abs(b)?$"{a:0.##} [{ca}]":$"{b:0.##} [{cb}]";
 static string PickD(double a,double b,string ca,string cb)=>Math.Abs(a)>=Math.Abs(b)?$"{a*1000:0.##} mm [{ca}]":$"{b*1000:0.##} mm [{cb}]";
 static void Add(BlockTableRecord ms,Transaction tr,Line b,double x,string s,double h,double off){var dir=(b.EndPoint-b.StartPoint).GetNormal();var normal=new Vector3d(-dir.Y,dir.X,0);var t=new DBText{Position=b.StartPoint+dir*x+normal*off,Height=h,TextString=s,Layer="RF-EST-ENV-TEXTO"};ms.AppendEntity(t);tr.AddNewlyCreatedDBObject(t,true);}
 static List<ContinuousEnvelopeService.EnvelopePoint> Read(DBDictionary d,Transaction tr){var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_CONTINUOUS_ENVELOPE"),OpenMode.ForRead)).Data?.AsArray();var r=new List<ContinuousEnvelopeService.EnvelopePoint>();if(a==null)return r;int n=Convert.ToInt32(a[0].Value);for(int i=0;i<n;i++){int j=1+13*i;r.Add(new(Convert.ToDouble(a[j].Value),Convert.ToDouble(a[j+1].Value),Convert.ToDouble(a[j+2].Value),Convert.ToString(a[j+3].Value)??"",Convert.ToString(a[j+4].Value)??"",Convert.ToDouble(a[j+5].Value),Convert.ToDouble(a[j+6].Value),Convert.ToString(a[j+7].Value)??"",Convert.ToString(a[j+8].Value)??"",Convert.ToDouble(a[j+9].Value),Convert.ToDouble(a[j+10].Value),Convert.ToString(a[j+11].Value)??"",Convert.ToString(a[j+12].Value)??""));}return r;}
 static void Ensure(Transaction tr,Database db){var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);if(lt.Has("RF-EST-ENV-TEXTO"))return;lt.UpgradeOpen();var l=new LayerTableRecord{Name="RF-EST-ENV-TEXTO"};lt.Add(l);tr.AddNewlyCreatedDBObject(l,true);}
}
