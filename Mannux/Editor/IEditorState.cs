using System.Windows.Forms;

namespace Editor {
    interface IEditorState {
        void MouseDown(MouseEventArgs e);
        void MouseUp(MouseEventArgs e);
        void MouseClick(MouseEventArgs e);
        void KeyPress(KeyEventArgs e);

        void RenderHUD();
        //void OnWheel(MouseEventArgs e);		
    }
}
