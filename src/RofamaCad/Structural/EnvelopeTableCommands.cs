using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class EnvelopeTableCommands
{
 [CommandMethod("RFTABELAENVELOPE")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var p=d.Editor.GetPoint("\nPonto da tabela de envelopes: ");if(p.Status!=PromptStatus.OK)return;using var tr=d.Database.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(d.Database),OpenMode.ForWrite);var rows=new List<string[]>();int seq=0;
  foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_CONTINUOUS_ENVELOPE"))continue;var pts=Read(dic,tr);if(pts.Count==0)continue;var v=pts.OrderByDescending(x=>Math.Max(Math.Abs(x.VMin),Math.Abs(x.VMax))).First();var m=pts.OrderByDescending(x=>Math.Max(Math.Abs(x.MMin),Math.Abs(x.MMax))).First();var z=pts.OrderByDescending(x=>Math.Max(Math.Abs(x.DMin),Math.Abs(x.DMax))).First();rows.Add(new[]{StructuralIdService.Get(b,tr,$"V{++seq:00}"),Pick(v.VMin,v.VMax),Case(v.VMin,v.VMax,v.VMinCase,v.VMaxCase),Pick(m.MMin,m.MMax),Case(m.MMin,m.MMax,m.MMinCase,m.MMaxCase),(1000*Abs(z.DMin,z.DMax)).ToString("0.##"),Case(z.DMin,z.DMax,z.DMinCase,z.DMaxCase)});}
  var tb=new Table();tb.SetSize(rows.Count+2,7);tb.Position=p.Value;tb.Cells[0,0].TextString="ROFAMA - ENVELOPE CONTÍNUO";tb.MergeCells(CellRange.Create(tb,0,0,0,6));string[] h={"Viga","|V|max kN","Comb V","|M|max kN.m","Comb M","|v|max mm","Comb v"};for(int j=0;j<h.Length;j++)tb.Cells[1,j].TextString=h[j];for(int i=0;i<rows.Count;i++)for(int j=0;j<7;j++)tb.Cells[i+2,j].TextString=rows[i][j];tb.GenerateLayout();ms.AppendEntity(tb);tr.AddNewlyCreatedDBObject(tb,true);tr.Commit();d.Editor.WriteMessage($"\nTabela de envelope criada com {rows.Count} viga(s).");
 }
 static double Abs(double a,double b)=>Math.Abs(a)>=Math.Abs(b)?a:b;static string Pick(double a,double b)=>Abs(a,b).ToString("0.##");static string Case(double a,double b,string ca,string cb)=>Math.Abs(a)>=Math.Abs(b)?ca:cb;
 static List<ContinuousEnvelopeService.EnvelopePoint> Read(DBDictionary d,Transaction tr){var a=((Xrecord)tr.GetObject(d.GetAt("ROFAMA_CONTINUOUS_ENVELOPE"),OpenMode.ForRead)).Data?.AsArray();var r=new List<ContinuousEnvelopeService.EnvelopePoint>();if(a==null)return r;int n=Convert.ToInt32(a[0].Value);for(int i=0;i<n;i++){int j=1+13*i;r.Add(new(Convert.ToDouble(a[j].Value),Convert.ToDouble(a[j+1].Value),Convert.ToDouble(a[j+2].Value),Convert.ToString(a[j+3].Value)??"",Convert.ToString(a[j+4].Value)??"",Convert.ToDouble(a[j+5].Value),Convert.ToDouble(a[j+6].Value),Convert.ToString(a[j+7].Value)??"",Convert.ToString(a[j+8].Value)??"",Convert.ToDouble(a[j+9].Value),Convert.ToDouble(a[j+10].Value),Convert.ToString(a[j+11].Value)??"",Convert.ToString(a[j+12].Value)??""));}return r;}
}
