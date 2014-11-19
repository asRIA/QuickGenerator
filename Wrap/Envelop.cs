using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;

namespace QuickGenerator.Abbreviation
{
    class Wrap 
    {


        #region ICommandInterface Membri di

 
        private Wrap()
        {
    
        }


        public static void EnvelopInFunction()
        {
           
             ScintillaNet.ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;

              
            ASCompletion.Model.MemberModel mm =   ASContext.Context.CurrentMember;
            int startLine = sci.LineFromPosition(sci.SelectionStart);
            int endLine = sci.LineFromPosition(sci.SelectionEnd);

            if (mm == null || (startLine <(mm.LineFrom +2)) || (endLine >= mm.LineTo) )
            {
                MessageBox.Show("The text is not in a body function!!!");
                return;
            }



            ASExpr expr = new ASExpr();

            expr = GetExpression(sci, sci.CurrentPos, true);
            ASCompletion.Model.MemberList ml =  ASCompletion.Completion.ASComplete.ParseLocalVars(expr);
            

            QuickGenerator.UI.form.CreateFunctionForm form = new QuickGenerator.UI.form.CreateFunctionForm();

            form.ShowDialog();

            if (form.cancel) return;

            if (form.NameFunction.Trim().Length == 0)
            {
                MessageBox.Show("Name function missing!!");
                return;
            }

            string text = sci.SelText;

            string[] lines = text.Split('\n');

            string nl = ASCompletion.Completion.ASComplete.GetNewLineMarker(sci.EOLMode);
            StringBuilder sbBody = new StringBuilder(text.Length);
            int indent = sci.GetLineIndentation(mm.LineFrom);

            string tab = new string('\t', indent / sci.Indent);

            for (int x = 0; x < lines.Length; x++)
            {
                sbBody.Append(nl);
                sbBody.Append(tab);
                sbBody.Append('\t');
                sbBody.Append(lines[x].Trim());
                
            }

         
           
            int startSelection = sci.SelectionStart;
            int endSelection = sci.SelectionEnd;

            Regex regWords = new Regex("\\b[a-zA-Z][a-zA-Z0-9]*\\b");



            MatchCollection matchCollettion = regWords.Matches(text);
            Match mtc;
            List<string> words = new List<string>();
            for (int i = 0; i < matchCollettion.Count; i++)
            {
                mtc = matchCollettion[i];

                if (!words.Contains(mtc.Value))
                    words.Add(mtc.Value);

            }


            StringBuilder sbParams = new StringBuilder(100);
            StringBuilder nameFunction = new StringBuilder(100);
            nameFunction.Append(form.NameFunction);

            MemberModel member;

            
            List<MemberModel> membersToInsert = new List<MemberModel>();
            
            for (int i = 0; i < ml.Count; i++)
            {
                member = ml[i];
                int length = words.Count;

                for (int j = 0; j < length; j++)
                {
                    if (member.Name == words[j])
                    {
                        words.RemoveAt(j);


                        if (!(startLine <= member.LineFrom && endLine >= member.LineFrom))
                         membersToInsert.Add(member);


                        break;
                    }
                    
                }

            }


            nameFunction.Append("(");
      

            if (membersToInsert.Count > 0)
            {
                foreach (MemberModel item in membersToInsert)
                {

                    sbParams.Append(item.Name);
                    sbParams.Append(":");
                    sbParams.Append(item.Type);
                    nameFunction.Append(item.Name);
                    sbParams.Append(", ");
                    nameFunction.Append(", ");
                }

                sbParams.Remove(sbParams.Length - 2, 2);
                nameFunction.Remove(nameFunction.Length - 2, 2);
            }

            nameFunction.Append(");");
 

            int pos = sci.PositionFromLine(mm.LineTo + 1);

           
         
            StringBuilder sb = new StringBuilder(200);
            tab = nl + tab;


            sb.Insert(0, "\nfunction [@1]([@2])\n{[@3]\n}");
            sb.Replace("\n", tab);
            sb.Replace("[@1]", form.NameFunction);
            sb.Replace("[@2]", sbParams.ToString());
            sb.Replace("[@3]", sbBody.ToString());

            sci.BeginUndoAction();

            sci.InsertText(pos,sb.ToString());
            int currentLine = sci.LineFromPosition(startSelection);
            int oldIndent = sci.GetLineIndentation(currentLine );

            sci.SetSel(startSelection, endSelection);

            sci.ReplaceSel("");

            if (form.writeNameFunction)
            {

                int newIndent = sci.GetLineIndentation(currentLine);
                sci.InsertText(startSelection, nameFunction.ToString());

                if (oldIndent != newIndent)
                {
                    sci.SetLineIndentation(currentLine,oldIndent);
                }
                sci.LineEnd();
                
            }
            
                



            sci.EndUndoAction();
          
        }


