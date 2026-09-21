using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;
namespace RofamaCad.Structural;
public sealed class CombinationCatalogCommands
{
 [CommandMethod("RFLISTARCOMBINACOES")]
 public void List(){
  var d=AcApp.DocumentManager.MdiActiveDocument;using var tr=d.Database.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(d.Database.NamedObjectsDictionaryId,OpenMode.ForRead);d.Editor.WriteMessage("\n--- COMBINAÇÕES ROFAMA ---");foreach(DBDictionaryEntry x in nod)if(x.Key.StartsWith("ROFAMA_COMB_")){var a=((Xrecord)tr.GetObject(x.Value,OpenMode.ForRead)).Data?.AsArray();if(a==null||a.Length<2)continue;string name=Convert.ToString(a[0].Value)??x.Key;int n=Convert.ToInt32(a[1].Value);var terms=new List<string>();for(int i=0;i<n&&3+2*i<a.Length;i++)terms.Add($"{Convert.ToDouble(a[3+2*i].Value):0.###}*{a[2+2*i].Value}");d.Editor.WriteMessage($"\n{name} = {string.Join(" + ",terms)}");}tr.Commit();
 }
 [CommandMethod("RFEXCLUIRCOMBINACAO")]
 public void Delete(){
  var d=AcApp.DocumentManager.MdiActiveDocument;var p=d.Editor.GetString(new PromptStringOptions("\nNome da combinação a excluir: "){AllowSpaces=false});if(p.Status!=PromptStatus.OK)return;string name=p.StringResult.Trim().ToUpperInvariant();string key="ROFAMA_COMB_"+new string(name.Where(c=>char.IsLetterOrDigit(c)||c=='_').ToArray());using var tr=d.Database.TransactionManager.StartTransaction();var nod=(DBDictionary)tr.GetObject(d.Database.NamedObjectsDictionaryId,OpenMode.ForWrite);if(nod.Contains(key)){var id=nod.GetAt(key);nod.Remove(key);var x=(DBObject)tr.GetObject(id,OpenMode.ForWrite);x.Erase();d.Editor.WriteMessage($"\nCombinação {name} excluída.");}else d.Editor.WriteMessage("\nCombinação não encontrada.");tr.Commit();
 }
}
