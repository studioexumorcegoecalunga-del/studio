using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcApp = Autodesk.AutoCAD.ApplicationServices.Application;
using RofamaCad.Core;

[assembly: CommandClass(typeof(RofamaCad.Plugin))]

namespace RofamaCad;

public sealed class Plugin : IExtensionApplication
{
    const double DefaultWall = 0.15;
    const double DefaultHeight = 2.80;
    const string WallLayer = "RF-ARQ-PAREDE";
    const string DoorLayer = "RF-ARQ-PORTA";
    const string WindowLayer = "RF-ARQ-JANELA";
    const string RoomLayer = "RF-ARQ-AMBIENTE";
    const string DimLayer = "RF-ARQ-COTA";
    const string Wall3dLayer = "RF-3D-PAREDE";
    static int _doorSeq = 1, _windowSeq = 1;

    public void Initialize()
    {
        Ed()?.WriteMessage("\nROFAMA CAD PRO para AutoCAD 2026 carregado. Digite RFSOBRE para ajuda.");
    }
    public void Terminate() { }

    [CommandMethod("RFSOBRE")]
    public void About() => Ed()?.WriteMessage("\nROFAMA CAD PRO | AutoCAD 2026 | parede padrão 0,15 m | comandos: RFPAREDE RFPORTA RFJANELA RFAMBIENTE RFAREA RFCOTAR RFESQUADRIAS RFGERAR3D RFATUALIZAR3D.");

    [CommandMethod("RFPAREDE")]
    public void Wall()
    {
        var ed = Ed(); if (ed == null) return;
        var a = ed.GetPoint("\nInício da parede: "); if (a.Status != PromptStatus.OK) return;
        var b = ed.GetPoint(new PromptPointOptions("\nFim da parede: ") { BasePoint = a.Value, UseBasePoint = true }); if (b.Status != PromptStatus.OK) return;
        var w = ed.GetDouble(new PromptDoubleOptions("\nEspessura <0.15>: ") { DefaultValue = DefaultWall, UseDefaultValue = true, AllowZero = false, AllowNegative = false }); if (w.Status != PromptStatus.OK) return;
        if (a.Value.DistanceTo(b.Value) < 1e-6) { ed.WriteMessage("\nParede inválida: os pontos são coincidentes."); return; }

        using var tr = Db().TransactionManager.StartTransaction();
        EnsureLayer(tr, WallLayer);
        var pl = WallPolygon(a.Value, b.Value, w.Value); pl.Layer = WallLayer;
        Append(tr, pl); SetData(pl, tr, "PAREDE", w.Value, DefaultHeight, 0, ""); StoreyService.Tag(pl, tr, Db());
        tr.Commit();
    }

    [CommandMethod("RFPORTA")] public void Door() => Opening("PORTA", DoorLayer, 0.80, 2.10, 0);
    [CommandMethod("RFJANELA")] public void Window() => Opening("JANELA", WindowLayer, 1.20, 1.20, 1.00);

    [CommandMethod("RFAMBIENTE")]
    public void Room()
    {
        var ed = Ed(); if (ed == null) return;
        var name = ed.GetString("\nNome do ambiente: "); if (name.Status != PromptStatus.OK) return;
        var p = ed.GetPoint("\nPonto do ambiente: "); if (p.Status != PromptStatus.OK) return;
        using var tr = Db().TransactionManager.StartTransaction();
        EnsureLayer(tr, RoomLayer);
        var t = new DBText { Position = p.Value, TextString = name.StringResult, Height = 0.20, Layer = RoomLayer };
        Append(tr, t); SetData(t, tr, "AMBIENTE", 0, 0, 0, name.StringResult); StoreyService.Tag(t, tr, Db()); tr.Commit();
    }

    [CommandMethod("RFAREA")]
    public void Area()
    {
        var ed = Ed(); if (ed == null) return;
        var opt = new PromptEntityOptions("\nSelecione a polilinha fechada do ambiente: ");
        opt.SetRejectMessage("\nSelecione uma polilinha."); opt.AddAllowedClass(typeof(Polyline), true);
        var er = ed.GetEntity(opt); if (er.Status != PromptStatus.OK) return;
        var name = ed.GetString("\nNome do ambiente: "); if (name.Status != PromptStatus.OK) return;
        var p = ed.GetPoint("\nPonto do rótulo: "); if (p.Status != PromptStatus.OK) return;
        using var tr = Db().TransactionManager.StartTransaction();
        var pl = (Polyline)tr.GetObject(er.ObjectId, OpenMode.ForRead);
        if (!pl.Closed) { ed.WriteMessage("\nA polilinha precisa estar fechada."); return; }
        EnsureLayer(tr, RoomLayer);
        var mt = new MText { Location = p.Value, TextHeight = 0.18, Contents = $"{name.StringResult}\\P{pl.Area:0.00} m²", Layer = RoomLayer };
        Append(tr, mt); SetData(mt, tr, "AREA", pl.Area, 0, 0, name.StringResult); StoreyService.Tag(mt, tr, Db()); tr.Commit();
    }

