using System.Drawing;

namespace Guard {
    internal class ListBoxItem {
        public string Text { get; set; }

        //public Image Thumbnail { get; set; }

        public string FullPath { get; set; }

        //public string DisplayPath { get; set; }

        public bool IsDuplicate { get; set; }

        public override string ToString() {
            return Text;
        }
    }
}