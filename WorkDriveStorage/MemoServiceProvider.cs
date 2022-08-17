using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkDriveStorage.CustomControls;
using WorkDriveStorage.FrameWork;
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
            DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData("GetDataStickerMemoAll", "0001");

            foreach (DataRow row in dt.Rows)
            {
                string memoKey = row["Sequence"].ToString();
                string text = row["Contents"].ToString();
                string textRtf = row["ContentsRTF"].ToString();
                bool wallPaper = bool.Parse(row["WallPaper"].ToString());
                string[] Location = row["Location"].ToString() == "" ? new string[] { "0", "0" } : row["Location"].ToString().Split(',');

                MemoItem memo = new MemoItem(memoKey, text, textRtf, wallPaper);
                memo.Margin = new Thickness(7);
                _itemDir.Add(memoKey, memo);


                if (wallPaper)
                {
                    MemoWindow memoWindow = new MemoWindow(memoKey, text);
                    memoWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                    memoWindow.Top = double.Parse(Location[0]);
                    memoWindow.Left = double.Parse(Location[1]);
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


        public void MemoWindowShow(string memoKey, MemoWindow win)
        {
            _windowMemo.Add(memoKey, win);
            win.Show();
        }

        public void Add()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("WallPaper", "False");
            bool result = ServiceProvider.StaticService().MainDatabase.SetData("SetStickerMemoAdd", "0001", param);

            if (result)
            {
                DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData("GetDataStickerMemoMaxSequence", "0001");
                string memoKey = dt.Rows[0][0].ToString();


                MemoItem memo = new MemoItem(dt.Rows[0][0].ToString(), string.Empty, string.Empty, bool.Parse("False"));
                memo.Margin = new Thickness(7);
                _itemDir.Add(memoKey, memo); 
            }
        }

        public void Update(string memoKey, string memoText, string memoRtf)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Contents", memoText.Replace("'", "''"));
            param.Add("ContentRTF", memoRtf.Replace("'", "''"));
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData("SetStickerMemoUpdate", "0001", param);
        }

        public void WallPaperChanged(string memoKey, bool value)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("WallPaper", value.ToString());
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData("SetStickerMemoWallPaperChanged", "0001", param);
        }

        public void MemoWindowLocationChanged(string memoKey, string LocationValue)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Location", LocationValue);
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData("SetStickerMemoLocationChanged", "0001", param);
        }

        public void Delete(string memoKey)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Sequence", memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData("SetStickerMemoDelete", "0001", param);

            if (result)
            {
                _itemDir.Remove(memoKey);

                _windowMemo[memoKey].Close();
                _windowMemo.Remove(memoKey);

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
