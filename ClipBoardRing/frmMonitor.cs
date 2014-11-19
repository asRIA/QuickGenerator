using System;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using ScintillaNet;
using System.Collections.Generic;

namespace QuickGenerator.clipboardring
{
    class frmMonitor : Form, IPressKey
    {
      


        IntPtr nextClipboardViewer;
        private ScintillaNet.ScintillaControl  sci;
        // defined in winuser.h
        const int WM_DRAWCLIPBOARD = 0x308;
        const int WM_CHANGECBCHAIN = 0x030D;


        private ClipBoardRing clipRing;
        private QuickGenerator.Settings _setting;
        private bool isActive;


        public  frmMonitor(QuickGenerator.Settings setting)
        {
            

            _setting = setting;

            if (_setting.capacityClipBoardRing < 3)
                _setting.capacityClipBoardRing = 3;

   
            clipRing = new ClipBoardRing(_setting.CapacityClipBoardRing);
            
        }

        /// <summary>
        /// Active the monitoring of clipboard
        /// </summary>
        public void ActiveMonitorClipBoard()
        {
            if (isActive) return;
            nextClipboardViewer = (IntPtr)NativeMethods.SetClipboardViewer((int)Handle);
            isActive = true;

        }



        public void SetCapacity(int capacity)
        {
            if (capacity < 3)
                capacity = 3;
            else if (capacity > 50)
                capacity = 50;

            clipRing.SetCapacity(capacity);
        }


        public void ShowNextClipBoardRing()
        {
            ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;
            if (sci == null) return;
            if (!sci.Focused) return;
            
                String current = clipRing.ShowCurrent();
                if (sci.SelText == current)
                    SciInsertAndSelect(sci, clipRing.ShowFoward());
                else
                SciInsertAndSelect(sci, current);
           
        }



        public void ShowPrevClipBoardRing()
        {
            ScintillaControl sci = ASCompletion.Context.ASContext.CurSciControl;
            if (sci == null) return;
            if (!sci.Focused) return;
          
                String current = clipRing.ShowCurrent();
                if (sci.SelText == current)
                    SciInsertAndSelect(sci, clipRing.ShowPrevious());
                else
                    SciInsertAndSelect(sci, current);
          
        }

        static private void SciInsertAndSelect(ScintillaControl sci, string text)
        {
            if (text.Length == 0) return;
            if (sci.SelText.Length == 0)
            {
                sci.InsertText(sci.CurrentPos, text);
                sci.CurrentPos += text.Length;
            }
            else
                sci.ReplaceSel(text);

            sci.SetSel(sci.CurrentPos - text.Length , sci.CurrentPos);
        }


        protected override void WndProc(ref Message m)
        {
           
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:

                    NativeMethods.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);


                    sci = ASCompletion.Context.ASContext.CurSciControl;

                    if(sci!=null)
                    if (sci.Focused)
                        if (sci.SelText.Trim().Length > 0)
                        {
                            clipRing.insert(sci.SelText);
                        }

                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        NativeMethods.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                       

                default:
                    base.WndProc(ref m);
                    break;
            }	

        }

    

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {

            if (isActive)
            {
                isActive = false;
                NativeMethods.ChangeClipboardChain(this.Handle, nextClipboardViewer);
            }


            if (clipRing != null)
            {
                clipRing.Clear();
                clipRing = null;
            }

            _setting = null;
            base.Dispose(disposing);
        }




        #region IPressKey Membri di

        public void EventKey(KeyEvent k)
        {
            if (k.Value == _setting.FowardShortCut)
            {
                ShowNextClipBoardRing();
            }
            else if (k.Value == _setting.PreviousShortCut)
            {
                ShowPrevClipBoardRing();
            }
            else if (k.Value == _setting.ShowClipBoardRingShortCut)
            {
                List<PluginCore.ICompletionListItem> lcomp = new List<PluginCore.ICompletionListItem>();
                List<string> lsClip = clipRing.getClipBoardRing();
                if (lsClip.Count > 0)
                {
                    int maxLenght = 30;
                    StringBuilder sb = new StringBuilder(" ", maxLenght+3);
                    int totalChar = 0;  
                    int count = lsClip.Count;

                    for (int i = count-1; i >-1; i--)
                    {

                        if (lsClip[i].Length > maxLenght)
                            totalChar = maxLenght;
                        else
                            totalChar = lsClip[i].Length;


                        sb.Append(lsClip[i], 0, totalChar);

                        totalChar += 3;

                        sb.Append("...");
                        QuickGenerator.UI.ClipBoardItem comp = new QuickGenerator.UI.ClipBoardItem(sb.ToString(), lsClip[i], lsClip[i], ManagerResources.ClipBoardImage);
                        // for test
                        ///QuickGenerator.UI.CompletionListItem comp = new QuickGenerator.UI.CompletionListItem(sb.ToString(), lsClip[i], lsClip[i], ManagerResources.ClipBoardImage);

                        sb.Remove(1, totalChar);
                        lcomp.Add(comp);
                    }
                    

                   
                    PluginCore.Controls.CompletionList.Show(lcomp, false);
                }
            }
        }

        #endregion

      

      
    }
}
