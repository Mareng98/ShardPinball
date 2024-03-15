/*
*
*   This is the implementation of the Simple Directmedia Layer through C#.   This isn't a course on 
*       graphics, so we're not going to roll our own implementation.   If you wanted to replace it with 
*       something using OpenGL, that'd be a pretty good extension to the base Shard engine.
*       
*   Note that it extends from DisplayText, which also uses SDL.  
*   
*   @author Michael Heron
*   @version 1.0
*   
*/

using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace Shard
{

    class Line
    {
        private int sx, sy;
        private int ex, ey;
        private int r, g, b, a;

        public int Sx { get => sx; set => sx = value; }
        public int Sy { get => sy; set => sy = value; }
        public int Ex { get => ex; set => ex = value; }
        public int Ey { get => ey; set => ey = value; }
        public int R { get => r; set => r = value; }
        public int G { get => g; set => g = value; }
        public int B { get => b; set => b = value; }
        public int A { get => a; set => a = value; }
    }

    class Circle
    {
        int x, y, rad;
        private int r, g, b, a;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Radius { get => rad; set => rad = value; }
        public int R { get => r; set => r = value; }
        public int G { get => g; set => g = value; }
        public int B { get => b; set => b = value; }
        public int A { get => a; set => a = value; }
    }


    class DisplaySDL : DisplayText
    {
        private List<Transform> _toDraw;
        private List<Line> _linesToDraw;
        private List<Circle> _circlesToDraw;
        private List<SDL.SDL_Vertex[]> _polygonsToDraw;
        private Dictionary<string, IntPtr> spriteBuffer;

        // fix this definition later, for now it will work
        // Transform, radius
        public override void initialize()
        {
            spriteBuffer = new Dictionary<string, IntPtr>();

            base.initialize();

            _toDraw = new List<Transform>();
            _linesToDraw = new List<Line>();
            _circlesToDraw = new List<Circle>();
            _polygonsToDraw = new List<SDL.SDL_Vertex[]>();

        }

        public IntPtr loadTexture(Transform trans)
        {
            IntPtr ret;
            uint format;
            int access;
            int w;
            int h;

            ret = loadTexture(trans.SpritePath);

            SDL.SDL_QueryTexture(ret, out format, out access, out w, out h);
            trans.Ht = h;
            trans.Wid = w;
            trans.recalculateCentre();

            return ret;

        }


        public IntPtr loadTexture(string path)
        {
            IntPtr img;

            if (spriteBuffer.ContainsKey(path))
            {
                return spriteBuffer[path];
            }

            img = SDL_image.IMG_Load(path);

            Debug.getInstance().log("IMG_Load: " + SDL_image.IMG_GetError());

            spriteBuffer[path] = SDL.SDL_CreateTextureFromSurface(_rend, img);

            SDL.SDL_SetTextureBlendMode(spriteBuffer[path], SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            return spriteBuffer[path];

        }


        public override void addToDraw(GameObject gob)
        {
            _toDraw.Add(gob.Transform);

            if (gob.Transform.SpritePath == null)
            {
                return;
            }

            loadTexture(gob.Transform.SpritePath);
        }

        public override void removeToDraw(GameObject gob)
        {
            _toDraw.Remove(gob.Transform);
        }

        public override void drawCircle(int x, int y, int rad, int r, int g, int b, int a)
        {
            Circle c = new Circle();

            c.X = x;
            c.Y = y;
            c.Radius = rad;

            c.R = r;
            c.G = g;
            c.B = b;
            c.A = a;

            _circlesToDraw.Add(c);
        }
        public override void drawLine(int x, int y, int x2, int y2, int r, int g, int b, int a)
        {
            Line l = new Line();
            l.Sx = x;
            l.Sy = y;
            l.Ex = x2;
            l.Ey = y2;

            l.R = r;
            l.G = g;
            l.B = b;
            l.A = a;

            _linesToDraw.Add(l);
        }



        // Helper method to add a triangle to _polygonsToDraw
        void AddTriangleToDraw(Vector2[] triangle, Color color)
        {
            SDL.SDL_Vertex[] sdlVertices = new SDL.SDL_Vertex[3];

            for (int i = 0; i < 3; i++)
            {
                Vector2 v = triangle[i];
                sdlVertices[i] = new SDL.SDL_Vertex
                {
                    position = new SDL.SDL_FPoint { x = v.X, y = v.Y },
                    color = new SDL.SDL_Color { r = color.R, g = color.G, b = color.B, a = color.A },
                    tex_coord = new SDL.SDL_FPoint { x = 1, y = 1 }
                };
            }

            _polygonsToDraw.Add(sdlVertices);
        }

        /// <summary>
        /// Renders a Polygon with no intersecting lines, or vertex that is on a straight line between two other vertices.
        /// </summary>
        /// <param name="vertices">The vertices to render from.</param>
        public override void renderGeometry(Vector2[] vertices, Color color)
        {

            if (vertices is null)
            {
                throw new ArgumentNullException("vertices");
            }
            else if (vertices.Length < 3)
            {
                throw new ArgumentOutOfRangeException("needs at least 3 vertices");
            }
            else if (vertices.Length == 3)
            {
                AddTriangleToDraw(vertices, color);
                return;
            }

            // Perform ear clipping
            List<Vector2[]> triangles = Triangulator.Triangulate(vertices);
            foreach (Vector2[] triangle in triangles)
            {
                AddTriangleToDraw(triangle, color);
            }
        }

        public override void display()
        {

            SDL.SDL_Rect sRect;
            SDL.SDL_Rect tRect;

            foreach (SDL.SDL_Vertex[] polygon in _polygonsToDraw)
            {
                SDL.SDL_SetRenderDrawColor(_rend, 255, 255, 255, 255);
                int[] indices = { 0, 1, 2 };
                SDL.SDL_RenderGeometry(_rend, nint.Zero, polygon, polygon.Length, indices, 3);
            }

            foreach (Transform trans in _toDraw)
            {

                if (trans.SpritePath == null)
                {
                    continue;
                }

                var sprite = loadTexture(trans);

                sRect.x = 0;
                sRect.y = 0;
                sRect.w = (int)(trans.Wid * trans.Scalex);
                sRect.h = (int)(trans.Ht * trans.Scaley);

                tRect.x = (int)trans.X;
                tRect.y = (int)trans.Y;
                tRect.w = sRect.w;
                tRect.h = sRect.h;

                SDL.SDL_RenderCopyEx(_rend, sprite, ref sRect, ref tRect, (int)trans.Rotz, IntPtr.Zero, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
            }

            foreach (Circle c in _circlesToDraw)
            {
                SDL.SDL_SetRenderDrawColor(_rend, (byte)c.R, (byte)c.G, (byte)c.B, (byte)c.A);
                renderCircle(c.X, c.Y, c.Radius);
            }

            foreach (Line l in _linesToDraw)
            {
                SDL.SDL_SetRenderDrawColor(_rend, (byte)l.R, (byte)l.G, (byte)l.B, (byte)l.A);
                SDL.SDL_RenderDrawLine(_rend, l.Sx, l.Sy, l.Ex, l.Ey);
            }

            // Show it off.
            base.display();


        }

        public override void drawLightMap(Color shadowColor)
        {
            // set lightmap as render target
            SDL.SDL_SetRenderTarget(_rend, lightMapTex);
            // fill the lightmap with shadowColor
            SDL.SDL_SetRenderDrawColor(_rend, shadowColor.R, shadowColor.G, shadowColor.B, shadowColor.A);
            // draw it to the texture
            SDL.SDL_RenderClear(_rend);

            foreach (var light in Bootstrap.GetLightObjects())
            {
                _renderFilledCircle(light.x, light.y, light.radius, light.color);
            }

            SDL.SDL_SetRenderTarget(_rend, IntPtr.Zero);
        }
        public void _renderFilledCircle(int x, int y, int rad, Color col)
        {
            SDL.SDL_SetRenderDrawColor(_rend, (byte)col.R, (byte)col.G, (byte)col.B, (byte)col.A);
            for (int width = 0; width < rad * 2; width++) 
            {
                for (int height = 0; height < rad * 2; height++) 
                {
                    var horizontalOffset = rad - width;
                    var verticalOffset = rad - height;
                    if ((horizontalOffset * horizontalOffset + verticalOffset * verticalOffset) <= (rad * rad))
                    {
                        SDL.SDL_RenderDrawPoint(_rend, x + horizontalOffset, y + verticalOffset);
                    }
                }
            } 
        }

        public override void clearDisplay()
        {

            _toDraw.Clear();
            _circlesToDraw.Clear();
            _linesToDraw.Clear();
            _polygonsToDraw.Clear();
            Bootstrap.ClearLightObjects();
            base.clearDisplay();
        }

    }


}