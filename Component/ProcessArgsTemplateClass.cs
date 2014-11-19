using System;
using System.Collections.Generic;
using System.IO;
using ProjectManager.Projects;
using PluginCore;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.Utilities;

namespace QuickGenerator.QuickSettings
{
    class ProcessArgsTemplateClass
    {
        static string lastFileFromTemplate;
        static ClassSettings lastFileOptions;
        private static String processOnSwitch;


        private ProcessArgsTemplateClass()
        {
        }

        public static string ProcessArgs(Project project, string args)
        {

            lastFileFromTemplate = QuickGenerator.QuickSettings.GenerateClass.LastFileFromTemplate;
            lastFileOptions = QuickGenerator.QuickSettings.GenerateClass.LastFileOptions;


            if (lastFileFromTemplate != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(lastFileFromTemplate);

                args = args.Replace("$(FileName)", fileName);

                if (args.Contains("$(FileNameWithPackage)") || args.Contains("$(Package)"))
                {
                    string package = "";
                    string path = lastFileFromTemplate;

                    // Find closest parent

                    string classpath="";
                    if(project!=null)
                    classpath = project.AbsoluteClasspaths.GetClosestParent(path);

                    // Can't find parent, look in global classpaths
                    if (classpath == null)
                    {
                        PathCollection globalPaths = new PathCollection();
                        foreach (string cp in ProjectManager.PluginMain.Settings.GlobalClasspaths)
                            globalPaths.Add(cp);
                        classpath = globalPaths.GetClosestParent(path);
                    }
                    if (classpath != null)
                    {
                        if (project != null)
                        {
                            // Parse package name from path
                            package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, path));
                            package = package.Replace(Path.DirectorySeparatorChar, '.');
                        }
                    }

                    args = args.Replace("$(Package)", package);

                    if (package.Length!=0)
                        args = args.Replace("$(FileNameWithPackage)", package + "." + fileName);
                    else
                        args = args.Replace("$(FileNameWithPackage)", fileName);

                    if (lastFileOptions != null)
                    {
                        args = ProcessFileTemplate(args);
                        if (processOnSwitch == null) lastFileOptions = null;
                    }
                }
                lastFileFromTemplate = null;
            }
            return args;
        }


        private static string ProcessFileTemplate(string args)
        {

            Int32 eolMode = (Int32)PluginBase.MainForm.Settings.EOLMode;
            String lineBreak = LineEndDetector.GetNewLineMarker(eolMode);
            ClassModel cmodel;

            List<String> imports = new List<string>();
            string extends = "";
            string implements = "";
            string access = "";
            string inheritedMethods = "";
            string paramString = "";
            string superConstructor = "";
            int index;

            // resolve imports
            if (lastFileOptions.Interfaces != null && lastFileOptions.Interfaces.Count > 0)
            {
                implements = " implements ";
                string[] _implements;
                index = 0;
                foreach (string item in lastFileOptions.Interfaces)
                {
                    if (item.Split('.').Length > 1) imports.Add(item);
                    _implements = item.Split('.');
                    implements += (index > 0 ? ", " : "") + _implements[_implements.Length - 1];

                    if (lastFileOptions.createInheritedMethods)
                    {
                        processOnSwitch = lastFileFromTemplate;
                        // let ASCompletion generate the implementations when file is opened
                    }

                    index++;
                }
            }

            if (lastFileOptions.superClass.Length != 0)
            {
                String super = lastFileOptions.superClass;
                if (lastFileOptions.superClass.Split('.').Length > 1) imports.Add(super);
                string[] _extends = super.Split('.');
                extends = " extends " + _extends[_extends.Length - 1];

                
                if (lastFileOptions.createConstructor )
                {
                    cmodel = ASContext.Context.GetModel(
                        super.LastIndexOf('.') < 0 ? "" : super.Substring(0, super.LastIndexOf('.')),
                        _extends[_extends.Length - 1],
                        "");

                    if (!cmodel.IsVoid())
                    {
                        foreach (MemberModel member in cmodel.Members)
                        {
                            if (member.Name == cmodel.Constructor)
                            {
                                paramString = member.ParametersString();
                                AddImports(imports, member, cmodel);

                                superConstructor = "super(";

                                index = 0;
                                if (member.Parameters != null)
                                    foreach (MemberModel param in member.Parameters)
                                    {
                                        if (param.Name.StartsWith(".")) break;
                                        superConstructor += (index > 0 ? ", " : "") + param.Name;
                                        index++;
                                    }
                                superConstructor += ");\n" + (lastFileOptions.Language == "as3" ? "\t\t\t" : "\t\t");
                                break;
                            }
                        }
                    }
                }
               
            }

                access = lastFileOptions.isPublic ? "public " : "internal ";
                access += lastFileOptions.isDynamic ? "dynamic " : "";
                access += lastFileOptions.isFinal ? "final " : "";


            string importsSrc = "";
            string prevImport = null;
            imports.Sort();
            foreach (string import in imports)
                if (prevImport != import)
                {
                    prevImport = import;
                    importsSrc += (lastFileOptions.Language == "as3" ? "\t" : "")
                        + "import " + import + ";" + lineBreak;
                }
            if (importsSrc.Length > 0)
                importsSrc += (lastFileOptions.Language == "as3" ? "\t" : "") + lineBreak;

            args = args.Replace("$(Import)", importsSrc);
            args = args.Replace("$(Extends)", extends);
            args = args.Replace("$(Implements)", implements);
            args = args.Replace("$(Access)", access);
            args = args.Replace("$(InheritedMethods)", inheritedMethods);
            args = args.Replace("$(ConstructorArguments)", paramString);
            args = args.Replace("$(Super)", superConstructor);
            return args;
        }


        private static void AddImports(List<String> imports, MemberModel member, ClassModel inClass)
        {
            AddImport(imports, member.Type, inClass);
            if (member.Parameters != null)
                foreach (MemberModel item in member.Parameters)
                    AddImport(imports, item.Type, inClass);
        }

        private static String AddImport(List<string> imports, String cname, ClassModel inClass)
        {
            ClassModel aClass = ASContext.Context.ResolveType(cname, inClass.InFile);
            if (aClass != null && !aClass.IsVoid() && aClass.InFile.Package.Length!=0)
                imports.Add(aClass.QualifiedName);
            return "";
        }
    }
}
