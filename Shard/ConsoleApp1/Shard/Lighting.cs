using System.Collections.Generic;
using System.Drawing;

namespace Shard
{
    class Lighting
    {
        private List<LightInfo> lightObjects = new();
        private bool lighting = false;
        private Color shadowColor = Color.FromArgb(200, 0, 0, 0);

        public Color ShadowColor { get => shadowColor; set => shadowColor = value; }

        public Lighting(){}

        public void EnableLight()
        {
            lighting = true;
        }

        public void DisableLight()
        {
            lighting = false;
        }

        public bool IsLightingOn()
        {
            return lighting;
        }

        public List<LightInfo> GetLightObjects()
        {
            return lightObjects;
        }

        public void ClearLightObjects()
        {
            lightObjects.Clear();
        }

        public void AddLightObject(int x, int y, int radius, Color col)
        {
            lightObjects.Add(new LightInfo(x, y, radius, col));
        }

        public void DrawLightMap()
        {
            Bootstrap.getDisplay().drawLightMap(shadowColor);
        }
    }
    struct LightInfo
    {
        public int x;
        public int y;
        public int radius;
        public Color color;
        public LightInfo(int x, int y, int radius, Color color)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.color  = color;
        }
    }
}
