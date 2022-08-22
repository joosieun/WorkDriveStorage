using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkDriveStorage.CustomControls;
using WorkDriveStorage.FrameWork;
using WorkDriveStorage.FrameWork.Database;
using WorkDriveStorage.Popup;

namespace WorkDriveStorage
{
    public class MemoServiceProvider
    {
        Dictionary<string, MemoItem> _itemDir = new Dictionary<string, MemoItem>();
        Dictionary<string, MemoWindow> _windowMemo = new Dictionary<string, MemoWindow>();

        private static MemoServiceProvider staticService;

        public static MemoServiceProvider StaticService()
        {
            if (staticService == null)
            {
                staticService = new MemoServiceProvider();
            }

            return staticService;
        }

        public MemoServiceProvider()
        {
            init();
        }

        public void Dispose()
        {
            if (_windowMemo.Count > 0)
            {
                foreach (KeyValuePair<string, MemoWindow> memoWin in _windowMemo)
                {
                    memoWin.Value.Close();
                }
            }
        }

        private void init()
        {
            DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData(GetQueryString.SQLite.GetDataStickerMemoAll);

            foreach (DataRow row in dt.Rows)
            {
                string memoKey = row["Sequence"].ToString();
                string text = row["Contents"].ToString();
                string textRtf = row["ContentsRTF"].ToString();
                bool wallPaper = bool.Parse(row["WallPaper"].ToString());
                string[] Location = row["Location"].ToString() == "" ? new string[] { "0", "0" } : row["Location"].ToString().Split(',');
                string[] Sizes = row["Size"].ToString() == "" ? new string[] { "300", "400" } : row["Size"].ToString().Split(',');

                MemoItem memo = new MemoItem(memoKey, text, textRtf, wallPaper);
                memo.Margin = new Thickness(7);
                _itemDir.Add(memoKey, memo);


                if (wallPaper)
                {
                    MemoWindow memoWindow = new MemoWindow(memoKey, text);
                    memoWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    memoWindow.Top = double.Parse(Location[0]);
                    memoWindow.Left = double.Parse(Location[1]);
                    memoWindow.Width = double.Parse(Sizes[0]);
                    memoWindow.Height = double.Parse(Sizes[1]);
                    _windowMemo.Add(memoKey, memoWindow);
                }
            }
        }

        public void MemoWindowInit()
        { 
            foreach(KeyValuePair<string, MemoWindow> win in _windowMemo)
            {
                win.Value.Show();   
            }
        }

        public void MemoWindow_MemoValeChanged(string memoKey, string memo)
        {
            _itemDir[memoKey].Contents = memo;
            MemoValeChanged_Event();
        }

        public Dictionary<string, MemoItem> GetMemoItemAll()
        {
            return _itemDir;
        }

        public MemoWindow GetMemoWindow(string memoKey)
        {
            if (_windowMemo.ContainsKey(memoKey))
            {
                return _windowMemo[memoKey];
            }
            else
            {
                return null;
            }
        }

        public void MemoWindowClose(string memoKey)
        {
            _windowMemo.Remove(memoKey);
        }


        public void MemoWindowShow(string memoKey, MemoWindow win, double w = 0, double h = 0)
        {
            _windowMemo.Add(memoKey, win);

            if (w > 0 && h > 0)
            {
                win.Width = w;
                win.Height = h;
            }
            win.Show();
        }

        public void Add(bool NowShow = false)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("WallPaper", "False");
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoAdd, param);

            if (result)
            {
                DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData(GetQueryString.SQLite.GetDataStickerMemoMaxSequence);
                string memoKey = dt.Rows[0]["Sequence"].ToString();
                string memoContents = dt.Rows[0]["Contents"].ToString();

                MemoItem memo = new MemoItem(dt.Rows[0][0].ToString(), string.Empty, string.Empty, bool.Parse("False"));
                memo.Margin = new Thickness(7);
                _itemDir.Add(memoKey, memo);

                if (NowShow)
                {
                    MemoWindow win = new MemoWindow(memoKey, memoContents);
                    MemoWindowShow(memoKey, win);
                }
            }
        }

        public void Update(string memoKey, string memoText, string memoRtf)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Contents", memoText.Replace("'", "''"));
            param.Add("ContentRTF", memoRtf.Replace("'", "''"));
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoUpdate, param);
        }

        public void WallPaperChanged(string memoKey, bool value)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("WallPaper", value.ToString());
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoWallPaperChanged, param);
        }

        public void MemoWindowLocationChanged(string memoKey, string LocationValue)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Location", LocationValue);
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoLocationChanged, param);
        }

        public void MemoWindowSizeChanged(string memoKey, string Value)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Size", Value);
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoSizeChanged, param);
        }

        public void Delete(string memoKey)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoDelete, param);

            if (result)
            {
                _itemDir.Remove(memoKey);

                if (_windowMemo.ContainsKey(memoKey))
                {
                    _windowMemo[memoKey].Close();
                    _windowMemo.Remove(memoKey);
                }

                MemoDelete_Event();
            }
        }

        public delegate void OnMemoDeleteDelegate();
        public event OnMemoDeleteDelegate MemoDelete;
        private void MemoDelete_Event()
        {
            MemoDelete?.Invoke();
        }

        public delegate void OnMemoValeChangedDelegate();
        public event OnMemoValeChangedDelegate MemoValeChanged;
        private void MemoValeChanged_Event()
        {
            MemoValeChanged?.Invoke();
        }
    }
}
