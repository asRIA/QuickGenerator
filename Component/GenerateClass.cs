using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Helpers;
using FlashDevelop;
using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;

namespace QuickGenerator.QuickSettings
{

    public class GenerateClass 
    {

        static ClassSettings lastFileOptions;
        private static String lastFileFromTemplate;


        public static ClassSettings LastFileOptions
        {
            get { return lastFileOptions; }
    }




        public static String LastFileFromTemplate
         {
             get { return lastFileFromTemplate; }
         }



        /// <summary>
        /// Text validation file
        /// </summary>
        /// <param name="pathToValidate"></param>
        /// <returns></returns>
        public static bool ValidateFile(string pathToValidate)
        {
            if (pathToValidate.Trim().Length==0) return false;


            return true;
        }

        public void SaveClassFileHaxe(string newFilePath, string ClassName, ClassSettings classSettings)
        {


            StringBuilder sb = new StringBuilder(200);
            StringBuilder sbImports = new StringBuilder(100);
            StringBuilder sbExtends = new StringBuilder(50);
            StringBuilder superConstructor = new StringBuilder(50);
            StringBuilder sbParams = new StringBuilder(50);
            Int32 eolMode = (Int32)Globals.Settings.EOLMode;
            String lineBreak = PluginCore.Utilities.LineEndDetector.GetNewLineMarker(eolMode);
            string author = "";
            List<Argument> arguments = Globals.Settings.CustomArguments;

            foreach (Argument item in arguments)
            {
                if (item.Key == "DefaultUser")
                {
                    author = item.Value;
                    break;
                }
            }
           
            
            sb.Append("/**");
            sb.Append(lineBreak);
            sb.Append("* ...");
            sb.Append(lineBreak);
            sb.Append("* @author ");
            sb.Append(author);
            sb.Append(lineBreak);
            sb.Append("*/");
            sb.Append(lineBreak);
            sb.Append(lineBreak);
            sb.Append("package ");
            sb.Append(classSettings.Package);
            sb.Append(";");

            if (classSettings.superClass.Length!=0)
            {

                // Generate Imports superclass
                sbImports.Append(lineBreak);
                sbImports.Append("import ");
                sbImports.Append(classSettings.superClass);
                sbImports.Append(";");

                sbExtends.Append(" extends ");

                string classSuperName ="";
                string packageSuper = "";
                int pos = classSettings.superClass.LastIndexOf(".");
                if (pos == -1)
                {
                    sbExtends.Append(classSettings.superClass);
                    classSuperName = classSettings.superClass;
                }
                else
                {
                    classSuperName = classSettings.superClass.Substring(pos + 1, classSettings.superClass.Length - pos - 1);
                    packageSuper = classSettings.superClass.Substring(0, pos);
                    sbExtends.Append(classSuperName);
                }


                if (classSettings.createConstructor)
                {
                    ClassModel cm =     ASContext.Context.GetModel(packageSuper, classSuperName, "");

                    MemberList mlImports = cm.InFile.Imports;

                    foreach (MemberModel member in cm.Members)
                    {

                        if (member.Name == classSuperName)
                        {
                            //paramString = member.ParametersString();
                            //AddImports(imports, member, cmodel);
                            superConstructor.Append(lineBreak);
                           superConstructor.Append ("\t\tsuper(");

                           int index = 0;
                         
                            

                           if (member.Parameters != null)
                               foreach (MemberModel param in member.Parameters)
                               {
                                   
                                   //if (param.Name.StartsWith(".")) break;
                                   //superConstructor += (index > 0 ? ", " : "") + param.Name;
                                   index++;
                                     
                                   superConstructor.Append(param.Name);
                                   sbParams.Append(param.Name);
                                   sbParams.Append(":");
                                   sbParams.Append(param.Type);


                                   if (mlImports != null)
                                   {
                                       MemberModel removeimport = null;
                                       foreach (MemberModel  item in mlImports)
                                       {
                                           removeimport = null;
                                           if (item.Name == param.Type)
                                           {
                                               sbImports.Append(lineBreak);
                                               sbImports.Append("import ");
                                               sbImports.Append(item.Type);
                                               sbImports.Append(";");
                                               removeimport = item;
                                               break;
                                           }
                                       }

                                       if (removeimport != null)
                                       {
                                           mlImports.Remove(removeimport);
                                       }
                                   }
                                   if (member.Parameters.Count != index)
                                   {
                                       superConstructor.Append(",");
                                       sbParams.Append(",");
                                   }

                               }
                           superConstructor.Append(");\n");
                            //break;
                        }
                    }
                }
            }


            sb.Append(sbImports.ToString());
            sb.Append(lineBreak);
            sb.Append(lineBreak);
            sb.Append("class ");
            sb.Append(ClassName);
            sb.Append(sbExtends.ToString()); // Extends Class
            sb.Append(lineBreak);
            sb.Append("{");
            sb.Append(lineBreak);
            sb.Append(lineBreak);
            sb.Append("\tpublic function new(");
            sb.Append(sbParams.ToString());
            sb.Append(")");
            sb.Append(lineBreak);
             sb.Append("\t{");
             sb.Append(lineBreak);
             sb.Append(superConstructor.ToString());
             sb.Append(lineBreak);
             sb.Append("\t}");
             sb.Append(lineBreak);
             sb.Append(lineBreak);
             sb.Append("}");


            FlashDevelop.MainForm mainForm = FlashDevelop.MainForm.Instance;
           Encoding encoding = Encoding.GetEncoding((Int32) mainForm.AppSettings.DefaultCodePage);
          //  Encoding encoding = Encoding.GetEncoding(Encoding.UTF8.WebName);
           

            
            FileHelper.WriteFile(newFilePath, sb.ToString(), encoding, Globals.Settings.SaveUnicodeWithBOM);
            //if (actionPoint.EntryPosition != -1)
            //{

            if (mainForm.Documents.Length == 1 && mainForm.Documents[0].IsUntitled)
            {
               // mainForm.closingForOpenFile = true;
                mainForm.Documents[0].Close();
               // mainForm.closingForOpenFile = false;
            }

          
      
        }




