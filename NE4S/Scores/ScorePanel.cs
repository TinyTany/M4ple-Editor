using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Component;
using NE4S.Notes;
using NE4S.Define;
using NE4S.Operation;

namespace NE4S.Scores
{
    /// <summary>
    /// MainFormのTabPageのScoreに貼るやつ
    /// </summary>
    public class ScorePanel
    {
        /// <summary>
        /// 仮想的な譜面領域の大きさ
        /// </summary>
        private static Size virtualPanelSize;
        /// <summary>
        /// 譜面パネルの位置と大きさ
        /// 実際に表示する領域
        /// </summary>
        private Rectangle displayRect;
        private Model model;
        private HScrollBar hScrollBar;
        private VScrollBar vScrollBar;
        private PictureBox pictureBox;
        private PreviewNote pNote;
        private DataIO dataIO;
        private SusLoader susLoader;
        private SelectionArea selectionArea;
        public OperationManager OperationManager { get; private set; }
        private Note selectedNotePrev = null;
        private SelectionArea selectionAreaPrev = null;
#if DEBUG
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
#endif

        public static class Margin
        {
            public static readonly int
                Top = 10,
                Bottom = 10,
                Left = 3,
                Right = 3;
        }

        public ScorePanel(PictureBox pBox, HScrollBar hScroll, VScrollBar vScroll)
        {
            pictureBox = pBox;
            virtualPanelSize = new Size(
                0,
                (int)(ScoreLane.Height + Margin.Top + Margin.Bottom + 17));
            displayRect = new Rectangle(0, 0, pBox.Width, pBox.Height);
            model = new Model();
            hScrollBar = hScroll;
            vScrollBar = vScroll;
			pNote = new PreviewNote();
            dataIO = new DataIO();
            susLoader = new SusLoader();
            selectionArea = new SelectionArea();
            OperationManager = new OperationManager();
		}

        #region 画面やレーンのサイズ関連
        public void ReSizePanel(Size newSize)
        {
            pictureBox.Size = displayRect.Size = newSize;
            UpdateSizeComponent();
        }

        private void UpdateSizeComponent()
        {
            hScrollBar.Maximum =
                virtualPanelSize.Width < displayRect.Width ? 0 : virtualPanelSize.Width - displayRect.Width;
            vScrollBar.Maximum =
                virtualPanelSize.Height < displayRect.Height ? 0 : virtualPanelSize.Height - displayRect.Height;
            vScrollBar.Visible = virtualPanelSize.Height > displayRect.Height;
            displayRect.Y = vScrollBar.Value = 0;
        }

        public void RefreshLaneSize()
        {
            virtualPanelSize.Height = (int)(ScoreLane.Height + Margin.Top + Margin.Bottom + 17);
            model.LaneBook.Clear(model.ScoreBook);
            model.LaneBook.SetScoreToLane(model.ScoreBook);
            UpdateSizeComponent();
            Update();
        }

        public void RefreshScoreScale(float scale)
        {
            model.LaneBook.Clear(model.ScoreBook);
            model.ScoreBook.ForEach(x => x.RefreshHeight());
            model.LaneBook.SetScoreToLane(model.ScoreBook);
            // NOTE: 拡大縮小したときに小節の表示位置が大きくずれないようにする
            displayRect.X = (int)(displayRect.X * scale);
            MouseScroll(0);
            Update();
        }
        #endregion

        #region 譜面のセーブとロード、エクスポートに関わるもの
        /// <summary>
        /// 保存されていない変更があるかどうかを返します
        /// </summary>
        public bool IsEdited { get; set; } = false;

        public string FileName
        {
            get { return dataIO.FileName; }
        }

