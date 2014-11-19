using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using ScintillaNet;
using ASCompletion.Context;
using PluginCore;
using QuickGenerator.Command;

namespace QuickGenerator.CustomCompletionList
{
   

    class CustomCompletionScintillaControl :CustomCompletionBase, IMessageFilter
    {
        //Int
        private Int32 startPos;
       // private const int WM_MOUSEWHEEL = 0x20A;
        private const int WM_KEYDOWN = 0x100;
       // private const int WM_KEYUP = 0x101;

        //Event
        public event ItemSelectedEventHandler OnSelectItem;
        public delegate void ItemSelectedEventHandler(ICommandInterface cmd);

        public CustomCompletionScintillaControl():base()
        {
           
        }


        private void AddHandler()
        {
            
            ASContext.CurSciControl.FocusChanged += new FocusHandler(CurSciControl_FocusChanged);
            ASContext.CurSciControl.UpdateUI += new UpdateUIHandler(CurSciControl_UpdateUI);
          
        }


        private void removeHandler()
        {
            ASContext.CurSciControl.FocusChanged -= new FocusHandler(CurSciControl_FocusChanged);
            ASContext.CurSciControl.UpdateUI -= new UpdateUIHandler(CurSciControl_UpdateUI);
        }

        void CurSciControl_UpdateUI(ScintillaControl sender)
        {
            Hide();
            removeHandler();
        }



        void CurSciControl_FocusChanged(ScintillaControl sender)
        {
            
            if (!completionList.Focused)
            {
                Hide();
                removeHandler();
            }
        }


        public override System.Windows.Forms.Form formDisplay
        {
            get
            {
                return (Form) PluginBase.MainForm;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CLDoubleClick(object sender, EventArgs e)
        {

            ICommandInterface icmd = (ICommandInterface)completionList.SelectedItem;
            Hide();
            removeHandler();
           

            if (OnSelectItem != null)
            {
                OnSelectItem(icmd);
            }


          
        }
        WeakReference lockedSciControl;
        public override void Show(List<ICompletionListItem> itemList, bool autoHide)
        {

            if (completionList.Visible)
                return;


           ScintillaControl sci =  ASContext.CurSciControl;

           lockedSciControl = new WeakReference(sci);
           sci.IgnoreAllKeys = true;
            base.Show(itemList, autoHide);

            AddHandler();

            Application.AddMessageFilter(this);
        }


        public override System.Drawing.Point CalculatePositionCtr()
        {
            ScintillaControl sci = ASContext.CurSciControl;
            startPos = sci.CurrentPos - word.Length;
            

            
            Point coord = new Point(sci.PointXFromPosition(startPos), sci.PointYFromPosition(startPos));
            //listUp = UITools.CallTip.CallTipActive || (coord.Y + completionList.Height > (sci as Control).Height);



            coord = sci.PointToScreen(coord);
            coord.X -= 20;
            coord.Y -= 2;
            //coord = formDisplay.PointToClient(coord);

            //coord.X  = coord.X - 20 + sci.Left;
            //if (listUp) completionList.Top = coord.Y - completionList.Height;
            //else completionList.Top = coord.Y + UITools.Manager.LineHeight(sci);
            ////// Keep on control area
            //if (completionList.Right > formDisplay.ClientRectangle.Right)
            //{
            //    coord.X = formDisplay.ClientRectangle.Right - completionList.Width;
            //}

           
            return coord;
        }

     

        public override void Hide()
        {
            Application.RemoveMessageFilter(this);
            ScintillaControl sci = (ScintillaControl)lockedSciControl.Target;
            sci.IgnoreAllKeys = false;
            sci = null;
            base.Hide();
        }
        protected override Control curCont
        {
            get { return (Control)ASContext.CurSciControl; }
        }
        #region IMessageFilter Membri di

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {

                switch ((Keys) (int) m.WParam & Keys.KeyCode)
                {
                    case Keys.Up:
                        UpList();
                        break;
                    case Keys.Down:
                        downList();
                        break;
                    case Keys.Right:
                        Hide();
                        break;
                    case Keys.Left:
                        Hide();
                        break;
                    case Keys.Enter:



                        ICommandInterface icmd = (ICommandInterface)completionList.SelectedItem;
                        Hide();
                        if (OnSelectItem != null)
                            OnSelectItem(icmd);

                          
                        return true;
    

                    default:
                        break;
                }
              //  downList();
               
            }



            return false;
        }

        #endregion

   


    }
}
