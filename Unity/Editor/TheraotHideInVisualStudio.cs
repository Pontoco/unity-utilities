using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The solution/csproj files are setup fairly differently to the actual compiled version of your game within Unity.
/// Theraot works fine during the Unity build but causes issues in the solution because the solution is linked with
/// .NET 4.5 and provides duplicate definitions of most System.* functions. We want to hide the Theraot dll from the
/// Visual Studio project.
/// </summary>
public class TheraotHideInVisualStudio : AssetPostprocessor
{
    public static void OnGeneratedCSProjectFiles()
    {
        // Open the project file
        string projectDirectory = System.IO.Directory.GetParent(Application.dataPath).FullName;
        foreach (var fileName in new[]
        {
            "Assembly-CSharp.csproj", "Assembly-CSharp-Editor.csproj", "Assembly-CSharp-Editor-firstpass.csproj",
            "Assembly-CSharp-firstpass.csproj"
        })
        {
            string projectFile = Path.Combine(projectDirectory, fileName);

            XmlDocument xml = new XmlDocument();
            xml.Load(projectFile);
            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");

            List<XmlElement> toremove = new List<XmlElement>();

            // Find any Theraot.Core references and remove them.
            var theraot = xml.DocumentElement.SelectNodes("//ns:Reference", nsmgr);
            foreach (XmlElement i in theraot)
            {
                if (i.HasAttributes && i.Attributes["Include"].Value == "Theraot.Core")
                {
                    toremove.Add(i);
                }
            }

            // Remove the nodes.
            foreach (XmlElement xmlElement in toremove)
            {
                xmlElement.ParentNode.RemoveChild(xmlElement);
            }

            xml.Save(projectFile);
        }
    }
}
