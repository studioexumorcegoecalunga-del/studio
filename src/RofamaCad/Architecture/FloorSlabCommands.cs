using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;

public sealed class FloorSlabCommands
{
    const string FloorLayer="RF-ARQ-PISO";
    const string SlabLayer="RF-ARQ-LAJE";

    [CommandMethod("RFPISO")]
    public void Floor()=>CreatePlanar("PISO",FloorLayer,0.03);

    [CommandMethod("RFLAJE")]
    public void Slab()=>CreatePlanar("LAJE",SlabLayer,0.12);

    void CreatePlanar(string type,string layer,double defaultThickness)
    {
        var doc=AcApp.DocumentManager.MdiActiveDocument; var ed=doc.Editor; var db=doc.Database;
        var o=new PromptEntityOptions($"\nSelecione a polilinha fechada para {type.ToLower()}: ");
        o.SetRejectMessage("\nSelecione uma polilinha."); o.AddAllowedClass(typeof(Polyline),true);
        var r=ed.GetEntity(o); if(r.Status!=PromptStatus.OK)return;
        var t=ed.GetDouble(new PromptDoubleOptions($"\nEspessura <{defaultThickness:0.00}>: "){DefaultValue=defaultThickness,UseDefaultValue=true,AllowNegative=false,AllowZero=false});
        if(t.Status!=PromptStatus.OK)return;
        using var tr=db.TransactionManager.StartTransaction();
        var src=(Polyline)tr.GetObject(r.ObjectId,OpenMode.ForRead);
        if(!src.Closed){ed.WriteMessage("\nA polilinha precisa estar fechada.");return;}
        var lt=(LayerTable)tr.GetObject(db.LayerTableId,OpenMode.ForRead);
        if(!lt.Has(layer)){lt.UpgradeOpen();var lr=new LayerTableRecord{Name=layer};lt.Add(lr);tr.AddNewlyCreatedDBObject(lr,true);}
        var copy=(Polyline)src.Clone();copy.Layer=layer;
        var ms=(BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db),OpenMode.ForWrite);
        ms.AppendEntity(copy);tr.AddNewlyCreatedDBObject(copy,true);
        if(copy.ExtensionDictionary.IsNull)copy.CreateExtensionDictionary();
        var d=(DBDictionary)tr.GetObject(copy.ExtensionDictionary,OpenMode.ForWrite);
        var x=new Xrecord{Data=new ResultBuffer(new TypedValue((int)DxfCode.Text,type),new TypedValue((int)DxfCode.Real,t.Value),new TypedValue((int)DxfCode.Real,src.Area))};
        d.SetAt("ROFAMA_DATA",x);tr.AddNewlyCreatedDBObject(x,true);
        tr.Commit(); ed.WriteMessage($"\n{type} criado. Área {src.Area:0.00} m².");
    }
}
