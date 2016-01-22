using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Froser.Quick.UI
{
    public class QuickListBox : ListBox
    {
        public event EventHandler ListItemClicked;

        public void ActivateListItemClickedEvent(ListBoxItem sender)
        {
            if (ListItemClicked != null)
                ListItemClicked(sender, new EventArgs());
        }
    }
}