        public bool Load()
        {
            Model loadData = null;
            //ファイルが保存されていない場合はメッセージボックスを出す
            if (IsEdited)
            {
                DialogResult dialogResult = 
                    MessageBox.Show(
                        "ファイルは変更されています。保存しますか？",
                        "開く", 
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                if(dialogResult == DialogResult.Yes)
                {
                    Save();
                }
                else if(dialogResult == DialogResult.No)
                {
                    loadData = dataIO.Load();
                }
                else if(dialogResult == DialogResult.Cancel) { }
            }
            else
            {
                loadData = dataIO.Load();
            }
            //読み込みの処理
            if(loadData != null)
            {
                model = loadData;
                RefreshLaneSize();
                IsEdited = false;
                return true;
            }
            return false;
        }

        public bool Save()
        {
            bool isSaved = dataIO.Save(model);
            IsEdited = !isSaved;
            return isSaved;
        }

        public bool SaveAs()
        {
            bool isSaved =  dataIO.SaveAs(model);
            IsEdited = !isSaved;
            return isSaved;
        }

        public bool Import()
        {
            Model importData = null;

            //ファイルが保存されていない場合はメッセージボックスを出す
            if (IsEdited)
            {
                DialogResult dialogResult =
                    MessageBox.Show(
                        "ファイルは変更されています。保存しますか？",
                        "インポート",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes)
                {
                    Save();
                }
                else if (dialogResult == DialogResult.No) { }
                else if (dialogResult == DialogResult.Cancel)
                {
                    return true;
                }
            }

            List<string> message = new List<string>();
            string susFileName = "";
            importData = susLoader.ShowDialog(message, ref susFileName);
            if (importData == null)
            {
                return false;
            }

            dataIO.FileName = susFileName;
            if (message.Count > 0)
            {
                int maxCount = 8;
                string m = (message.Count > maxCount) ?
                    (string.Join("\r\n", message.GetRange(0, maxCount)) + "\r\n\r\n他" + (message.Count - maxCount) + "件の警告")
                    : string.Join("\r\n", message);
                DialogResult dialogResult =
                    MessageBox.Show(
                        "読み込んだsusファイルのうち、以下のデータは失われます。続行しますか？\r\n\r\n" + m,
                        "インポート",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes) { }
                else if (dialogResult == DialogResult.No)
                {
                    return true;
                }
            }

            //読み込みの処理
            model = importData;
            RefreshLaneSize();
            IsEdited = false;
            return true;
        }

        public void Export()
        {
            using(ExportForm exportForm = new ExportForm(model.MusicInfo))
            {
                exportForm.Export(model.ScoreBook, model.NoteBook);
                model.MusicInfo = exportForm.MusicInfo;
            }
        }

        public void ExportAs()
        {
            using (ExportForm exportForm = new ExportForm(model.MusicInfo))
            {
                exportForm.ShowDialog(model.ScoreBook, model.NoteBook);
                model.MusicInfo = exportForm.MusicInfo;
            }
        }

        #endregion

        #region コピペなど

        public void CopyNotes()
        {
            new CopyNotesOperation(selectionArea).Invoke();
        }

        public void CutNotes()
        {
            OperationManager.AddOperationAndInvoke(
                new CutNotesOperation(model, selectionArea));
        }

        /// <summary>
        /// 相対座標を自動で設定してクリップボードからノーツを貼り付けます
        /// </summary>
        public void PasteNotes()
        {
            var scoreLane = model.LaneBook.Find(x => x.HitRect.Left >= displayRect.X);
            Position position = new Position(0, scoreLane.EndTick - ScoreInfo.MaxBeatDiv / Status.Beat + 1);
            PasteNotes(position);
        }

        /// <summary>
        /// 相対座標を指定してクリップボードからノーツを貼り付けます
        /// </summary>
        /// <param name="position"></param>
        public void PasteNotes(Position position)
        {
            OperationManager.AddOperationAndInvoke(
                new PasteNotesOperation(
                    model,
                    selectionArea,
                    position));
        }

        public void PasteAndReverseNotes(Position position)
        {
            OperationManager.AddOperationAndInvoke(
                new PasteAndReverseNotesOperation(
                    model,
                    selectionArea,
                    position));
        }

        public void ClearAreaNotes()
        {
            OperationManager.AddOperationAndInvoke(
                new ClearAreaNotesOperation(
                    model,
                    selectionArea));
        }

        public void ReverseNotes()
        {
            OperationManager.AddOperationAndInvoke(
                new ReverseNotesOperation(model, selectionArea));
        }

        #endregion

        #region Undo, Redo

        public void Undo()
        {
            OperationManager.Undo();
        }

        public void Redo()
        {
            OperationManager.Redo();
        }

        #endregion

        #region ノーツ編集用メニューから

        public void LongNoteToFront(LongNote longNote)
        {
            OperationManager.AddOperationAndInvoke(
                new LongNoteToFrontOperation(model, longNote));
            Refresh();
        }

        public void LongNoteToBack(LongNote longNote)
        {
            OperationManager.AddOperationAndInvoke(
                new LongNoteToBackOperation(model, longNote));
            Refresh();
        }

        public void CutSlide(Slide slide, int tick)
        {
            System.Diagnostics.Debug.Assert(slide != null, "slideがnullです");
            if (slide == null) { return; }
            var past = slide.OrderBy(x => x.Position.Tick).Where(x => x.Position.Tick <= tick).Last();
            var future = slide.OrderBy(x => x.Position.Tick).Where(x => x.Position.Tick > tick).First();
            OperationManager.AddOperationAndInvoke(
                new CutSlideOperation(model, slide, past, future));
            Refresh();
        }

        #endregion

        #region laneBookを触る用メソッド群

        /// <summary>
        /// 末尾に指定した拍子数の譜面を指定した個数追加
        /// </summary>
        /// <param name="beatNumer">拍子分子</param>
        /// <param name="beatDenom">拍子分母</param>
        /// <param name="barCount">個数</param>
        public void SetScore(int beatNumer, int beatDenom, int barCount)
        {
            model.SetScore(beatNumer, beatDenom, barCount);
            Update();
        }

        /// <summary>
        /// scoreの1つ先に新たにscoreを挿入
        /// </summary>
        /// <param name="score"></param>
        /// <param name="beatNumer"></param>
        /// <param name="beatDenom"></param>
        /// <param name="barCount"></param>
        public void InsertScoreForward(Score score, int beatNumer, int beatDenom, int barCount)
        {
            OperationManager.AddOperationAndInvoke(
                new InsertScoreForwardOperation(
                    model,
                    score,
                    beatNumer,
                    beatDenom,
                    barCount));
			Update();
        }

        public void InsertScoreForwardWithNote(Score score, int beatNumer, int beatDenom, int barCount)
        {
            OperationManager.AddOperationAndInvoke(
                new InsertScoreForwardWithNoteOperation(
                    model,
                    score,
                    beatNumer,
                    beatDenom,
                    barCount));
            Update();
        }

        /// <summary>
        /// scoreの1つ前に新たにscoreを挿入
        /// </summary>
        /// <param name="score"></param>
        /// <param name="beatNumer"></param>
        /// <param name="beatDenom"></param>
        /// <param name="barCount"></param>
        public void InsertScoreBackward(Score score, int beatNumer, int beatDenom, int barCount)
        {
            OperationManager.AddOperationAndInvoke(
                new InsertScoreBackwardOperation(
                    model,
                    score,
                    beatNumer,
                    beatDenom,
                    barCount));
            Update();
        }

        public void InsertScoreBackwardWithNote(Score score, int beatNumer, int beatDenom, int barCount)
        {
            OperationManager.AddOperationAndInvoke(
                new InsertScoreBackwardWithNoteOperation(
                    model,
                    score,
                    beatNumer,
                    beatDenom,
                    barCount));
            Update();
        }

        /// <summary>
        /// 指定したscoreとその1つ前のScoreでレーンを2つに分割する
        /// </summary>
        /// <param name="score"></param>
        public void DivideLane(Score score)
        {
            model.DivideLane(score);
            Update();
        }

        /// <summary>
        /// 指定されたscore削除
        /// </summary>
        /// <param name="score">削除対象のScore</param>
        public void DeleteScore(Score score)
        {
            DeleteScore(score, 1);
        }

        public void DeleteScoreWithNote(Score score)
        {
            DeleteScoreWithNote(score, 1);
        }

        /// <summary>
        /// 指定されたscoreからcount個のScoreを削除
        /// </summary>
        /// <param name="score">削除開始のScore</param>
        /// <param name="count">削除する個数</param>
        public void DeleteScore(Score score, int count)
        {
            OperationManager.AddOperationAndInvoke(
                new DeleteScoreOperation(model, score, count));
            Update();
        }

        public void DeleteScoreWithNote(Score score, int count)
        {
            OperationManager.AddOperationAndInvoke(
                new DeleteScoreWithNoteOperation(
                    model,
                    score,
                    count));
            Update();
        }

        /// <summary>
        /// レーン全体を詰める
        /// </summary>
        public void FillLane()
        {
			model.FillLane();
        }

        /// <summary>
        /// begin以降のレーンを詰める
        /// </summary>
        /// <param name="begin"></param>
        public void FillLane(ScoreLane begin)
        {
            model.FillLane(begin);
            Update();
        }
		#endregion

		private void Update()
        {
			var laneBook = model.LaneBook;
            virtualPanelSize.Width = (int)(ScoreLane.Width + Margin.Left + Margin.Right) * laneBook.Count;
            hScrollBar.Maximum = 
                virtualPanelSize.Width < displayRect.Width ? 0 : virtualPanelSize.Width - displayRect.Width;
            //pBoxを更新
            pictureBox.Refresh();
        }

        #region マウス入力とかに反応して処理するメソッドたち

        public void MouseClick(MouseEventArgs e)
        {
            var laneBook = model.LaneBook;
            //クリックされたレーンを特定
            ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.Add(displayRect.Location)));
            if (selectedLane != null && selectedLane.SelectedScore(e.Location.Add(displayRect.Location)) != null && e.Button == MouseButtons.Right && Status.Mode == Mode.Edit)
            {
                //クリックされたグリッド座標を特定
                Position currentPosition = selectedLane.GetLocalPosition(PointToGrid(e.Location, selectedLane, 0).Add(displayRect.Location));
                var slide = model.SelectedSlide(e.Location.Add(displayRect.Location));
                if (selectionArea.Contains(currentPosition))
                {
                    new NoteEditCMenu(this, currentPosition).Show(pictureBox, e.Location);
                }
                else if (slide != null)
                {
                    new LongNoteEditCMenu(this, slide, currentPosition.Tick).Show(pictureBox, e.Location);
                }
                else
                {
                    new EditCMenu(this, selectedLane, selectedLane.SelectedScore(e.Location.Add(displayRect.Location)), currentPosition).Show(pictureBox, e.Location);
                }
            }
#if DEBUG
            if (selectedLane != null)
            {
                //MessageBox.Show(selectedLane.CurrentBarSize.ToString());
            }
            if (Status.Mode != Mode.Edit) { return; }
            var selectedNote = model.NoteBook.SelectedNote(e.Location.Add(displayRect.Location));
            if (selectedNote == null) { return; }
            if (e.Button == MouseButtons.Middle)
            {
                MessageBox.Show(
                    selectedNote.ToString() + "\n" +
                    selectedNote.Position.Lane + ", " + selectedNote.Position.Tick);
            }
#endif
        }

