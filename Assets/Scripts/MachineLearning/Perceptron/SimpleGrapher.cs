using UnityEngine;
using UnityEngine.UI;

// Written by Penny de Byl, refactored by TheAshenWolf

namespace MachineLearning.Perceptron
{
	public class SimpleGrapher : MonoBehaviour {

		public int scale = 500;
		public int xOffset = 100;
		public int yOffset = 100;
		private RawImage _rend;
		private Texture2D _tex;
		private Texture2D _graph;
		private Color[] _colours;

		private void Circle (int cx, int cy, int r, Color col) 
		{
			int y = r;
			float d = 1/4.0f - r;
			float end = Mathf.Ceil(r/Mathf.Sqrt(2));
	   
			for (int x = 0; x <= end; x++) {
				_tex.SetPixel(cx+x, cy+y, col);
				_tex.SetPixel(cx+x, cy-y, col);
				_tex.SetPixel(cx-x, cy+y, col);
				_tex.SetPixel(cx-x, cy-y, col);
				_tex.SetPixel(cx+y, cy+x, col);
				_tex.SetPixel(cx-y, cy+x, col);
				_tex.SetPixel(cx+y, cy-x, col);
				_tex.SetPixel(cx-y, cy-x, col);
	       
				d += 2*x+1;
				if (d > 0) {
					d += 2 - 2*y--;
				}
			}
		}

		public void DrawLine(float x, float y, float x2, float y2, Color c)
		{
			x = x * scale + xOffset;
			y = y * scale + yOffset;
			x2 = x2 * scale + xOffset;
			y2 = y2 * scale + yOffset;
			Circle((int)x,(int)y,10,Color.red);
			Circle((int)x2,(int)y2,10,Color.red);

			int w = (int)(x2 - x);
			int h = (int)(y2 - y);
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
			if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
			if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
			if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
			int longest = Mathf.Abs(w) ;
			int shortest = Mathf.Abs(h) ;
			if (!(longest>shortest)) {
				longest = Mathf.Abs(h) ;
				shortest = Mathf.Abs(w) ;
				if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
				dx2 = 0 ;            
			}
			int numerator = longest >> 1 ;
			for (int i=0;i<=longest;i++) {
				_tex.SetPixel((int)x,(int)y,c) ;
				numerator += shortest ;
				if (!(numerator<longest)) {
					numerator -= longest ;
					x += dx1 ;
					y += dy1 ;
				} else {
					x += dx2 ;
					y += dy2 ;
				}
			}
			_tex.Apply();
			_rend.texture = _tex;
		}

		public void DrawRay(float slope, float intercept, Color c)
		{
			//y = mx + c
			float x = 0 + xOffset;
			float y = (intercept * scale) + yOffset;

			float x2 = 600 + xOffset;
			float y2 = slope * x2 + (intercept * scale) + yOffset;

			//Circle((int)x,(int)y,10,Color.green);
			//Circle((int)x2,(int)y2,10,Color.green);

			int w = (int)(x2 - x);
			int h = (int)(y2 - y);
			int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
			if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
			if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
			if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
			int longest = Mathf.Abs(w) ;
			int shortest = Mathf.Abs(h) ;
			if (!(longest>shortest)) {
				longest = Mathf.Abs(h) ;
				shortest = Mathf.Abs(w) ;
				if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
				dx2 = 0 ;            
			}
			int numerator = longest >> 1 ;
			for (int i=0;i<=longest;i++) {
				_tex.SetPixel((int)x,(int)y,c) ;
				numerator += shortest ;
				if (!(numerator<longest)) {
					numerator -= longest ;
					x += dx1 ;
					y += dy1 ;
				} else {
					x += dx2 ;
					y += dy2 ;
				}
			}
			_tex.Apply();
			_rend.texture = _tex;
		}

		public void DrawPoint(float x, float y, Color c)
		{
			Circle((int)(x*scale)+xOffset,(int)(y*scale)+yOffset,5,c);
			_tex.Apply();
			_rend.texture = _tex;
		}

		private void DrawAxis(Color c)
		{
			for(int x = 0; x < 600; x++)
				_tex.SetPixel(x, yOffset, c);
			for(int y = 0; y < 600; y++)
				_tex.SetPixel(xOffset, y, c);
		}

		// Use this for initialization
		private void Start () {
			_rend = this.GetComponent<RawImage>();
			_tex = _rend.texture as Texture2D;
			
			if (_tex == null) return;

			_colours = _tex.GetPixels(0);
			for(int i = 0; i < _colours.Length; i++)
				_colours[i] = Color.black;

			_tex.SetPixels(_colours,0);
			DrawAxis(Color.white);
		
			_tex.Apply();
			_rend.texture = _tex;
		}
	}
}
