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

namespace NE4S.Scores
{
    /// <summary>
    /// MainFormのTabPageのScoreに貼るやつ
    /// </summary>
    public class ScorePanel
    {
        private Size panelSize;
        private int currentPositionX, currentWidthMax;
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
            model = new Model();
            this.hSBar = hSBar;
            hSBar.Minimum = 0;
			pNote = new PreviewNote();
#if DEBUG
            //*
            //SetScore(11, 4, 1);
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

        public Model GetModelForIO() => model;

        public void SetModelForIO(Model model) => this.model = model;

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
            ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.AddX(currentPositionX)));
            if (selectedLane != null && selectedLane.SelectedScore(e.Location.AddX(currentPositionX)) != null && e.Button == MouseButtons.Right && Status.Mode == Mode.EDIT)
            {
                new EditCMenu(this, selectedLane, selectedLane.SelectedScore(e.Location.AddX(currentPositionX))).Show(pBox, e.Location);
            }
        }

        public void MouseDown(MouseEventArgs e)
        {
			var laneBook = model.LaneBook;
			Status.IsMousePressed = true;
			ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.AddX(currentPositionX)));
            #region 座標などをコンソール出力（デバッグ時のみ）
#if DEBUG
            //デバッグ用にクリックした座標などをコンソールに出力する
            //本番では必要ない
            if (selectedLane != null && e.Button == MouseButtons.Left)
			{
                Point gridPoint = PointToGrid(e.Location, selectedLane);
                Position position = selectedLane.GetPos(gridPoint.AddX(currentPositionX));
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
            if (e.Button == MouseButtons.Left)
			{
                int noteArea = NoteArea.NONE;
                var selectedNote = model.NoteBook.SelectedNote(e.Location.AddX(currentPositionX), ref noteArea);
				switch (Status.Mode)
				{
					case Mode.ADD:
                        if(selectedLane != null)
                        {
                            AddNote(e.Location, selectedLane);
                        }
                        break;
					case Mode.EDIT:
                        //Airは単体で動かせないようにする
                        if (selectedNote is Air)
                        {
                            selectedNote = null;
                        }
                        if (selectedNote != null)
                        {
                            Status.SelectedNote = selectedNote;
                            Status.SelectedNoteArea = noteArea;
                        }
                        if (selectedNote is SlideRelay && !Status.IsSlideRelayVisible)
                        {
                            Status.SelectedNote = null;
                        }
                        if (selectedNote is SlideCurve && !Status.IsSlideCurveVisible)
                        {
                            Status.SelectedNote = null;
                        }
                        break;
					case Mode.DELETE:
                        if (selectedNote != null)
                        {
                            model.NoteBook.Delete(selectedNote);
                        }
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
				case Mode.ADD:
					ScoreLane selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.AddX(currentPositionX)));
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
                    //ロングノーツを置いたときに終点をそのまま移動できるようにとりあえずほぼそのままコピペ
                    //PointToGridのオーバーロードが違うだけ
                    //動いた
                    if (Status.IsMousePressed && e.Button == MouseButtons.Left && Status.SelectedNote != null && selectedLane != null)
                    {
                        Point physicalGridPoint = PointToGrid(e.Location, selectedLane);
                        Point virtualGridPoint = physicalGridPoint.AddX(currentPositionX);
                        Position newPos = selectedLane.GetPos(virtualGridPoint);
                        Status.SelectedNote.Relocate(newPos, virtualGridPoint);
                        //ロングノーツで使うのでどのレーンにノーツが乗ってるかちゃんと更新する
                        Status.SelectedNote.LaneIndex = selectedLane.Index;
                    }
                    break;
				case Mode.EDIT:
					selectedLane = laneBook.Find(x => x.HitRect.Contains(e.Location.AddX(currentPositionX)));
					if (Status.IsMousePressed && e.Button == MouseButtons.Left && Status.SelectedNote != null && selectedLane != null)
					{
                        switch (Status.SelectedNoteArea)
                        {
                            case NoteArea.LEFT:
                                {
                                    if (Status.SelectedNote.LaneIndex != selectedLane.Index) return;
                                    Point virtualGridPoint = PointToGrid(e.Location, selectedLane, 0).AddX(currentPositionX);
                                    int newSize = (int)((Status.SelectedNote.Location.X + Status.SelectedNote.Width - virtualGridPoint.X) / ScoreInfo.MinLaneWidth);
                                    if (newSize <= 0) newSize = 1;
                                    else if (newSize > 16) newSize = 16;
                                    Status.SelectedNote.ReSize(newSize);
                                }
                                break;
                            case NoteArea.CENTER:
                                {
                                    //ノーツのサイズを考慮したほうのメソッドを使う
                                    Point physicalGridPoint = PointToGrid(e.Location, selectedLane, Status.SelectedNote.Size);
                                    Point virtualGridPoint = physicalGridPoint.AddX(currentPositionX);
                                    Position newPos = selectedLane.GetPos(virtualGridPoint);
                                    Status.SelectedNote.Relocate(newPos, virtualGridPoint);
                                    //ロングノーツで使うのでどのレーンにノーツが乗ってるかちゃんと更新する
                                    Status.SelectedNote.LaneIndex = selectedLane.Index;
                                }
                                break;
                            case NoteArea.RIGHT:
                                {
                                    if (Status.SelectedNote.LaneIndex != selectedLane.Index) return;
                                    Point virtualGridPoint = PointToGrid(e.Location, selectedLane, 0).AddX(currentPositionX);
                                    int newSize = (int)((virtualGridPoint.X - Status.SelectedNote.Location.X) / ScoreInfo.MinLaneWidth);
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
					break;
				case Mode.DELETE:
                    var selectedNote = model.NoteBook.SelectedNote(e.Location.AddX(currentPositionX));
                    if(Status.IsMousePressed && selectedNote != null)
                    {
                        model.NoteBook.Delete(selectedNote);
                    }
                    break;
				default:
					break;
			}
        }

        public void MouseUp(MouseEventArgs e)
        { 
			Status.IsMousePressed = false;
            Status.SelectedNote = null;
            Status.SelectedNoteArea = NoteArea.NONE;
		}

        public void MouseEnter(EventArgs e) { }

        public void MouseLeave(EventArgs e) { }

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

        private void AddNote(Point location, ScoreLane lane)
        {
            //与えられた自由物理座標からグリッド仮想座標とポジション座標を作成
            Point gridPoint = PointToGrid(location, lane);
            Position position = lane.GetPos(gridPoint.AddX(currentPositionX));
            PointF locationVirtual = gridPoint.AddX(currentPositionX);

            Note newNote = null;
            switch (Status.Note)
            {
                case NoteType.TAP:
                    newNote = new Tap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.EXTAP:
                    newNote = new ExTap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.AWEXTAP:
                    newNote = new AwesomeExTap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.HELL:
                    newNote = new HellTap(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.FLICK:
                    newNote = new Flick(Status.NoteSize, position, locationVirtual, lane.Index);
                    break;
                case NoteType.HOLD:
                    model.AddLongNote(new Hold(Status.NoteSize, position, locationVirtual, lane.Index));
                    break;
                case NoteType.SLIDE:
                    //Slideとの当たり判定は自由仮想座標を使う
                    Slide selectedSlide = model.SelectedSlide(location.AddX(currentPositionX));
                    if(selectedSlide != null)
                    {
                        if (Status.InvisibleSlideTap)
                        {
                            SlideRelay slideRelay = new SlideRelay(Status.NoteSize, position, locationVirtual, lane.Index);
                            selectedSlide.Add(slideRelay);
                            Status.SelectedNote = slideRelay;
                        }
                        else
                        {
                            SlideTap slideTap = new SlideTap(Status.NoteSize, position, locationVirtual, lane.Index);
                            selectedSlide.Add(slideTap);
                            Status.SelectedNote = slideTap;
                        }
                    }
                    else
                    {
                        model.AddLongNote(new Slide(Status.NoteSize, position, locationVirtual, lane.Index));
                    }
                    break;
                case NoteType.SLIDECURVE:
                    selectedSlide = model.SelectedSlide(location.AddX(currentPositionX));
                    if (selectedSlide != null)
                    {
                        SlideCurve slideCurve = new SlideCurve(Status.NoteSize, position, locationVirtual, lane.Index);
                        selectedSlide.Add(slideCurve);
                        Status.SelectedNote = slideCurve;
                    }
                    break;
                case NoteType.AIRHOLD:
                    AirHold selectedAirHold = model.SelectedAirHold(location.AddX(currentPositionX));
                    var selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedAirHold != null)
                    {
                        AirAction airAction = new AirAction(selectedAirHold.Size, position, locationVirtual, lane.Index);
                        selectedAirHold.Add(airAction);
                        Status.SelectedNote = airAction;
                    }
                    if (selectedNote != null && !selectedNote.IsAirHoldAttached)
                    {
                        AirHold airHold = new AirHold(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddLongNote(airHold);
                        selectedNote.AttachAirHold(airHold);
                    }
                    if (selectedNote != null && !selectedNote.IsAirAttached) { 
                        AirUpC air = new AirUpC(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
                    break;
                case NoteType.AIRUPC:
                    selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirUpC air = new AirUpC(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
                    break;
                case NoteType.AIRUPL:
                    selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirUpL air = new AirUpL(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
                    break;
                case NoteType.AIRUPR:
                    selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirUpR air = new AirUpR(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
                    break;
                case NoteType.AIRDOWNC:
                    selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirDownC air = new AirDownC(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
                    break;
                case NoteType.AIRDOWNL:
                    selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirDownL air = new AirDownL(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
                    break;
                case NoteType.AIRDOWNR:
                    selectedNote = model.NoteBook.SelectedNote(location.AddX(currentPositionX)) as AirableNote;
                    if (selectedNote != null && !selectedNote.IsAirAttached)
                    {
                        AirDownR air = new AirDownR(selectedNote.Size, selectedNote.Position, selectedNote.Location, lane.Index);
                        model.AddNote(air);
                        selectedNote.AttachAir(air);
                    }
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
            return PointToGrid(location, lane, Status.NoteSize);
        }

        /// <summary>
        /// 与えられた座標を現在のグリッド情報に合わせて変換します
        /// 選択したnoteのサイズが考慮されます
        /// 与えられる座標も返り値もXにcurrentPositionXを足していない生のもの
        /// </summary>
        /// <param name="location"></param>
        /// <param name="lane"></param>
        /// <returns></returns>
        private Point PointToGrid(Point location, ScoreLane lane, int noteSize)
        {
            Point gridP = new Point();
            //HACK: 当たり判定のピクセル座標を調節のためlane.HitRect.Yに-1をする
            Point relativeP = new Point(location.X + currentPositionX - (int)lane.HitRect.X, location.Y - (int)(lane.HitRect.Y - 1 + lane.HitRect.Height));
            Point deltaP = new Point();
            float gridWidth = ScoreInfo.MinLaneWidth * ScoreInfo.Lanes / Status.Grid;
            float gridHeight = ScoreInfo.MaxBeatHeight * ScoreInfo.MaxBeatDiv / Status.Beat;
            float maxGridX = (ScoreInfo.Lanes - noteSize) * ScoreInfo.MinLaneWidth;
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
            var drawLaneBook = model.LaneBook.Where(
                x =>
                x.HitRect.Right >= currentPositionX - ScoreInfo.LaneMargin.Right &&
                x.HitRect.Left <= currentPositionX + pBox.Width + ScoreInfo.LaneMargin.Left)
                .ToList();
            drawLaneBook.ForEach(x => x.PaintLane(e, originPosX, originPosY));
            if (drawLaneBook.Any())
            {
                //現在の描画範囲にあるレーンの小節数の範囲を設定
                Status.DrawTickFirst = drawLaneBook.First().FirstScore;
                Status.DrawTickLast = drawLaneBook.Last().LastScore;
            }
            //ノーツ描画
            model.PaintNote(e, originPosX, originPosY, currentPositionX);
            //プレビューノーツ描画
			pNote.Paint(e);
		}
    }
}