        public static bool CheckFileExistence(string newFilePath)
        {

            if (File.Exists(newFilePath))
            {
                string title = " " + TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string message = TextHelper.GetString("ProjectManager.Info.FolderAlreadyContainsFile");

                DialogResult result = MessageBox.Show(PluginBase.MainForm, string.Format(message, newFilePath, "\n"),
                    title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if ( result == DialogResult.No) return false;
            }


            return true;
        }

        public void SaveAS3ClassFile(string filePath, ClassSettings AS3ClassOptions)
        {
          


            string templateFile = Path.Combine(PluginCore.Helpers.PathHelper.TemplateDir, @"ProjectFiles\AS3Project\Class.as.fdt.wizard");
            lastFileFromTemplate = filePath;

            lastFileOptions = AS3ClassOptions;


            ProjectManager.Projects.Project project = (ProjectManager.Projects.Project)PluginBase.CurrentProject;
               //PluginBase.MainForm.FileFromTemplate(templateFile, newFilePath);

            String contents = FileHelper.ReadFile(templateFile);

            FlashDevelop.MainForm mainForm = FlashDevelop.MainForm.Instance;
            Encoding encoding = Encoding.GetEncoding((Int32)mainForm.AppSettings.DefaultCodePage);


            contents = ProcessArgsTemplateClass.ProcessArgs(project, contents);
          contents =  mainForm.ProcessArgString(contents, false);
          ActionPoint actionPoint = SnippetHelper.ProcessActionPoint(contents);
          FileHelper.WriteFile(filePath, actionPoint.Text, encoding, Globals.Settings.SaveUnicodeWithBOM);
            //if (actionPoint.EntryPosition != -1)
            //{

            if (mainForm.Documents.Length == 1 && mainForm.Documents[0].IsUntitled)
            {
                // mainForm.closingForOpenFile = true;
                mainForm.Documents[0].Close();
                // mainForm.closingForOpenFile = false;
            }

            ITabbedDocument document = (ITabbedDocument)mainForm.CreateEditableDocument(filePath, actionPoint.Text, encoding.CodePage);
            SnippetHelper.ExecuteActionPoint(actionPoint, document.SciControl);

            if(LastFileOptions.Interfaces!=null)
                if (LastFileOptions.Interfaces.Count > 0)
                {
                    ASCompletion.Model.ClassModel cm = ASCompletion.Context.ASContext.Context.CurrentModel.Classes[0];
                    foreach (String cname in LastFileOptions.Interfaces)
                    {

                        ASCompletion.Completion.ASGenerator.SetJobContext(null, cname, null, null);
                        ASCompletion.Completion.ASGenerator.GenerateJob(ASCompletion.Completion.GeneratorJobType.ImplementInterface, null, cm, null);
                    }
                }


        
        }

        public static void FileFromTemplate(String templatePath, String newFilePath)
        {
            try
            {


                Encoding encoding = Encoding.GetEncoding((Int32)PluginBase.MainForm.Settings.DefaultCodePage);
                String contents = PluginCore.Helpers.FileHelper.ReadFile(templatePath);
                String processed = FlashDevelop.Utilities.ArgsProcessor.ProcessString(contents, true); 
                PluginCore.Helpers.ActionPoint  actionPoint = PluginCore.Helpers.SnippetHelper.ProcessActionPoint(processed);
                PluginCore.Helpers.FileHelper.WriteFile(newFilePath, actionPoint.Text, encoding);
                if (actionPoint.EntryPosition != -1)
                {
                    if (PluginBase.MainForm.Documents.Length == 1 && PluginBase.MainForm.Documents[0].IsUntitled)
                    {
                        //this.closingForOpenFile = true;
                        //this.Documents[0].Close();
                        //this.closingForOpenFile = false;
                    }
                    ITabbedDocument document = (ITabbedDocument)PluginBase.MainForm.CreateEditableDocument(newFilePath, actionPoint.Text, encoding.CodePage);
                    PluginCore.Helpers.SnippetHelper.ExecuteActionPoint(actionPoint, document.SciControl);
                }
            }finally
                {
                }
           
        }



        //private void CreateRegexAssembly()
        //{
        //    RegexCompilationInfo[] rcinfo = { 
        //    new RegexCompilationInfo("a", RegexOptions.Multiline, "provareg", "QuickGenerator.Regex", true) };
        //    AssemblyName an = new AssemblyName();
        //    an.Name = "QuickGeneratorRegexLibrary";
        //    an.Version = new Version("1.0.0.0");
        //    Regex.CompileToAssembly(rcinfo, an);
        //}


    }
}