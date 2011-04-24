using System.Windows.Forms;
using Microsoft.Xna.Framework;
namespace Editor {
    interface IEditorState {
        void MouseDown(Point e, Input.MouseButton b);
        void MouseUp(Point e, Input.MouseButton b);
        void MouseClick(Point e, Input.MouseButton b);
        void MouseWheel(Point p, int delta);
        void KeyPress(KeyEventArgs e);

        void RenderHUD();
        //void OnWheel(MouseEventArgs e);		
    }
}