        public void MouseDoubleClick(MouseEventArgs e)
        {
            if (Status.Mode != Mode.Edit) { return; }
            var slide = model.SelectedSlide(e.Location.Add(displayRect.Location));
            var selectedNote = model.NoteBook.SelectedNote(e.Location.Add(displayRect.Location));
            if (slide == null) { return; }
            if (selectedNote is SlideTap || selectedNote is SlideRelay)
            {
                OperationManager.AddOperationAndInvoke(new SlideTapRelayReverseOperation(
                    slide,
                    new List<Note>() { selectedNote }));
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
			Status.IsMousePressed = true;
			ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.Add(displayRect.Location)));
            #region 座標などをコンソール出力（デバッグ時のみ）
#if DEBUG
            //デバッグ用にクリックした座標などをコンソールに出力する
            //本番では必要ない
            if (selectedLane != null && e.Button == MouseButtons.Left)
			{
                System.Diagnostics.Debug.WriteLine(selectedLane.Index);
                Point gridPoint = PointToGrid(e.Location, selectedLane);
                System.Diagnostics.Debug.WriteLine(gridPoint);
                Position position = selectedLane.GetLocalPosition(gridPoint.AddX(displayRect.X));
				if(position != null)
				{
                    position.PrintPosition();
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("MouseDown(MouseEventArgs) : selectedLane.GetPos = null");
				}
			}
#endif
            #endregion
            #region 左クリック
            if (e.Button == MouseButtons.Left)
			{
                NoteArea noteArea = NoteArea.None;
                var selectedNote = model.NoteBook.SelectedNote(e.Location.Add(displayRect.Location), ref noteArea);
				switch (Status.Mode)
				{
					case Mode.Add:
                        if(selectedLane != null)
                        {
                            AddNote(e.Location, selectedLane);
                        }
                        break;
					case Mode.Edit:
                        //Airは単体で動かせないようにする
                        if (selectedNote is Air)
                        {
                            selectedNote = null;
                        }
                        if (selectedLane != null)
                        {
                            Position currentPosition = 
                                selectedLane.GetLocalPosition(PointToGrid(e.Location, selectedLane, 0).Add(displayRect.Location));
                            if (selectionArea.Contains(currentPosition))
                            {
                                selectionArea.MovePositionDelta = new Position(
                                    currentPosition.Lane - selectionArea.TopLeftPosition.Lane,
                                    currentPosition.Tick - selectionArea.TopLeftPosition.Tick);
                                // 現在のPosition情報を控え、矩形選択移動が行われることを覚えておく
                                selectionAreaPrev = new SelectionArea(selectionArea);
                            }
                            else if (selectedNote != null)
                            {
                                // 現在のPosition情報を控え、ノーツ移動かサイズ変更が行われることを覚えておく
                                selectedNotePrev = new Note(selectedNote);
                                Status.SelectedNote = selectedNote;
                                Status.SelectedNoteArea = noteArea;
                                if (selectedNote is SlideRelay && !Status.IsSlideRelayVisible)
                                {
                                    Status.SelectedNote = null;
                                    Status.SelectedNoteArea = NoteArea.None;
                                }
                                if (selectedNote is SlideCurve && !Status.IsSlideCurveVisible)
                                {
                                    Status.SelectedNote = null;
                                    Status.SelectedNoteArea = NoteArea.None;
                                }
                                //カーソルの設定
                                SetCursor(selectedNote, noteArea);
                            }
                            else
                            {
                                selectionArea = new SelectionArea
                                {
                                    StartPosition = currentPosition,
                                    EndPosition = null
                                };
                            }
                        }
                        else
                        {
                            selectionArea = new SelectionArea();
                        }
                        break;
					case Mode.Delete:
                        if (selectedNote != null)
                        {
                            OperationManager.AddOperationAndInvoke(new DeleteNoteOperation(model, selectedNote));
                        }
                        break;
					default:
						break;
				}
			}
            #endregion
            #region 右クリック
            else if (e.Button == MouseButtons.Right)
            {
                var selectedNote = model.NoteBook.SelectedNote(e.Location.Add(displayRect.Location));
                if (Status.Mode == Mode.Edit && selectedNote is AttributeNote attributeNote)
                {
                    float noteValue = attributeNote.NoteValue;
                    new SetValueCustomForm(attributeNote).ShowDialog();
                    if (attributeNote.NoteValue != noteValue)
                    {
                        OperationManager.AddOperation(new ChangeNoteValueOperation(
                            attributeNote,
                            noteValue,
                            attributeNote.NoteValue));
                    }
                    pictureBox.Cursor = Cursors.Default;
                    Status.IsMousePressed = false;
                }
            }
            #endregion
            if (selectedLane == null) System.Diagnostics.Debug.WriteLine("MouseDown(MouseEventArgs) : selectedLane = null");
		}

        private void SetCursor(Note selectedNote, NoteArea noteArea)
        {
            if (selectedNote == null)
            {
                pictureBox.Cursor = Cursors.Default;
                return;
            }
            if (selectedNote is AirHoldEnd || selectedNote is AirAction || selectedNote is AttributeNote)
            {
                pictureBox.Cursor = Cursors.SizeNS;
            }
            else if (noteArea == NoteArea.Left || noteArea == NoteArea.Right)
            {
                pictureBox.Cursor = Cursors.SizeWE;
            }
            else if (noteArea == NoteArea.Center)
            {
                if (selectedNote is HoldEnd)
                {
                    pictureBox.Cursor = Cursors.SizeNS;
                }
                else
                {
                    pictureBox.Cursor = Cursors.SizeAll;
                }
            }
            else
            {
                pictureBox.Cursor = Cursors.Default;
            }
        }

        public void MouseMove(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
			switch (Status.Mode)
			{
				case Mode.Add:
					ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.Add(displayRect.Location)));
					if (selectedLane != null)
					{
						pNote.Location = PointToGrid(e.Location, selectedLane);
						pNote.Visible = true;
					}
					else
					{
						pNote.Visible = false;
					}
                    //ロングノーツを置いたときに終点をそのまま移動できるようにとりあえずほぼそのままコピペ
                    //PointToGridのオーバーロードが違うだけ
                    //動いた
                    if (Status.IsMousePressed && e.Button == MouseButtons.Left && Status.SelectedNote != null && selectedLane != null)
                    {
                        Point physicalGridPoint = PointToGrid(e.Location, selectedLane);
                        Point virtualGridPoint = physicalGridPoint.Add(displayRect.Location);
                        Position newPos = selectedLane.GetLocalPosition(virtualGridPoint);
                        Status.SelectedNote.Relocate(newPos, virtualGridPoint, selectedLane.Index);
                    }
                    break;
				case Mode.Edit:
					selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.Add(displayRect.Location)));
                    //選択されているノーツに対するサイズ変更、位置変更を行う
					if (Status.IsMousePressed && e.Button == MouseButtons.Left && Status.SelectedNote != null && selectedLane != null)
					{
                        switch (Status.SelectedNoteArea)
                        {
                            case NoteArea.Left:
                                {
                                    if (Status.SelectedNote.LaneIndex != selectedLane.Index) return;
                                    Point virtualGridPoint = PointToGrid(e.Location, selectedLane, 0).Add(displayRect.Location);
                                    int newSize = 
                                        (int)((Status.SelectedNote.Location.X + Status.SelectedNote.Width - virtualGridPoint.X) / ScoreInfo.UnitLaneWidth);
                                    if (newSize <= 0) newSize = 1;
                                    else if (newSize > 16) newSize = 16;
                                    Status.SelectedNote.ReSize(newSize);
                                }
                                break;
                            case NoteArea.Center:
                                {
                                    //ノーツのサイズを考慮したほうのメソッドを使う
                                    Point physicalGridPoint = PointToGrid(e.Location, selectedLane, Status.SelectedNote.Size);
                                    Point virtualGridPoint = physicalGridPoint.Add(displayRect.Location);
                                    Position newPos = selectedLane.GetLocalPosition(virtualGridPoint);
                                    Status.SelectedNote.Relocate(newPos, virtualGridPoint, selectedLane.Index);
                                }
                                break;
                            case NoteArea.Right:
                                {
                                    if (Status.SelectedNote.LaneIndex != selectedLane.Index) return;
                                    Point virtualGridPoint = PointToGrid(e.Location, selectedLane, 1).Add(displayRect.Location);
                                    int newSize = (int)((virtualGridPoint.X - Status.SelectedNote.Location.X) / ScoreInfo.UnitLaneWidth);
                                    ++newSize;
                                    if (newSize <= 0) newSize = 1;
                                    else if (newSize > 16) newSize = 16;
                                    Status.SelectedNote.ReSize(newSize);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    //選択矩形のいち変更を行う
                    if (Status.IsMousePressed && selectedLane != null && Status.SelectedNote == null && e.Button == MouseButtons.Left)
                    {
                        Position currentPosition = 
                            selectedLane.GetLocalPosition(PointToGrid(e.Location, selectedLane, 0).Add(displayRect.Location));
                        if (selectionArea.MovePositionDelta != null)
                        {
                            pictureBox.Cursor = Cursors.SizeAll;
                            selectionArea.Relocate(currentPosition, model.LaneBook);
                        }
                        else
                        {
                            selectionArea.EndPosition = currentPosition;
                        }
                    }
                    //選択矩形上にカーソルが乗ったときのカーソルのタイプを変更する
                    else if (!Status.IsMousePressed && selectedLane != null)
                    {
                        Position currentPosition = 
                            selectedLane.GetLocalPosition(PointToGrid(e.Location, selectedLane, 0).Add(displayRect.Location));
                        if (selectionArea.Contains(currentPosition))
                        {
                            pictureBox.Cursor = Cursors.SizeAll;
                        }
                        else
                        {
                            NoteArea noteArea = NoteArea.None;
                            var note = model.NoteBook.SelectedNote(e.Location.Add(displayRect.Location), ref noteArea);
                            SetCursor(note, noteArea);
                        }
                    }
                    break;
				case Mode.Delete:
                    var selectedNote = model.NoteBook.SelectedNote(e.Location.Add(displayRect.Location));
                    if(Status.IsMousePressed && selectedNote != null)
                    {
                        OperationManager.AddOperationAndInvoke(new DeleteNoteOperation(model, selectedNote));
                    }
                    break;
				default:
					break;
			}
        }

        public void MouseUp(MouseEventArgs e)
        {
            // MouseDownで控えた情報からここでOperationをOperationManagerに追加
            if (selectedNotePrev != null && Status.SelectedNote != null)
            {
                // NOTE: ノーツの左からサイズ変更を行うとノーツの位置も変更されるので、
                // 条件分岐に気をつける（そもそも1回の操作で位置とサイズ変更は行えないしね）
                if (selectedNotePrev.Size != Status.SelectedNote.Size)
                {
                    OperationManager.AddOperation(
                        new ReSizeNoteOperation(
                            Status.SelectedNote,
                            selectedNotePrev.Size,
                            Status.SelectedNote.Size,
                            Status.SelectedNoteArea));
                }
                else if (!selectedNotePrev.Position.Equals(Status.SelectedNote.Position))
                {
                    OperationManager.AddOperation(
                        new RelocateNoteOperation(
                            Status.SelectedNote,
                            selectedNotePrev.Position,
                            Status.SelectedNote.Position,
                            model.LaneBook));
                }
            }
            else if (selectionAreaPrev != null && selectionArea != null)
            {
                Position diff = new Position(
                    selectionArea.TopLeftPosition.Lane - selectionAreaPrev.TopLeftPosition.Lane,
                    selectionArea.TopLeftPosition.Tick - selectionAreaPrev.TopLeftPosition.Tick);
                if (diff.Tick != 0 || diff.Lane != 0)
                {
                    OperationManager.AddOperation(
                    new RelocateNoteOperation(
                        selectionArea.SelectedNoteList,
                        selectionArea.SelectedLongNoteList,
                        diff,
                        model.LaneBook));
                }
            }
            selectedNotePrev = null;
            selectionAreaPrev = null;
            Status.IsMousePressed = false;
            Status.SelectedNote = null;
            Status.SelectedNoteArea = NoteArea.None;
            selectionArea.MovePositionDelta = null;
            if (!selectionArea.SelectedNoteList.Any() && !selectionArea.SelectedLongNoteList.Any())
            {
                selectionArea.SetContainsNotes(model.NoteBook);
            }
            pictureBox.Cursor = Cursors.Default;
        }

        public void MouseEnter(EventArgs e) { }

        public void MouseLeave(EventArgs e) { }

        public void MouseScroll(int delta)
        {
            if (Control.ModifierKeys == Keys.Shift)
            {
                displayRect.Y -= delta;
                if (displayRect.Y < vScrollBar.Minimum) displayRect.Y = vScrollBar.Minimum;
                else if (vScrollBar.Maximum < displayRect.Y) displayRect.Y = vScrollBar.Maximum;
                vScrollBar.Value = displayRect.Y;
            }
            else
            {
                displayRect.X -= delta;
                if (displayRect.X < hScrollBar.Minimum) displayRect.X = hScrollBar.Minimum;
                else if (hScrollBar.Maximum < displayRect.X) displayRect.X = hScrollBar.Maximum;
                hScrollBar.Value = displayRect.X;
            }
        }

        public void HSBarScroll(ScrollEventArgs e)
        {
            displayRect.X += (e.NewValue - e.OldValue);
        }

        public void VSBarScroll(ScrollEventArgs e)
        {
            displayRect.Y += (e.NewValue - e.OldValue);
        }
        #endregion

        private void AddNote(Point location, ScoreLane lane)
        {
            //与えられた自由物理座標からグリッド仮想座標とポジション座標を作成
            Point gridPoint = PointToGrid(location, lane);
            Position position = lane.GetLocalPosition(gridPoint.Add(displayRect.Location));
            PointF locationVirtual = gridPoint.Add(displayRect.Location);

            Note newNote = null;
            switch (Status.Note)
            {
                #region ShortNote
                case NoteType.TAP:
                    if (!Status.IsShortNoteVisible) break;
                    newNote = new Tap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.EXTAP:
                    if (!Status.IsShortNoteVisible) break;
                    newNote = new ExTap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.EXTAPDOWN:
                    if (!Status.IsShortNoteVisible) break;
                    newNote = new ExTapDown(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.AWEXTAP:
                    if (!Status.IsShortNoteVisible) break;
                    newNote = new AwesomeExTap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.HELL:
                    if (!Status.IsShortNoteVisible) break;
                    newNote = new HellTap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.FLICK:
                    if (!Status.IsShortNoteVisible) break;
                    newNote = new Flick(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                #endregion
                #region LongNote
                case NoteType.HOLD:
                    if (!Status.IsHoldVisible) break;
                    OperationManager.AddOperationAndInvoke(
                        new AddLongNoteOperation(
                            model,
                            new Hold(Status.NoteSize, position, locationVirtual, lane.Index)));
                    break;
                case NoteType.SLIDE:
                    if (!Status.IsSlideVisible) break;
                    //Slideとの当たり判定は自由仮想座標を使う
                    Slide selectedSlide = model.SelectedSlide(location.Add(displayRect.Location));
                    //Shiftキーを押しながら追加した際はかならず新規Slideノーツを追加する
                    if(selectedSlide != null && Control.ModifierKeys != Keys.Shift)
                    {
                        if (Status.InvisibleSlideTap && Status.IsSlideRelayVisible)
                        {
                            SlideRelay slideRelay = 
                                new SlideRelay(Status.NoteSize, position, locationVirtual, lane.Index);
                            OperationManager.AddOperationAndInvoke(
                                new AddStepNoteOperation(
                                    selectedSlide,
                                    slideRelay));
                            Status.SelectedNote = slideRelay;
                        }
                        else
                        {
                            SlideTap slideTap = new SlideTap(Status.NoteSize, position, locationVirtual, lane.Index);
                            OperationManager.AddOperationAndInvoke(
                                new AddStepNoteOperation(
                                    selectedSlide,
                                    slideTap));
                            Status.SelectedNote = slideTap;
                        }
                    }
                    else
                    {
                        OperationManager.AddOperationAndInvoke(
                        new AddLongNoteOperation(
                            model,
                            new Slide(Status.NoteSize, position, locationVirtual, lane.Index)));
                    }
                    break;
                case NoteType.SLIDECURVE:
                    if (!Status.IsSlideVisible || !Status.IsSlideCurveVisible) break;
                    selectedSlide = model.SelectedSlide(location.Add(displayRect.Location));
                    if (selectedSlide != null)
                    {
                        SlideCurve slideCurve = 
                            new SlideCurve(Status.NoteSize, position, locationVirtual, lane.Index);
                        OperationManager.AddOperationAndInvoke(
                                new AddStepNoteOperation(
                                    selectedSlide,
                                    slideCurve));
                        Status.SelectedNote = slideCurve;
                    }
                    break;
                case NoteType.AIRHOLD:
                    if (!Status.IsAirHoldVisible) break;
                    AirHold selectedAirHold = model.SelectedAirHold(location.Add(displayRect.Location));
                    var selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedAirHold != null)
                    {
                        AirAction airAction = 
                            new AirAction(selectedAirHold.Size, position, locationVirtual, lane.Index);
                        OperationManager.AddOperationAndInvoke(
                                new AddStepNoteOperation(
                                    selectedAirHold,
                                    airAction));
                        Status.SelectedNote = airAction;
                    }
                    else if (selectedNote != null)
                    {
                        OperationManager.AddOperationAndInvoke(
                            new AddLongNoteOperation(
                                model,
                                new AirHold(
                                    selectedNote.Size,
                                    selectedNote.Position, 
                                    selectedNote.Location,
                                    lane.Index),
                                new AirUpC(selectedNote.Size,
                                    selectedNote.Position,
                                    selectedNote.Location,
                                    lane.Index),
                                selectedNote));
                    }
                    break;
                #endregion
                #region Air
                case NoteType.AIRUPC:
                    if (!Status.IsAirVisible) break;
                    selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirUpC air = new AirUpC(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        OperationManager.AddOperationAndInvoke(new AddAirNoteOperation(model, air, selectedNote));
                    }
                    break;
                case NoteType.AIRUPL:
                    if (!Status.IsAirVisible) break;
                    selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirUpL air = new AirUpL(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        OperationManager.AddOperationAndInvoke(new AddAirNoteOperation(model, air, selectedNote));
                    }
                    break;
                case NoteType.AIRUPR:
                    if (!Status.IsAirVisible) break;
                    selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirUpR air = new AirUpR(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        OperationManager.AddOperationAndInvoke(new AddAirNoteOperation(model, air, selectedNote));
                    }
                    break;
                case NoteType.AIRDOWNC:
                    if (!Status.IsAirVisible) break;
                    selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirDownC air = new AirDownC(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        OperationManager.AddOperationAndInvoke(new AddAirNoteOperation(model, air, selectedNote));
                    }
                    break;
                case NoteType.AIRDOWNL:
                    if (!Status.IsAirVisible) break;
                    selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirDownL air = new AirDownL(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        OperationManager.AddOperationAndInvoke(new AddAirNoteOperation(model, air, selectedNote));
                    }
                    break;
                case NoteType.AIRDOWNR:
                    if (!Status.IsAirVisible) break;
                    selectedNote = model.NoteBook.SelectedNote(location.Add(displayRect.Location)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirDownR air = new AirDownR(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        OperationManager.AddOperationAndInvoke(new AddAirNoteOperation(model, air, selectedNote));
                    }
                    break;
                #endregion
                #region AttributeNote
                case NoteType.BPM:
                    // すでに同一TickにBPMノーツが配置されていた場合は、今置こうとしているBPMノーツの値で上書きをする
                    var note = model.NoteBook.AttributeNotes.Find(x => x.Position.Tick == position.Tick && x is BPM);
                    if (note != null)
                    {
                        if (note.NoteValue != Status.CurrentValue)
                        {
                            OperationManager.AddOperationAndInvoke(new ChangeNoteValueOperation(
                            note,
                            note.NoteValue,
                            Status.CurrentValue));
                        }
                        else { }
                        return;
                    }
                    newNote = new BPM(position, locationVirtual, Status.CurrentValue, lane.Index);
                    break;
                case NoteType.HIGHSPEED:
                    // すでに同一TickにHighSpeedノーツが配置されていた場合は、今置こうとしているBPMノーツの値で上書きをする
                    note = model.NoteBook.AttributeNotes.Find(x => x.Position.Tick == position.Tick && x is HighSpeed);
                    if (note != null)
                    {
                        if (note.NoteValue != Status.CurrentValue)
                        {
                            OperationManager.AddOperationAndInvoke(new ChangeNoteValueOperation(
                            note,
                            note.NoteValue,
                            Status.CurrentValue));
                        }
                        else { }
                        return;
                    }
                    newNote = new HighSpeed(position, locationVirtual, Status.CurrentValue, lane.Index);
                    break;
                #endregion
                default:
                    break;
            }
            if (newNote != null)
            {
                OperationManager.AddOperationAndInvoke(new AddShortNoteOperation(model, newNote));
            }
        }

        #region 座標変換

        /// <summary>
        /// 与えられた座標を現在のグリッド情報に合わせて変換します
        /// 与えられる座標も返り値もXにdisplayRect.Xを足していない生のもの
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private Point PointToGrid(Point location, ScoreLane lane)
        {
            return PointToGrid(location, lane, Status.NoteSize);
        }

        /// <summary>
        /// 与えられた座標を現在のグリッド情報に合わせて変換します
        /// 選択したnoteのサイズが考慮されます
        /// 与えられる座標も返り値もXにdisplayRect.Xを足していない生のもの
        /// </summary>
        /// <param name="location"></param>
        /// <param name="lane"></param>
        /// <returns></returns>
        private Point PointToGrid(Point location, ScoreLane lane, int noteSize)
        {
            Point gridP = new Point();
            //HACK: 当たり判定のピクセル座標を調節のためlane.HitRect.Yに-1をする
            Point relativeP = new Point(
                location.X + displayRect.X - (int)lane.LaneRect.X, 
                location.Y + displayRect.Y - (int)(lane.HitRect.Y - 1 + lane.HitRect.Height));
            Point deltaP = new Point();
            float gridWidth = ScoreInfo.UnitLaneWidth * ScoreInfo.Lanes / Status.Grid;
            float gridHeight = ScoreInfo.UnitBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            float maxGridX = (ScoreInfo.Lanes - noteSize) * ScoreInfo.UnitLaneWidth;
            //現在の自由座標とそこから計算したグリッド座標の差分
            //deltaP.X = Math.Min((int)(Math.Floor(relativeP.X / gridWidth) * gridWidth), (int)maxGridX) - relativeP.X;
            deltaP.X = (int)(Math.Floor(relativeP.X / gridWidth) * gridWidth);
            if (deltaP.X < 0)
            {
                deltaP.X = 0;
            }
            else if (deltaP.X > (int)maxGridX)
            {
                deltaP.X = (int)maxGridX;
            }
            deltaP.X -= relativeP.X;
            deltaP.Y = (int)(Math.Ceiling(relativeP.Y / gridHeight) * gridHeight) - relativeP.Y;
            gridP.X = location.X + deltaP.X;
            gridP.Y = location.Y + deltaP.Y;
            //帰ってくる座標はXにdisplayRect.X足されていない生のもの
            return gridP;
        }
        #endregion

        /// <summary>
        /// レーン、譜面、ノーツなどをすべて描画します
        /// </summary>
        /// <param name="e"></param>
        public void PaintPanel(Graphics g)
		{
#if DEBUG
            sw.Start();
#endif
            var drawLaneBook = model.LaneBook.Where(
                x =>
                x.LaneRect.Right >= displayRect.X - ScoreLane.Margin.Right &&
                x.LaneRect.Left <= displayRect.X + pictureBox.Width + ScoreLane.Margin.Left)
                .ToList();
            drawLaneBook.ForEach(x => x.PaintLane(g, displayRect.Location));
            if (drawLaneBook.Any())
            {
                //現在の描画範囲にあるレーンの小節数の範囲を設定
                Status.DrawTickFirst = drawLaneBook.First().FirstScore.StartTick;
                Status.DrawTickLast = drawLaneBook.Last().LastScore.EndTick;
            }
            //ノーツ描画
            model.PaintNote(g, displayRect.Location);
            //プレビューノーツ描画
			pNote.Paint(g);
            //矩形選択領域描画
            if(Status.Mode == Mode.Edit)
            {
                selectionArea.Draw(g, model.LaneBook, displayRect.Location);
            }
#if DEBUG
            sw.Stop();
            float fps = 1000 / (float)sw.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine(fps);
            sw.Reset();
#endif
        }

        public void Refresh()
        {
            pictureBox.Refresh();
        }
    }
}