        /// <summary>
        /// Find Actionscript expression at cursor position
        /// </summary>
        /// <param name="sci">Scintilla Control</param>
        /// <param name="position">Cursor position</param>
        /// <param name="ignoreWhiteSpace">Skip whitespace at position</param>
        /// <returns></returns>
        static private ASExpr GetExpression(ScintillaNet.ScintillaControl Sci, int position, bool ignoreWhiteSpace)
        {
            ASExpr expression = new ASExpr();
            expression.Position = position;
            expression.Separator = ' ';

            // file's member declared at this position
            expression.ContextMember = ASContext.Context.CurrentMember;
            int minPos = 0;
            string body = null;
            if (expression.ContextMember != null)
            {
                minPos = Sci.PositionFromLine(expression.ContextMember.LineFrom);
                StringBuilder sbBody = new StringBuilder();
                for (int i = expression.ContextMember.LineFrom; i <= expression.ContextMember.LineTo; i++)
                    sbBody.Append(Sci.GetLine(i)).Append('\n');
                body = sbBody.ToString();
                //int tokPos = body.IndexOf(expression.ContextMember.Name);
                //if (tokPos >= 0) minPos += tokPos + expression.ContextMember.Name.Length;

                if ((expression.ContextMember.Flags & (FlagType.Function | FlagType.Constructor | FlagType.Getter | FlagType.Setter)) > 0)
                {
                    expression.ContextFunction = expression.ContextMember;
                    expression.FunctionOffset = expression.ContextMember.LineFrom;

                    Match mStart = Regex.Match(body, "(\\)|[a-z0-9*.,-<>])\\s*{", RegexOptions.IgnoreCase);
                    if (mStart.Success)
                    {
                        // cleanup function body & offset
                        int pos = mStart.Index + mStart.Length;
                        expression.BeforeBody = (position < Sci.PositionFromLine(expression.ContextMember.LineFrom) + pos);
                        string pre = body.Substring(0, pos);
                        for (int i = 0; i < pre.Length - 1; i++)
                            if (pre[i] == '\r') { expression.FunctionOffset++; if (pre[i + 1] == '\n') i++; }
                            else if (pre[i] == '\n') expression.FunctionOffset++;
                        body = body.Substring(pos);
                    }
                    expression.FunctionBody = body;
                }
                else
                {
                    int eqPos = body.IndexOf('=');
                    expression.BeforeBody = (eqPos < 0 || position < Sci.PositionFromLine(expression.ContextMember.LineFrom) + eqPos);
                }
            }

            // get the word characters from the syntax definition
            string characterClass = ScintillaNet.ScintillaControl.Configuration.GetLanguage(Sci.ConfigurationLanguage).characterclass.Characters;

            // get expression before cursor
            ContextFeatures features = ASContext.Context.Features;
            int stylemask = (1 << Sci.StyleBits) - 1;
            int style = (position >= minPos) ? Sci.StyleAt(position) & stylemask : 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbSub = new StringBuilder();
            int subCount = 0;
            char c = ' ';
            char c2;
            int startPos = position;
            int braceCount = 0;
            int sqCount = 0;
            int genCount = 0;
            bool hasGenerics = features.hasGenerics;
            bool hadWS = false;
            bool hadDot = ignoreWhiteSpace;
            bool inRegex = false;
            char dot = features.dot[features.dot.Length - 1];
            while (position > minPos)
            {
                position--;
                style = Sci.StyleAt(position) & stylemask;
                if (style == 14) // regex literal
                {
                    inRegex = true;
                }
                else if (!ASComplete.IsCommentStyle(style))
                {
                    c2 = c;
                    c = (char)Sci.CharAt(position);
                    // end of regex literal
                    if (inRegex)
                    {
                        inRegex = false;
                        if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                        expression.SubExpressions.Add("");
                        sb.Insert(0, "RegExp.#" + (subCount++) + "~");
                    }
                    // array access
                    if (c == '[')
                    {
                        sqCount--;
                        if (sqCount == 0)
                        {
                            if (sbSub.Length > 0) sbSub.Insert(0, '[');
                            if (braceCount == 0) sb.Insert(0, ".[]");
                            continue;
                        }
                        if (sqCount < 0)
                        {
                            expression.Separator = ';';
                            break;
                        }
                    }
                    else if (c == ']')
                    {
                        sqCount++;
                    }
                    //
                    else if (c == '<' && hasGenerics)
                    {
                        genCount--;
                        if (genCount < 0)
                        {
                            expression.Separator = ';';
                            break;
                        }
                    }
                    // ignore sub-expressions (method calls' parameters)
                    else if (c == '(')
                    {
                        braceCount--;
                        if (braceCount == 0)
                        {
                            sbSub.Insert(0, c);
                            expression.SubExpressions.Add(sbSub.ToString());
                            sb.Insert(0, ".#" + (subCount++) + "~"); // method call or sub expression

                            int testPos = position - 1;
                            string testWord = ASComplete.GetWordLeft(Sci, ref testPos);
                            if (testWord == "return" || testWord == "case" || testWord == "defaut")
                            {
                                // ex: return (a as B).<complete>
                                expression.Separator = ';';
                                expression.WordBefore = testWord;
                                break;
                            }
                            else continue;
                        }
                        else if (braceCount < 0)
                        {
                            expression.Separator = ';';
                            int testPos = position - 1;
                            string testWord = ASComplete.GetWordLeft(Sci, ref testPos); // anonymous function
                            string testWord2 = ASComplete.GetWordLeft(Sci, ref testPos) ?? "null"; // regular function
                            if (testWord == features.functionKey || testWord == "catch"
                                || testWord2 == features.functionKey
                                || testWord2 == features.getKey || testWord2 == features.setKey)
                            {
                                expression.Separator = ',';
                                expression.coma = ComaExpression.FunctionDeclaration;
                            }
                            break;
                        }
                    }
                    else if (c == ')')
                    {
                        if (!hadDot)
                        {
                            expression.Separator = ';';
                            break;
                        }
                        if (braceCount == 0) // start sub-expression
                        {
                            if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                            sbSub = new StringBuilder();
                        }
                        braceCount++;
                    }
                    else if (c == '>' && hasGenerics)
                    {
                        if (c2 == '.' || c2 == '(')
                            genCount++;
                        else break;
                    }
                    if (braceCount > 0 || sqCount > 0 || genCount > 0)
                    {
                        if (c == ';') // not expected: something's wrong
                        {
                            expression.Separator = ';';
                            break;
                        }
                        // build sub expression
                        sbSub.Insert(0, c);
                        continue;
                    }
                    // build expression
                    if (c <= 32)
                    {
                        if (genCount == 0) hadWS = true;
                    }
                    else if (c == dot)
                    {
                        if (features.dot.Length == 2)
                            hadDot = position > 0 && Sci.CharAt(position - 1) == features.dot[0];
                        else hadDot = true;
                        sb.Insert(0, c);
                    }
                    else if (characterClass.IndexOf(c) >= 0)
                    {
                        if (hadWS && !hadDot)
                        {
                            expression.Separator = ' ';
                            break;
                        }
                        hadWS = false;
                        hadDot = false;
                        sb.Insert(0, c);
                        startPos = position;
                    }
                    else if (c == ';')
                    {
                        expression.Separator = ';';
                        break;
                    }
                    else if (hasGenerics && (genCount > 0 || c == '<'))
                    {
                        sb.Insert(0, c);
                    }
                    else if (c == '{')
                    {
                        expression.coma = DisambiguateComa(Sci, position, minPos);
                        expression.Separator = (expression.coma == ComaExpression.None) ? ';' : ',';
                        break;
                    }
                    else if (c == ',')
                    {
                        expression.coma = DisambiguateComa(Sci, position, minPos);
                        expression.Separator = (expression.coma == ComaExpression.None) ? ';' : ',';
                        break;
                    }
                    else if (c == ':')
                    {
                        expression.Separator = ':';
                        break;
                    }
                    else if (c == '=')
                    {
                        expression.Separator = '=';
                        break;
                    }
                    else //if (hadWS && !hadDot)
                    {
                        if (c == '\'' || c == '"') expression.Separator = '"';
                        else expression.Separator = ';';
                        break;
                    }
                }
                // string literals only allowed in sub-expressions
                else
                {
                    if (braceCount == 0) // not expected: something's wrong
                    {
                        expression.Separator = ';';
                        break;
                    }
                }
            }

            // check if there is a particular keyword
            if (expression.Separator == ' ')
            {
                expression.WordBefore = ASComplete.GetWordLeft(Sci, ref position);
            }

            // result
            expression.Value = sb.ToString();
            expression.PositionExpression = startPos;
         
            return expression;
        }


