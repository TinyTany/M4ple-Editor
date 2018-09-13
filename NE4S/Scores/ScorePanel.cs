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
        private LaneBook lanes;
        private Model model;
        private HScrollBar hSBar;
        private PictureBox pBox;
#if DEBUG
        //private Rectangle rec;
        private PreviewNote pNote;
#endif

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
            lanes = new LaneBook();
            model = new Model();
            this.hSBar = hSBar;
            hSBar.Minimum = 0;
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
            
            pNote = new PreviewNote();
#endif
        }

        /// <summary>
        /// 末尾に指定した拍子数の譜面を指定した個数追加
        /// </summary>
        /// <param name="beatNumer">拍子分子</param>
        /// <param name="beatDenom">拍子分母</param>
        /// <param name="barCount">個数</param>
        private void SetScore(int beatNumer, int beatDenom, int barCount)
        {
            lanes.SetScore(model, beatNumer, beatDenom, barCount);
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
            lanes.InsetScoreForward(model, score, beatNumer, beatDenom, barCount);
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
            lanes.InsertScoreBackward(model, score, beatNumer, beatDenom, barCount);
            Update();
        }

        /// <summary>
        /// 指定したscoreとその1つ前のScoreでレーンを2つに分割する
        /// </summary>
        /// <param name="score"></param>
        public void DivideLane(Score score)
        {
            lanes.DivideLane(score);
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
            lanes.DeleteScore(model, score, count);
            Update();
        }

        /// <summary>
        /// レーン全体を詰める
        /// </summary>
        public void FillLane()
        {
            if(lanes.Any()) FillLane(lanes.First());
        }

        /// <summary>
        /// begin以降のレーンを詰める
        /// </summary>
        /// <param name="begin"></param>
        public void FillLane(ScoreLane begin)
        {
            lanes.FillLane(begin);
            Update();
        }

        private void Update()
        {
            currentWidthMax = (int)(ScoreLane.Width + Margin.Left + Margin.Right) * lanes.Count;
            hSBar.Maximum = currentWidthMax < panelSize.Width ? 0 : currentWidthMax - panelSize.Width;
            //pBoxを更新
            pBox.Refresh();
        }

        public void MouseClick(MouseEventArgs e)
        {
#if DEBUG
            //クリックされたレーンを特定
            ScoreLane selectedLane = lanes.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
            if (selectedLane != null && selectedLane.SelectedScore(e) != null && e.Button == MouseButtons.Right)
            {
                new EditCMenu(this, selectedLane, selectedLane.SelectedScore(e)).Show(pBox, e.Location);
            }
#endif
        }

        public void MouseDown(MouseEventArgs e)
        {

        }

        public void MouseMove(MouseEventArgs e)
        {
            if(Status.Mode == Define.ADD)
            {
                ScoreLane selectedLane = lanes.Find(x => x.HitRect.Contains(currentPositionX + e.X, e.Y));
                if (selectedLane != null)
                {
                    pNote.Location = PointToGrid(e.Location, selectedLane);
                    pNote.Visible = true;
                }else
                {
                    pNote.Visible = false;
                }
            }
        }

        public void MouseUp(MouseEventArgs e)
        {

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

        /// <summary>
        /// 与えられた座標を現在のグリッド情報に合わせて変換します
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Point PointToGrid(Point p, ScoreLane lane)
        {
            Point gridP = new Point();
            Point relativeP = new Point(p.X + currentPositionX - (int)lane.HitRect.X, p.Y - (int)lane.HitRect.Y);
            Point deltaP = new Point();
            //後で直す
            deltaP.X = 0;
            gridP.X = p.X + deltaP.X;
            gridP.Y = p.Y + deltaP.Y;
            return gridP;
        }

        public void PaintPanel(PaintEventArgs e)
        {
            for(int i = 0; i < lanes.Count; ++i)
            {
                //ScoreLaneが表示範囲内にあるか
                if (currentPositionX < (ScoreLane.Width + Margin.Left + Margin.Right) * (i + 1) && 
                    (ScoreLane.Width + Margin.Left + Margin.Right) * i < currentPositionX + panelSize.Width)
                {
                    //ScoreLaneの相対位置のX座標を設定
                    int drawPosX = ((int)ScoreLane.Width + Margin.Left + Margin.Right) * i - currentPositionX + Margin.Left;
                    int drawPosY = Margin.Top;
                    //ScoreLaneを描画
                    lanes[i].PaintLane(e, drawPosX, drawPosY);
                }
            }
#if DEBUG
            //e.Graphics.FillRectangle(Brushes.Red, rec);
            pNote.Paint(e);
#endif
        }
    }
}
