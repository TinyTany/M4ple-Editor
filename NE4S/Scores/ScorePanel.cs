using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using NE4S.Component;
using NE4S.Notes;

namespace NE4S.Scores
{
    /// <summary>
    /// MainFormのTabPageのScoreに貼るやつ
    /// </summary>
    public class ScorePanel
    {
        private Size panelSize;
        private int currentPositionX, currentWidthMax;
		private readonly NoteMaterialBook noteMaterialBook;
        private Model model;
        private HScrollBar hSBar;
        private PictureBox pBox;
        private PreviewNote pNote;

        class Margin
        {
            public static int
                Top = ScoreInfo.PanelMargin.Top,
                Bottom = ScoreInfo.PanelMargin.Bottom,
                Left = ScoreInfo.PanelMargin.Left,
                Right = ScoreInfo.PanelMargin.Right;
        }

        public ScorePanel(PictureBox pBox, HScrollBar hSBar)
        {
            this.pBox = pBox;
            panelSize = pBox.Size;
            currentPositionX = 0;
            currentWidthMax = 0;
			noteMaterialBook = new NoteMaterialBook();
            model = new Model();
            this.hSBar = hSBar;
            hSBar.Minimum = 0;
			pNote = new PreviewNote();
#if DEBUG
			//*
			SetScore(4, 4, 10);
            SetScore(3, 4, 5);
            SetScore(6, 8, 8);
            //
            SetScore(2, 64, 32);
            SetScore(13, 4, 2);
            SetScore(2, 4, 8);
            SetScore(26, 8, 2);
            SetScore(1, 32, 32);
            //
            SetScore(7, 1, 1);
            SetScore(4, 4, 1);
            SetScore(8, 8, 1);
            SetScore(16, 16, 1);
			//*/
			//SetScore(4, 4, 1000);
#else
			SetScore(4, 4, 200);
#endif
		}

		#region laneBookを触る用メソッド群
		/// <summary>
		/// 末尾に指定した拍子数の譜面を指定した個数追加
		/// </summary>
		/// <param name="beatNumer">拍子分子</param>
		/// <param name="beatDenom">拍子分母</param>
		/// <param name="barCount">個数</param>
		private void SetScore(int beatNumer, int beatDenom, int barCount)
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
            model.InsertScoreForward(score, beatNumer, beatDenom, barCount);
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
            model.InsertScoreBackward(score, beatNumer, beatDenom, barCount);
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

        /// <summary>
        /// 指定されたscoreからcount個のScoreを削除
        /// </summary>
        /// <param name="score">削除開始のScore</param>
        /// <param name="count">削除する個数</param>
        public void DeleteScore(Score score, int count)
        {
            model.DeleteScore(score, count);
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
            currentWidthMax = (int)(ScoreLane.Width + Margin.Left + Margin.Right) * laneBook.Count;
            hSBar.Maximum = currentWidthMax < panelSize.Width ? 0 : currentWidthMax - panelSize.Width;
            //pBoxを更新
            pBox.Refresh();
        }

        #region マウス入力とかに反応して処理するメソッドたち  