        /// <summary>
        /// Find out in what context is a coma-separated expression
        /// </summary>
        /// <returns></returns>
        private static ComaExpression DisambiguateComa(ScintillaNet.ScintillaControl Sci, int position, int minPos)
        {
            ContextFeatures features = ASContext.Context.Features;
            // find block start '(' or '{'
            int parCount = 0;
            int braceCount = 0;
            int sqCount = 0;
            char c = (char)Sci.CharAt(position);
            bool wasPar = false;
            if (c == '{') { wasPar = true; position--; }
            while (position > minPos)
            {
                c = (char)Sci.CharAt(position);
                if (c == ';')
                {
                    return ComaExpression.None;
                }
                else if ((c == ',' || c == '=') && wasPar)
                {
                    return ComaExpression.AnonymousObject;
                }
                // var declaration
                else if (c == ':')
                {
                    position--;
                    string word = ASComplete.GetWordLeft(Sci, ref position);
                    word = ASComplete.GetWordLeft(Sci, ref position);
                    if (word == features.varKey) return ComaExpression.VarDeclaration;
                    else continue;
                }
                // Array values
                else if (c == '[')
                {
                    sqCount--;
                    if (sqCount < 0)
                    {
                        return ComaExpression.ArrayValue;
                    }
                }
                else if (c == ']')
                {
                    if (wasPar) return ComaExpression.None;
                    sqCount++;
                }
                // function declaration or parameter
                else if (c == '(')
                {
                    parCount--;
                    if (parCount < 0)
                    {
                        position--;
                        string word1 = ASComplete.GetWordLeft(Sci, ref position);
                        if (word1 == features.functionKey) return ComaExpression.FunctionDeclaration; // anonymous function
                        string word2 = ASComplete.GetWordLeft(Sci, ref position);
                        if (word2 == features.functionKey || word2 == features.setKey || word2 == features.getKey)
                            return ComaExpression.FunctionDeclaration; // function declaration
                        return ComaExpression.FunctionParameter; // function call
                    }
                }
                else if (c == ')')
                {
                    if (wasPar) return ComaExpression.None;
                    parCount++;
                }
                // code block or anonymous object
                else if (c == '{')
                {
                    braceCount--;
                    if (braceCount < 0)
                    {
                        position--;
                        string word1 = ASComplete.GetWordLeft(Sci, ref position);
                        c = (word1.Length > 0) ? word1[word1.Length - 1] : (char)Sci.CharAt(position);
                        if (c != ')' && c != '}' && !char.IsLetterOrDigit(c)) return ComaExpression.AnonymousObject;
                        break;
                    }
                }
                else if (c == '}')
                {
                    if (wasPar) return ComaExpression.None;
                    braceCount++;
                }
                else if (c == '?') return ComaExpression.AnonymousObject;
                position--;
            }
            return ComaExpression.None;
        }




