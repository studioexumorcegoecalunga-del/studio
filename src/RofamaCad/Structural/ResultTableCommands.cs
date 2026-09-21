using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Structural;
public sealed class ResultTableCommands
{
 [CommandMethod("RFTABELAV7")]
 public void Run(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var db=d.Database;var p=d.Editor.GetPoint("\nPonto de inserção da tabela: ");if(p.Status!=PromptStatus.OK)return;using var tr=db.TransactionManager.StartTransaction();var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);var rows=new List<(string id,double r,double m,double v)>();int k=1;foreach(ObjectId id in ms){if(tr.GetObject(id,OpenMode.ForRead) is not Line b||b.Layer!="RF-EST-VIGA"||b.ExtensionDictionary.IsNull)continue;var dic=(DBDictionary)tr.GetObject(b.ExtensionDictionary,OpenMode.ForRead);if(!dic.Contains("ROFAMA_MATRIX_ANALYSIS_V7"))continue;var a=((Xrecord)tr.GetObject(dic.GetAt("ROFAMA_MATRIX_ANALYSIS_V7"),OpenMode.ForRead)).Data!.AsArray();int nd=Convert.ToInt32(a[1].Value);var u=a.Skip(2).Take(nd).Select(x=>Math.Abs(Convert.ToDouble(x.Value))).ToArray();var r=a.Skip(2+nd).Take(nd).Select(x=>Math.Abs(Convert.ToDouble(x.Value))).ToArray();var ef=a.Skip(2+2*nd).Select(x=>Math.Abs(Convert.ToDouble(x.Value))).ToArray();rows.Add(($"V{k++:00}",r.Where((_,i)=>i%2==0).DefaultIfEmpty().Max(),ef.Where((_,i)=>i%2==1).DefaultIfEmpty().Max(),u.Where((_,i)=>i%2==0).DefaultIfEmpty().Max()*1000));}var tb=new Table();tb.SetSize(rows.Count+2,4);tb.Position=p.Value;tb.Cells[0,0].TextString="ROFAMA - RESULTADOS V7";tb.MergeCells(CellRange.Create(tb,0,0,0,3));tb.Cells[1,0].TextString="Viga";tb.Cells[1,1].TextString="R máx (kN)";tb.Cells[1,2].TextString="M extremo máx (kN.m)";tb.Cells[1,3].TextString="v nodal máx (mm)";for(int i=0;i<rows.Count;i++){tb.Cells[i+2,0].TextString=rows[i].id;tb.Cells[i+2,1].TextString=rows[i].r.ToString("0.##");tb.Cells[i+2,2].TextString=rows[i].m.ToString("0.##");tb.Cells[i+2,3].TextString=rows[i].v.ToString("0.##");}tb.GenerateLayout();ms.AppendEntity(tb);tr.AddNewlyCreatedDBObject(tb,true);tr.Commit();d.Editor.WriteMessage($"\nTabela V7 criada com {rows.Count} viga(s).");
 }
}
