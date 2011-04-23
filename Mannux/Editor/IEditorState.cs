using System.Windows.Forms;
using Microsoft.Xna.Framework;
namespace Editor {
    interface IEditorState {
        void MouseDown(Point e);
        void MouseUp(Point e);
        void MouseClick(Point e);
        void MouseWheel(Point p, int delta);
        void KeyPress(KeyEventArgs e);

        void RenderHUD();
        //void OnWheel(MouseEventArgs e);		
    }
}
