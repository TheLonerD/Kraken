using System.Collections.Generic;

namespace PKHack
{
    public static class UI
    {
        public delegate void EditorDelegate(Rom rom, RomObject obj);

        public struct Editor
        {
            public string Name;
            public string TypeName;
            public EditorDelegate Show;

            public override string ToString()
            {
                return Name;
            }
        }

        private static List<Editor> editors = new List<Editor>();

        public static List<Editor> Editors
        {
            get
            {
                return editors;
            }
        }

        public static void RegisterEditor(string id, string type, EditorDelegate del)
        {
            Editor e = new Editor();
            e.Name = id;
            e.TypeName = type;
            e.Show = del;
            editors.Add(e);
        }
    }
}