    [CommandMethod("RFCOTAR")]
    public void Dimension()
    {
        var ed = Ed(); if (ed == null) return;
        var a = ed.GetPoint("\nPrimeiro ponto: "); if (a.Status != PromptStatus.OK) return;
        var b = ed.GetPoint("\nSegundo ponto: "); if (b.Status != PromptStatus.OK) return;
        var p = ed.GetPoint("\nPosição da cota: "); if (p.Status != PromptStatus.OK) return;
        using var tr = Db().TransactionManager.StartTransaction();
        EnsureLayer(tr, DimLayer);
        var d = new AlignedDimension(a.Value, b.Value, p.Value, "", Db().Dimstyle) { Layer = DimLayer };
        Append(tr, d); tr.Commit();
    }

    [CommandMethod("RFESQUADRIAS")]
    public void Schedule()
    {
        using var tr = Db().TransactionManager.StartTransaction();
        int doors = 0, windows = 0;
        foreach (ObjectId id in Model(tr))
            if (tr.GetObject(id, OpenMode.ForRead) is Entity e)
            { if (e.Layer == DoorLayer) doors++; else if (e.Layer == WindowLayer) windows++; }
        tr.Commit();
        Ed()?.WriteMessage($"\nEsquadrias encontradas: {doors} porta(s), {windows} janela(s).");
    }

    [CommandMethod("RFGERAR3D")] public void Generate3d() => Build3d(false);
    [CommandMethod("RFATUALIZAR3D")] public void Update3d() => Build3d(true);

    void Opening(string type, string layer, double defaultWidth, double height, double sill)
    {
        var ed = Ed(); if (ed == null) return;
        var p = ed.GetPoint($"\nCentro da {type.ToLower()}: "); if (p.Status != PromptStatus.OK) return;
        var a = ed.GetAngle(new PromptAngleOptions("\nDireção: ") { BasePoint = p.Value, UseBasePoint = true }); if (a.Status != PromptStatus.OK) return;
        var w = ed.GetDouble(new PromptDoubleOptions($"\nLargura <{defaultWidth:0.00}>: ") { DefaultValue = defaultWidth, UseDefaultValue = true, AllowZero = false, AllowNegative = false }); if (w.Status != PromptStatus.OK) return;

        using var tr = Db().TransactionManager.StartTransaction();
        EnsureLayer(tr, layer);
        var v = new Vector2d(Math.Cos(a.Value), Math.Sin(a.Value));
        var n = new Vector2d(-v.Y, v.X); var c = new Point2d(p.Value.X, p.Value.Y); const double d = .075;
        var pl = new Polyline(4);
        pl.AddVertexAt(0, c - v * w.Value / 2 - n * d, 0, 0, 0);
        pl.AddVertexAt(1, c + v * w.Value / 2 - n * d, 0, 0, 0);
        pl.AddVertexAt(2, c + v * w.Value / 2 + n * d, 0, 0, 0);
        pl.AddVertexAt(3, c - v * w.Value / 2 + n * d, 0, 0, 0);
        pl.Closed = true; pl.Layer = layer; Append(tr, pl);
        var code = type == "PORTA" ? $"P{_doorSeq++:00}" : $"J{_windowSeq++:00}";
        SetData(pl, tr, type, w.Value, height, sill, code); StoreyService.Tag(pl, tr, Db());
        var tx = new DBText { Position = p.Value + new Vector3d(.10, .10, 0), TextString = code, Height = .15, Layer = layer };
        Append(tr, tx); tr.Commit();
    }

