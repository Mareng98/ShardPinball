/*
*
*   The baseline functionality for getting text to work via SDL.   You could write your own text 
*       implementation (and we did that earlier in the course), but bear in mind DisplaySDL is built
*       upon this class.
*   @author Michael Heron
*   @version 1.0
*   
*/

using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Shard
{

    // We'll be using SDL2 here to provide our underlying graphics system.
    class TextDetails
    {
        string text;
        double x, y;
        SDL.SDL_Color col;
        int size;
        IntPtr font;
        IntPtr lblText;


        public TextDetails(string text, double x, double y, SDL.SDL_Color col, int spacing)
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.col = col;
            this.size = spacing;
        }

        public string Text
        {
            get => text;
            set => text = value;
        }
        public double X
        {
            get => x;
            set => x = value;
        }
        public double Y
        {
            get => y;
            set => y = value;
        }
        public SDL.SDL_Color Col
        {
            get => col;
            set => col = value;
        }
        public int Size
        {
            get => size;
            set => size = value;
        }
        public IntPtr Font { get => font; set => font = value; }
        public IntPtr LblText { get => lblText; set => lblText = value; }
    }

    class DisplayText : Display
    {
        protected IntPtr _window, _rend, screenTextureBuf, lightMapTex;
        uint _format;
        int _access;
        private List<TextDetails> myTexts;
        private Dictionary<string, IntPtr> fontLibrary;
        public override void clearDisplay()
        {
            foreach (TextDetails td in myTexts)
            {
                SDL.SDL_DestroyTexture(td.LblText);
            }

            myTexts.Clear();
            SDL.SDL_SetRenderDrawColor(_rend, 0, 0, 0, 255);
            SDL.SDL_RenderClear(_rend);
            // reset render target to screenTextBuf 
            SDL.SDL_SetRenderTarget(_rend, screenTextureBuf);

        }

        public IntPtr loadFont(string path, int size)
        {
            string key = path + "," + size;

            if (fontLibrary.ContainsKey(key))
            {
                return fontLibrary[key];
            }

            fontLibrary[key] = SDL_ttf.TTF_OpenFont(path, size);
            return fontLibrary[key];
        }

        private void update()
        {


        }

        private void draw()
        {

            foreach (TextDetails td in myTexts)
            {

                SDL.SDL_Rect sRect;

                sRect.x = (int)td.X;
                sRect.y = (int)td.Y;
                sRect.w = 0;
                sRect.h = 0;


                SDL_ttf.TTF_SizeText(td.Font, td.Text, out sRect.w, out sRect.h);
                SDL.SDL_RenderCopy(_rend, td.LblText, IntPtr.Zero, ref sRect);

            }

            // first render pass
            // copy firstRenderPass texture to renderer
            SDL.SDL_SetRenderTarget(_rend, IntPtr.Zero);
            SDL.SDL_RenderCopy(_rend, screenTextureBuf, IntPtr.Zero, IntPtr.Zero);
            if (Bootstrap.IsLightingOn())
            {

                //drawLightMap(Color.FromArgb(255, 0, 0, 0));
                Bootstrap.DrawLightMap();
                SDL.SDL_RenderCopy(_rend, lightMapTex, IntPtr.Zero, IntPtr.Zero);
            }
            SDL.SDL_RenderPresent(_rend);
        }

        public override void drawLightMap(Color shadowMap)
        {
        }


        public override void display()
        {

            update();
            draw();
        }

        public override void setFullscreen()
        {
            SDL.SDL_SetWindowFullscreen(_window,
                 (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
        }

        public override void initialize()
        {
            fontLibrary = new Dictionary<string, IntPtr>();

            setSize(1280, 1080);

            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            SDL_ttf.TTF_Init();
            _window = SDL.SDL_CreateWindow("Shard Game Engine",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                getWidth(),
                getHeight(),
                0);


            _rend = SDL.SDL_CreateRenderer(_window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

//            SDL.SDL_SetRenderDrawBlendMode(_rend, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            SDL.SDL_SetRenderDrawColor(_rend, 0, 0, 0, 255);

            // render to a texture
            lightMapTex = SDL.SDL_CreateTexture(_rend, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, getWidth(), getHeight());
            screenTextureBuf = SDL.SDL_CreateTexture(_rend, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, _width, _height);

            //SDL.SDL_SetTextureBlendMode(lightMapTex, SDL.SDL_BlendMode.SDL_BLENDMODE_MUL);

            SDL.SDL_SetTextureBlendMode(screenTextureBuf, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDL.SDL_SetTextureBlendMode(lightMapTex, SDL.SDL_BlendMode.SDL_BLENDMODE_MUL);
            SDL.SDL_SetRenderTarget(_rend, screenTextureBuf);

            myTexts = new List<TextDetails>();
        }



        public override void showText(string text, double x, double y, int size, int r, int g, int b)
        {
            int nx, ny, w = 0, h = 0;

            string ffolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Fonts);

            IntPtr font = loadFont(ffolder + "\\calibri.ttf", size);
            SDL.SDL_Color col = new SDL.SDL_Color();

            col.r = (byte)r;
            col.g = (byte)g;
            col.b = (byte)b;
            col.a = (byte)255;

            if (font == IntPtr.Zero)
            {
                Debug.getInstance().log("TTF_OpenFont: " + SDL.SDL_GetError());
            }

            TextDetails td = new TextDetails(text, x, y, col, 12);

            td.Font = font;

            IntPtr surf = SDL_ttf.TTF_RenderText_Blended(td.Font, td.Text, td.Col);
            IntPtr lblText = SDL.SDL_CreateTextureFromSurface(_rend, surf);
            SDL.SDL_FreeSurface(surf);

            SDL.SDL_Rect sRect;

            sRect.x = (int)x;
            sRect.y = (int)y;
            sRect.w = w;
            sRect.h = h;

            SDL.SDL_QueryTexture(lblText, out _format, out _access, out sRect.w, out sRect.h);

            td.LblText = lblText;

            myTexts.Add(td);


        }
        public override void showText(char[,] text, double x, double y, int size, int r, int g, int b)
        {
            string str = "";
            int row = 0;

            for (int i = 0; i < text.GetLength(0); i++)
            {
                str = "";
                for (int j = 0; j < text.GetLength(1); j++)
                {
                    str += text[j, i];
                }


                showText(str, x, y + (row * size), size, r, g, b);
                row += 1;

            }

        }
        public override void renderCircle(int centreX, int centreY, int rad)
        {
            int dia = (rad * 2);
            byte r, g, b, a;
            int x = (rad - 1);
            int y = 0;
            int tx = 1;
            int ty = 1;
            int error = (tx - dia);

            SDL.SDL_GetRenderDrawColor(_rend, out r, out g, out b, out a);

            // We draw an octagon around the point, and then turn it a bit.  Do 
            // that until we have an outline circle.  If you want a filled one, 
            // do the same thing with an ever decreasing radius.
            while (x >= y)
            {

                SDL.SDL_RenderDrawPoint(_rend, centreX + x, centreY - y);
                SDL.SDL_RenderDrawPoint(_rend, centreX + x, centreY + y);
                SDL.SDL_RenderDrawPoint(_rend, centreX - x, centreY - y);
                SDL.SDL_RenderDrawPoint(_rend, centreX - x, centreY + y);
                SDL.SDL_RenderDrawPoint(_rend, centreX + y, centreY - x);
                SDL.SDL_RenderDrawPoint(_rend, centreX + y, centreY + x);
                SDL.SDL_RenderDrawPoint(_rend, centreX - y, centreY - x);
                SDL.SDL_RenderDrawPoint(_rend, centreX - y, centreY + x);

                if (error <= 0)
                {
                    y += 1;
                    error += ty;
                    ty += 2;
                }

                if (error > 0)
                {
                    x -= 1;
                    tx += 2;
                    error += (tx - dia);
                }
            }
        }
    }
}
