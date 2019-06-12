using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Utilities;
using Conditions;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Assets.Editor.ScriptParams
{
    /// <summary>
    /// Scans through the entire project and finds any classes that derive from <see cref="ScriptParams{T}"/>.
    /// In order for Unity to allow these assets to be saved as files, non-generic stub files need to be defined 
    /// for each. This class generates those stub files, updates them when the sources change, and removes those 
    /// that no longer are needed. Run this when you add / remove a Params class.
    /// </summary>
    public class ScriptParamsStubGenerator
    {
        private static string folderForStubs = "Scripts/ScriptParamsStubs/";

        [MenuItem("Funomena/Generation/Update ScriptParams Stubs")]
        private static void UpdateStubs()
        {
            Debug.Log("Checking for ScriptParams stubs that need to be added/removed/updated.");

            // All of the classes that need stubs.
            var scriptParamsClasses = Assembly.GetAssembly(typeof(ScriptParams<>)).GetTypes()
                                              .Where(myType =>
                                                  myType.IsClass && myType.BaseType != null &&
                                                  myType.BaseType.IsGenericType &&
                                                  myType.BaseType.GetGenericTypeDefinition() ==
                                                  typeof(ScriptParams<>));

            // It's an error for two classes with script params to have the same name!
            List<string> names = scriptParamsClasses.Select(c => c.DeclaringType.Name).ToList();
            List<string> namesUnique = names.Distinct().ToList();
            names.Requires().HasLength(namesUnique.Count, "Two classes with ScriptParams objects have the same name!");

            var directory = Path.Combine(Application.dataPath, folderForStubs);

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
                    Provider.Add(new Asset(fullPath), true);
                    Debug.Log("Created new script params stub file: " + fullPath);
                }
                // Only write if the file would change.
                else if (!File.ReadAllText(fullPath).Equals(classText))
                {
                    Provider.Checkout(fullPath, CheckoutMode.Asset);
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
                    Task task = Provider.Delete(path);
                    task.Wait();
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
