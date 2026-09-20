using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcApp=Autodesk.AutoCAD.ApplicationServices.Application;

namespace RofamaCad.Architecture;
public sealed class SectionFacadeCommands
{
 [CommandMethod("RFCORTE")]
 public void Section(){Run("_SECTIONPLANE ","\nRFCORTE iniciou a ferramenta nativa SECTIONPLANE. Defina o plano de corte no modelo 3D.");}
 [CommandMethod("RFFACHADA")]
 public void Facade(){Run("_FLATSHOT ","\nRFFACHADA iniciou FLATSHOT. Posicione a vista desejada antes de executar.");}
 static void Run(string cmd,string msg){var d=AcApp.DocumentManager.MdiActiveDocument;d.Editor.WriteMessage(msg);d.SendStringToExecute(cmd,true,false,true);}
}
