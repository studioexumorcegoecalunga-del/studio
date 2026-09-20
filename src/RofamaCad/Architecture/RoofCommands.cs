using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;

public sealed class RoofCommands
{
    [CommandMethod("RFCOBERTURA")]
    public void Roof()
    {
        var doc=AcApp.DocumentManager.MdiActiveDocument;var ed=doc.Editor;var db=doc.Database;
        var o=new PromptEntityOptions("\nSelecione o contorno fechado da cobertura: ");o.SetRejectMessage("\nSelecione uma polilinha.");o.AddAllowedClass(typeof(Polyline),true);
        var r=ed.GetEntity(o);if(r.Status!=PromptStatus.OK)return;
        var s=ed.GetDouble(new PromptDoubleOptions("\nInclinação em % <30>: "){DefaultValue=30,UseDefaultValue=true,AllowNegative=false});if(s.Status!=PromptStatus.OK)return;
        using var tr=db.TransactionManager.StartTransaction();var src=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);if(!src.Closed){ed.WriteMessage("\nContorno deve estar fechado.");return;}
        var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);const string layer="RF-ARQ-COBERTURA";
        if(!lt.Has(layer)){lt.UpgradeOpen();var lr=new LayerTableRecord{Name=layer};lt.Add(lr);tr.AddNewlyCreatedDBObject(lr,true);}
        var cp=(Polyline)src.Clone();cp.Layer=layer;var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);ms.AppendEntity(cp);tr.AddNewlyCreatedDBObject(cp,true);
        if(cp.ExtensionDictionary.IsNull)cp.CreateExtensionDictionary();var d=(DBDictionary)tr.GetObject(cp.ExtensionDictionary,OpenMode.ForWrite);
        var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,"COBERTURA"),new TypedValue((int)DxfCode.Real,s.Value),new TypedValue((int)DxfCode.Real,src.Area))};
        d.SetAt("ROFAMA_DATA",x);tr.AddNewlyCreatedDBObject(x,true);tr.Commit();ed.WriteMessage($"\nCobertura registrada: {src.Area:0.00} m², inclinação {s.Value:0.#}%.");
    }
}
