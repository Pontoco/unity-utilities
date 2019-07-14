using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Utilities;
using Conditions;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace Assets.Editor.ScriptParams
{
    /// <summary>
    /// Scans through the entire project and finds any classes that derive from <see cref="ScriptParams{T}"/>.
    /// In order for Unity to allow these assets to be saved as files, non-generic stub files need to be defined 
    /// for each. This class generates those stub files, updates them when the sources change, and removes those 
    /// that no longer are needed. <para>Run this when you add / remove a Params class.</para>
    /// </summary>
    public class ScriptParamsStubGenerator
    {

        [MenuItem("ASG/Generation/Update ScriptParams Stubs")]
        private static void UpdateStubs()
        {
            UpdateStubs(AssemblyUtilities.GetAllAssemblies().ToArray(), "Global/ScriptParamsStubs");
        }

        private static void UpdateStubs(Assembly[] assemblies, string stubsFolder)
        {
            Debug.Log("Checking for ScriptParams stubs that need to be added/removed/updated.");

            // Find all of the classes that need stubs. 
            var scriptParamsClasses = assemblies.SelectMany(assembly => assembly.GetTypes().Where(myType =>
                                                   myType.IsClass && myType.BaseType != null &&
                                                   myType.BaseType.IsGenericType &&
                                                   myType.BaseType.GetGenericTypeDefinition() ==
                                                   typeof(ScriptParams<>))).ToArray();

            // It's an error for two classes with script params to have the same name!
            List<string> names = scriptParamsClasses.Select(c => c.DeclaringType.Name).ToList();
            List<string> namesUnique = names.Distinct().ToList();
            names.Requires().HasLength(namesUnique.Count, "Two classes with ScriptParams objects have the same name!");

            var directory = Path.Combine(Application.dataPath, stubsFolder);

            // Generate/write the text files for every class.
            foreach (Type scriptParamsClass in scriptParamsClasses)
            {
                var filename = scriptParamsClass.DeclaringType.Name + "Params.cs";
                var fullPath = Path.Combine(directory, filename);
                var classTextTemplate = new ScriptParamsStubTemplate(scriptParamsClass);
                var classText = classTextTemplate.GenerateText();

                // Create file if it doesn't exist.
                if (!File.Exists(fullPath))
                {
                    Directory.CreateDirectory(directory);
                    File.WriteAllText(fullPath, classText);
                    Debug.Log("Created new script params stub file: " + fullPath);
                }
                // Only write if the file would change.
                else if (!File.ReadAllText(fullPath).Equals(classText))
                {
                    File.WriteAllText(fullPath, classText);
                    Debug.Log("Updated script params stub file: " + fullPath);
                }
            }

            // Remove stubs for classes that no longer exist.
            List<string> existingScriptParamStubs = Directory.GetFiles(directory)
                                                             .Where(path => Path.GetExtension(path).Equals(".cs"))
                                                             .ToList();

            foreach (string path in existingScriptParamStubs)
            {
                // Remove if we can't find a class that is named the same as the file.
                var name = RemoveFromEnd(Path.GetFileNameWithoutExtension(path), "Params");
                if (!names.Contains(name))
                {
                    Debug.Log("Couldn't find ScriptParams object for class named: " + name + ". Removing stub.");
                    File.Delete(path);
                }
            }

            AssetDatabase.Refresh();
        }

        private static string RemoveFromEnd(string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }
    }
}
