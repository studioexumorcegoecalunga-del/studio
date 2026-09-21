using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class GoverningEnvelopeReportCommands
{
 [CommandMethod("RFGOVERNANTEENVELOPE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;using var tr=d.Database.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(d.Database),OpenMode.ForRead);int seq=0;d.Editor.WriteMessage("\n--- GOVERNANTES DO ENVELOPE CONTÍNUO ---");
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_CONTINUOUS_ENVELOPE"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_CONTINUOUS_ENVELOPE"),OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<14)continue;int n=Convert.ToInt32(a[0].Value);var pts=new List<(double x,double v0,double v1,string vc0,string vc1,double m0,double m1,string mc0,string mc1,double d0,double d1,string dc0,string dc1)>();for(int i=0;i<n;i++){int j=1+13*i;pts.Add((Convert.ToDouble(a[j].Value),Convert.ToDouble(a[j+1].Value),Convert.ToDouble(a[j+2].Value),Convert.ToString(a[j+3].Value)??"",Convert.ToString(a[j+4].Value)??"",Convert.ToDouble(a[j+5].Value),Convert.ToDouble(a[j+6].Value),Convert.ToString(a[j+7].Value)??"",Convert.ToString(a[j+8].Value)??"",Convert.ToDouble(a[j+9].Value),Convert.ToDouble(a[j+10].Value),Convert.ToString(a[j+11].Value)??"",Convert.ToString(a[j+12].Value)??""));}var mv=pts.OrderByDescending(p=>Math.Max(Math.Abs(p.v0),Math.Abs(p.v1))).First();var mm=pts.OrderByDescending(p=>Math.Max(Math.Abs(p.m0),Math.Abs(p.m1))).First();var md=pts.OrderByDescending(p=>Math.Max(Math.Abs(p.d0),Math.Abs(p.d1))).First();string name=StructuralIdService.Get(b,tr,$"V{++seq:00}");d.Editor.WriteMessage($"\n{name}: Vgov={Pick(mv.v0,mv.v1,mv.vc0,mv.vc1)} @ {mv.x:0.###}m; Mgov={Pick(mm.m0,mm.m1,mm.mc0,mm.mc1)} @ {mm.x:0.###}m; vgov={PickD(md.d0,md.d1,md.dc0,md.dc1)} @ {md.x:0.###}m");}tr.Commit();
 }
 static string Pick(double a,double b,string ca,string cb)=>Math.Abs(a)>=Math.Abs(b)?$"{a:0.###} ({ca})":$"{b:0.###} ({cb})";
 static string PickD(double a,double b,string ca,string cb)=>Math.Abs(a)>=Math.Abs(b)?$"{a*1000:0.###}mm ({ca})":$"{b*1000:0.###}mm ({cb})";
}
