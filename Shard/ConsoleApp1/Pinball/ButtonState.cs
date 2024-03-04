using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Pinball
{
    class ButtonState
    {
        String tag;
        bool isHovered;
        string buttonAsset;
        string buttonHoveredAsset;
        string buttonClickedAsset;
        
        public ButtonState(string tag, string buttonAsset, string buttonHoveredAsset, string buttonClickedAsset)
        {
            this.tag = tag;
            this.isHovered = false;
            this.buttonAsset = buttonAsset == null ? null : Bootstrap.getAssetManager().getAssetPath(buttonAsset);
            this.buttonHoveredAsset = buttonHoveredAsset == null ? null : Bootstrap.getAssetManager().getAssetPath(buttonHoveredAsset);
            this.buttonClickedAsset = buttonClickedAsset == null ? null : Bootstrap.getAssetManager().getAssetPath(buttonClickedAsset);
        }

        public bool IsHovered { get => isHovered; set => isHovered = value; }
        public string ButtonAsset { get => buttonAsset; }
        public string ButtonHoveredAsset { get => buttonHoveredAsset; }
        public string ButtonClickedAsset { get => buttonClickedAsset; }
        public string Tag { get => tag; set => tag = value; }

        public string getButtonAsset()
        {
            if (isHovered)
            {
                return buttonHoveredAsset;
            }
            else
            {
                return buttonAsset;  
            }

        }
    }
}