    void Build3d(bool clear)
    {
        var db = Db(); int walls = 0, cuts = 0;
        using var tr = db.TransactionManager.StartTransaction();
        EnsureLayer(tr, Wall3dLayer);
        var ms = Model(tr); var ids = ms.Cast<ObjectId>().ToArray();

        if (clear)
            foreach (var id in ids)
                if (tr.GetObject(id, OpenMode.ForRead) is Entity e && e.Layer == Wall3dLayer) { e.UpgradeOpen(); e.Erase(); }

        var openings = new List<(Point3d Center, double Angle, double Width, double Height, double Sill)>();
        foreach (var id in ids)
        {
            if (tr.GetObject(id, OpenMode.ForRead) is not Polyline op || (op.Layer != DoorLayer && op.Layer != WindowLayer)) continue;
            var ex = op.GeometricExtents;
            var center = new Point3d((ex.MinPoint.X + ex.MaxPoint.X) / 2, (ex.MinPoint.Y + ex.MaxPoint.Y) / 2, 0);
            var width = Math.Max(ex.MaxPoint.X - ex.MinPoint.X, ex.MaxPoint.Y - ex.MinPoint.Y);
            var q = op.NumberOfVertices > 1 ? op.GetPoint2dAt(1) - op.GetPoint2dAt(0) : Vector2d.XAxis;
            openings.Add((center, Math.Atan2(q.Y, q.X), width, op.Layer == DoorLayer ? 2.10 : 1.20, op.Layer == DoorLayer ? 0 : 1.00));
        }

        foreach (var id in ids)
        {
            if (tr.GetObject(id, OpenMode.ForRead) is not Polyline p || p.Layer != WallLayer || !p.Closed) continue;
            using var curves = new DBObjectCollection(); curves.Add((Polyline)p.Clone());
            using var regs = Region.CreateFromCurves(curves); if (regs.Count == 0) continue;
            using var region = (Region)regs[0];
            var solid = new Solid3d(); solid.SetDatabaseDefaults();
            solid.CreateExtrudedSolid(region, new Vector3d(0, 0, DefaultHeight), new SweepOptions());

            foreach (var o in openings)
            {
                try
                {
                    using var box = new Solid3d(); box.CreateBox(o.Width + .02, DefaultWall + .20, o.Height + .02);
                    box.TransformBy(Matrix3d.Displacement(new Vector3d(-o.Width / 2 - .01, -(DefaultWall + .20) / 2, o.Sill - .01)));
                    box.TransformBy(Matrix3d.Rotation(o.Angle, Vector3d.ZAxis, Point3d.Origin));
                    box.TransformBy(Matrix3d.Displacement(o.Center - Point3d.Origin));
                    if (Intersects(solid.GeometricExtents, box.GeometricExtents)) { solid.BooleanOperation(BooleanOperationType.BoolSubtract, box); cuts++; }
                }
                catch { }
            }
            solid.Layer = Wall3dLayer; Append(tr, solid); walls++;
        }
        tr.Commit(); Ed()?.WriteMessage($"\n3D: {walls} parede(s), {cuts} vão(s) processado(s).");
    }

    static bool Intersects(Extents3d a, Extents3d b) =>
        a.MinPoint.X <= b.MaxPoint.X && a.MaxPoint.X >= b.MinPoint.X &&
        a.MinPoint.Y <= b.MaxPoint.Y && a.MaxPoint.Y >= b.MinPoint.Y &&
        a.MinPoint.Z <= b.MaxPoint.Z && a.MaxPoint.Z >= b.MinPoint.Z;

    static Polyline WallPolygon(Point3d a3, Point3d b3, double width)
    {
        var a = new Point2d(a3.X, a3.Y); var b = new Point2d(b3.X, b3.Y);
        var v = (b - a).GetNormal(); var n = new Vector2d(-v.Y, v.X) * width / 2;
        var p = new Polyline(4);
        p.AddVertexAt(0, a + n, 0, 0, 0); p.AddVertexAt(1, b + n, 0, 0, 0);
        p.AddVertexAt(2, b - n, 0, 0, 0); p.AddVertexAt(3, a - n, 0, 0, 0); p.Closed = true; return p;
    }

    static void SetData(Entity e, Transaction tr, string type, double a, double b, double c, string code)
    {
        if (e.ExtensionDictionary.IsNull) e.CreateExtensionDictionary();
        var dict = (DBDictionary)tr.GetObject(e.ExtensionDictionary, OpenMode.ForWrite);
        var x = new Xrecord { Data = new ResultBuffer(
            new TypedValue((int)DxfCode.Text, type), new TypedValue((int)DxfCode.Real, a),
            new TypedValue((int)DxfCode.Real, b), new TypedValue((int)DxfCode.Real, c),
            new TypedValue((int)DxfCode.Text, code), new TypedValue((int)DxfCode.Text, Guid.NewGuid().ToString("N"))) };
        dict.SetAt("ROFAMA_DATA", x); tr.AddNewlyCreatedDBObject(x, true);
    }

    static void EnsureLayer(Transaction tr, string name)
    {
        var lt = (LayerTable)tr.GetObject(Db().LayerTableId, OpenMode.ForRead); if (lt.Has(name)) return;
        lt.UpgradeOpen(); var rec = new LayerTableRecord { Name = name }; lt.Add(rec); tr.AddNewlyCreatedDBObject(rec, true);
    }
    static void Append(Transaction tr, Entity e) { var ms = Model(tr); ms.AppendEntity(e); tr.AddNewlyCreatedDBObject(e, true); }
    static BlockTableRecord Model(Transaction tr) => (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(Db()), OpenMode.ForWrite);
    static Database Db() => AcApp.DocumentManager.MdiActiveDocument.Database;
    static Editor? Ed() => AcApp.DocumentManager.MdiActiveDocument?.Editor;

}
