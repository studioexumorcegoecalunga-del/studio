using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;

public sealed class ScheduleCommands
{
    [CommandMethod("RFQUADROESQUADRIAS")]
    public void CreateSchedule()
    {
        var doc=AcApp.DocumentManager.MdiActiveDocument;var ed=doc.Editor;var db=doc.Database;
        var p=ed.GetPoint("\nPonto de inserção do quadro de esquadrias: ");if(p.Status!=PromptStatus.OK)return;
        using var tr=db.TransactionManager.StartTransaction();
        var rows=new List<(string Code,string Type,double W,double H,double S)>();
        var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
        foreach(ObjectId id in ms)
        {
            if(tr.GetObject(id,OpenMode.ForRead) is not Entity e || e.ExtensionDictionary.IsNull)continue;
            var d=(DBDictionary)tr.GetObject(e.ExtensionDictionary,OpenMode.ForRead);
            if(!d.Contains("ROFAMA_DATA"))continue;
            var x=(Xrecord)tr.GetObject(d.GetAt("ROFAMA_DATA"),OpenMode.ForRead);var a=x.Data?.AsArray();if(a==null||a.Length<5)continue;
            var type=a[0].Value?.ToString()??"";if(type!="PORTA"&&type!="JANELA")continue;
            rows.Add((a[4].Value?.ToString()??"",type,Convert.ToDouble(a[1].Value),Convert.ToDouble(a[2].Value),Convert.ToDouble(a[3].Value)));
        }
        rows=rows.OrderBy(x=>x.Code).ToList();
        var table=new Table();table.SetDatabaseDefaults();table.Position=p.Value;table.SetSize(rows.Count+2,5);
        table.SetRowHeight(.30);table.SetColumnWidth(1.10);
        table.Cells[0,0].TextString="QUADRO DE ESQUADRIAS";table.MergeCells(CellRange.Create(table,0,0,0,4));
        string[] h={"ID","TIPO","LARG.","ALT.","PEITORIL"};for(int c=0;c<5;c++)table.Cells[1,c].TextString=h[c];
        for(int i=0;i<rows.Count;i++){var r=rows[i];table.Cells[i+2,0].TextString=r.Code;table.Cells[i+2,1].TextString=r.Type;table.Cells[i+2,2].TextString=$"{r.W:0.00}";table.Cells[i+2,3].TextString=$"{r.H:0.00}";table.Cells[i+2,4].TextString=$"{r.S:0.00}";}
        ms.AppendEntity(table);tr.AddNewlyCreatedDBObject(table,true);tr.Commit();
        ed.WriteMessage($"\nQuadro criado com {rows.Count} esquadria(s).");
    }
}
