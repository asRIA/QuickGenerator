using System;
using System.Collections.Generic;

namespace QuickGenerator.QuickSettings
{
    [Serializable]
    public class ClassSettings 
    {
      
            public bool createInheritedMethods;
            public bool createConstructor;
            public string superClass;
            public List<string> Interfaces;
            public bool isPublic;
            public bool isDynamic;
            public bool isFinal;
            public string Language;
            public string Package;

            public ClassSettings(
                string language,
                string super_class,
                List<string> Interfaces,
                bool is_public,
                bool is_dynamic,
                bool is_final,
                bool create_inherited,
                bool create_constructor,
                string Package)
            {
                Language = language;
                createConstructor = create_constructor;
                createInheritedMethods = create_inherited;
                superClass = super_class;
                this.Interfaces = Interfaces;
                isPublic = is_public;
                isDynamic = is_dynamic;
                isFinal = is_final;
                this.Package = Package;
            }
       

    }
}