        public void MouseClick(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
            //クリックされたレーンを特定
            ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
            if (selectedLane != null && selectedLane.SelectedScore(currentPositionX + e.X, e.Y) != null && e.Button == MouseButtons.Right && Status.Mode == Define.EDIT)
            {
                new EditCMenu(this, selectedLane, selectedLane.SelectedScore(currentPositionX + e.X, e.Y)).Show(pBox, e.Location);
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
			Status.IsMousePressed = true;
			ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
            #region 座標などをコンソール出力
#if DEBUG
            //デバッグ用にクリックした座標などをコンソールに出力する
            //本番では必要ない
            if (selectedLane != null && e.Button == MouseButtons.Left)
			{
                Point gridPoint = PointToGrid(e.Location, selectedLane);
				if(selectedLane.GetPos(gridPoint.X + currentPositionX, gridPoint.Y) != null)
				{
					selectedLane.GetPos(gridPoint.X + currentPositionX, gridPoint.Y).PrintPos();
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("MouseDown(MouseEventArgs) : selectedLane.GetPos = null");
				}
			}
#endif
            #endregion
            if (selectedLane != null && e.Button == MouseButtons.Left)
			{
                Note selectedNote = model.NoteBook.SelectedNote(new PointF(currentPositionX + e.X, e.Y));
				switch (Status.Mode)
				{
					case Define.ADD:
                        /*
                        //破壊テスト
                        for(int i = 0; i < 1000; ++i)
                        {
                            //*/
                            Point gridPoint = PointToGrid(e.Location, selectedLane);
                            Position position = selectedLane.GetPos(gridPoint.X + currentPositionX, gridPoint.Y);
                            AddNote(new PointF(gridPoint.X + currentPositionX, gridPoint.Y), position, selectedLane.Index);
                        //}
						
						break;
					case Define.EDIT:
                        if (selectedNote != null) Status.selectedNote = selectedNote;
						break;
					case Define.DELETE:
                        if (selectedNote != null) model.NoteBook.Delete(selectedNote);
						break;
					default:
						break;
				}
			}
            if (selectedLane == null) System.Diagnostics.Debug.WriteLine("MouseDown(MouseEventArgs) : selectedLane = null");
		}

        public void MouseMove(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
			switch (Status.Mode)
			{
				case Define.ADD:
					ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
					if (selectedLane != null)
					{
						pNote.Location = PointToGrid(e.Location, selectedLane);
						pNote.Visible = true;
					}
					else
					{
						pNote.Visible = false;
					}
					if (selectedLane != null && Status.IsMousePressed)
					{
						
					}
                    //ロングノーツを置いたときに終点をそのまま移動できるようにとりあえずそのままコピペ
                    //動いた
                    if (Status.IsMousePressed && e.Button == MouseButtons.Left && Status.selectedNote != null && selectedLane != null)
                    {
                        Point physicalGridPoint = PointToGrid(e.Location, selectedLane);
                        Point virtualGridPoint = new Point(
                            physicalGridPoint.X + currentPositionX,
                            physicalGridPoint.Y);
                        Position newPos = selectedLane.GetPos(virtualGridPoint);
                        Status.selectedNote.Relocate(newPos, virtualGridPoint);
                        //ロングノーツで使うのでどのレーンにノーツが乗ってるかちゃんと更新する
                        Status.selectedNote.LaneIndex = selectedLane.Index;
                    }
                    break;
				case Define.EDIT:
					selectedLane = laneBook.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
					if (Status.IsMousePressed && e.Button == MouseButtons.Left && Status.selectedNote != null && selectedLane != null)
					{
						Point physicalGridPoint = PointToGrid(e.Location, selectedLane);
						Point virtualGridPoint = new Point(
							physicalGridPoint.X + currentPositionX,
							physicalGridPoint.Y);
						Position newPos = selectedLane.GetPos(virtualGridPoint);
						Status.selectedNote.Relocate(newPos, virtualGridPoint);
                        //ロングノーツで使うのでどのレーンにノーツが乗ってるかちゃんと更新する
                        Status.selectedNote.LaneIndex = selectedLane.Index;
                    }
					break;
				case Define.DELETE:
					break;
				default:
					break;
			}
        }

        public void MouseUp(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
			Status.IsMousePressed = false;
            Status.selectedNote = null;

			ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
			if (selectedLane != null && e.Button == MouseButtons.Left)
			{
				switch (Status.Mode)
				{
					case Define.ADD:
						
						break;
					case Define.EDIT:
						break;
					case Define.DELETE:
						break;
					default:
						break;
				}
			}
		}

        public void MouseEnter(EventArgs e)
        {

        }

        public void MouseLeave(EventArgs e)
        {

        }

        public void MouseScroll(int delta)
        {
            currentPositionX -= delta;
            if (currentPositionX < hSBar.Minimum) currentPositionX = hSBar.Minimum;
            else if (hSBar.Maximum < currentPositionX) currentPositionX = hSBar.Maximum;
            hSBar.Value = currentPositionX;
        }

        public void HSBarScroll(ScrollEventArgs e)
        {
            currentPositionX += (e.NewValue - e.OldValue);
        }
        #endregion

        private void AddNote(PointF locationVirtual, Position position, int laneIndex)
        {
            Note newNote = null;
            switch (Status.Note)
            {
                case Define.TAP:
                    newNote = new Tap(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.EXTAP:
                    newNote = new ExTap(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.AWEXTAP:
                    newNote = new AwesomeExTap(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.HELL:
                    newNote = new HellTap(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.FLICK:
                    newNote = new Flick(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.HOLD:
                    break;
                case Define.SLIDE:
                    //testように直書き
                    model.AddLongNote(new Slide(Status.NoteSize, position, locationVirtual, laneIndex));
                    break;
                case Define.SLIDECURVE:
                    break;
                case Define.AIRHOLD:
                    break;
                case Define.AIRUPC:
                    newNote = new AirUpC(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.AIRUPL:
                    newNote = new AirUpL(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.AIRUPR:
                    newNote = new AirUpR(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.AIRDOWNC:
                    newNote = new AirDownC(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.AIRDOWNL:
                    newNote = new AirDownL(Status.NoteSize, position, locationVirtual);
                    break;
                case Define.AIRDOWNR:
                    newNote = new AirDownR(Status.NoteSize, position, locationVirtual);
                    break;
                default:
                    break;
            }
            if (newNote != null) model.AddNote(newNote);
            return;
        }

        /// <summary>
        /// 与えられた座標を現在のグリッド情報に合わせて変換します
        /// 与えられる座標も返り値もXにcurrentPositionXを足していない生のもの
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private Point PointToGrid(Point location, ScoreLane lane)
        {
            Point gridP = new Point();
			//HACK: 当たり判定のピクセル座標を調節のためlane.HitRect.Yに-1をする
            Point relativeP = new Point(location.X + currentPositionX - (int)lane.HitRect.X, location.Y - (int)(lane.HitRect.Y - 1));
            Point deltaP = new Point();
			float gridWidth = ScoreInfo.MinLaneWidth * ScoreInfo.Lanes / Status.Grid;
			float gridHeight = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
			float maxGridX = (ScoreInfo.Lanes - Status.NoteSize) * ScoreInfo.MinLaneWidth;
			//現在の自由座標とそこから計算したグリッド座標の差分
			deltaP.X = Math.Min((int)(Math.Floor(relativeP.X / gridWidth) * gridWidth), (int)maxGridX) - relativeP.X;
			deltaP.Y = (int)(Math.Ceiling(relativeP.Y / gridHeight) * gridHeight) - relativeP.Y;
            gridP.X = location.X + deltaP.X;
            gridP.Y = location.Y + deltaP.Y;
            //帰ってくる座標はXにcurrentPositionX足されていない生のもの
            return gridP;
        }

		public void PaintPanel(PaintEventArgs e)
		{
			//PictureBox上の原点に対応する現在の仮想譜面座標の座標を設定
			int originPosX = currentPositionX;
			int originPosY = 0;
			var laneBook = model.LaneBook;
			for (int i = 0; i < laneBook.Count; ++i)
            {
                //ScoreLaneが表示範囲内にあるか
                if (currentPositionX < (ScoreLane.Width + Margin.Left + Margin.Right) * (i + 1) &&
					currentPositionX + panelSize.Width > (ScoreLane.Width + Margin.Left + Margin.Right) * i)
                {
                    //ScoreLaneを描画
                    laneBook[i].PaintLane(e, originPosX, originPosY);
				}
            }
#if DEBUG
			model.PaintNote(e, originPosX, originPosY, currentPositionX);
#endif
			pNote.Paint(e);
		}
    }
}