        public static void EnvelopInIF()
        {

            ScintillaNet.ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;

           // MessageBox.Show("end " + sci.SelectionEnd.ToString() + " start " + sci.SelectionStart.ToString());
         
            //int oldPosCur = 0;
            //if (sci.SelectionEnd == sci.CurrentPos)
            //    oldPosCur = sci.CurrentPos - sci.SelText.Length;
            //else
            //    oldPosCur = sci.CurrentPos;

            
            int pos = sci.SelectionStart;
            int initLine = sci.LineFromPosition(pos);
            int indent = sci.GetLineIndentation(initLine);
            string nl = ASComplete.GetNewLineMarker(sci.EOLMode);
            StringBuilder sb = new StringBuilder(200);
            sb.Append("if()");
            sb.Append(nl);
            sb.Append("{");
            sb.Append(nl);
            int posInitBody = sb.Length + pos;
            int initBody = posInitBody;
            sb.Append(sci.SelText.Trim());
            int endBody = posInitBody + sci.SelText.Trim().Length;
            sb.Append(nl);
            sb.Append("}");

            sci.BeginUndoAction();
            sci.ReplaceSel("");
            sci.InsertText(pos, sb.ToString());
            
            initBody = sci.LineFromPosition(initBody);
            endBody = sci.LineFromPosition(endBody);

            int endFunct = sci.LineFromPosition(sb.Length + pos);
            int internalIdent = indent + sci.Indent;

            for (int i = initLine ; i <= endFunct; i++)
            {
                if (i >= initBody && i <= endBody)
                    sci.SetLineIndentation(i, internalIdent);
                else
                    sci.SetLineIndentation(i, indent);
            }

            sci.GotoPos(pos + 3);

            sci.EndUndoAction();

        }
        #endregion
    }
}
