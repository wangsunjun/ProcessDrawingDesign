using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TestDesigner.controls
{
    /// <summary>
    /// ucPager.xaml 的交互逻辑
    /// </summary>
    public partial class ucPager : UserControl
    {

        private DataTable dt = new DataTable();
        private enum PagingMode { Next = 2, Previous = 3, First = 1, Last = 4 };
        private int pRecords = 20;
        private int pIndex = 1;
        private DataGrid grdList;
        ArrayList _list = new ArrayList();
        public ucPager()
        {
            InitializeComponent();
        }

        public void ShowPages(DataGrid grd, DataTable dtt, ArrayList list)
        {
            _list = list;
            _list.Clear();
            try
            {
                grdList = grd;
                dt = dtt.Clone();
                pIndex = 1;
                if (dtt.Rows.Count > 0)
                {
                    for (int i = 0; i < dtt.Rows.Count; i++)
                        dt.ImportRow(dtt.Rows[i]);
                    DataTable tmpTable = new DataTable();
                    tmpTable = dtt.Clone();

                    if (dtt.Rows.Count >= pRecords)
                    {
                        for (int i = 0; i < pRecords; i++)
                            tmpTable.ImportRow(dtt.Rows[i]);
                    }
                    else
                    {
                        for (int i = 0; i < dtt.Rows.Count; i++)
                            tmpTable.ImportRow(dtt.Rows[i]);
                    }
                    this.grdList.ItemsSource = tmpTable.DefaultView;
                    tmpTable.Dispose();
                    DisplayPagingInfo();
                }
                else
                {
                    this.grdList.ItemsSource = null;
                    lblPageNumber.Content = string.Format("共{0}条记录  {1}/{2}", dt.Rows.Count, 0,1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ShowPages系统错误！" + ex.Message);
            }
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {

            CustomPaging((int)PagingMode.Next);
        }

        private void btnPrev_Click(object sender, System.EventArgs e)
        {
            CustomPaging((int)PagingMode.Previous);
        }

        private void btnFirst_Click(object sender, System.EventArgs e)
        {
            CustomPaging((int)PagingMode.First);
        }

        private void btnLast_Click(object sender, System.EventArgs e)
        {
            CustomPaging((int)PagingMode.Last);
        }

        private void CustomPaging(int mode)
        {

            int totalRecords = dt.Rows.Count;
            int pageSize = pRecords;
            int currentPageIndex = pIndex;

            if (dt.Rows.Count <= pRecords)
                return;

            switch (mode)
            {
                case (int)PagingMode.Next:
                    if (dt.Rows.Count > (pIndex * pRecords))
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = dt.Clone();

                        if (dt.Rows.Count >= ((pIndex * pRecords) + pRecords))
                        {
                            for (int i = pIndex * pRecords; i < ((pIndex * pRecords) + pRecords); i++)
                                tmpTable.ImportRow(dt.Rows[i]);
                        }
                        else
                        {
                            for (int i = pIndex * pRecords; i < dt.Rows.Count; i++)
                                tmpTable.ImportRow(dt.Rows[i]);
                        }
                        pIndex += 1;
                        this.grdList.ItemsSource = tmpTable.DefaultView;
                        tmpTable.Dispose();
                        _list.Clear();
                    }
                    break;
                case (int)PagingMode.Previous:
                    if (pIndex > 1)
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = dt.Clone();
                        pIndex -= 1;
                        for (int i = ((pIndex * pRecords) - pRecords); i < (pIndex * pRecords); i++)
                            tmpTable.ImportRow(dt.Rows[i]);

                        this.grdList.ItemsSource = tmpTable.DefaultView;
                        tmpTable.Dispose();
                        _list.Clear();
                    }
                    break;
                case (int)PagingMode.First:
                    pIndex = 2;
                    CustomPaging((int)PagingMode.Previous);
                    break;
                case (int)PagingMode.Last:
                    pIndex = (int)(dt.Rows.Count / pRecords);
                    CustomPaging((int)PagingMode.Next);
                    break;
            }
            DisplayPagingInfo();
        }

        private void DisplayPagingInfo()
        {
            int pTotalPages = dt.Rows.Count / pRecords;
            if (dt.Rows.Count != (pTotalPages * pRecords))
            {
                if (dt.Rows.Count < (pTotalPages * pRecords))
                    pTotalPages--;
                else
                    pTotalPages++;
            }
            lblPageNumber.Content = string.Format("共{0}条记录  {1}/{2}", dt.Rows.Count, pIndex, pTotalPages);
        }

        private void cboRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)e.AddedItems[0];
            if (cbi.Content != null)
            {
                if (cbi.Content.ToString().Equals("全部"))
                {
                    pRecords = dt.Rows.Count + 1;
                    ShowPages(grdList, dt, _list);
                    this.lblPageNumber.Content = string.Format("共{0}条记录  1/1", dt.Rows.Count);
                }
                else
                {
                    this.pRecords = Convert.ToInt16(cbi.Content.ToString());
                    ShowPages(grdList, dt, _list);
                }
            }
        }
    }
}
