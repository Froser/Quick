using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Froser.Quick.UI
{
    public class QuickListItem
    {
        private const int IMAGE_SIZE = 32;
        private const int IMAGE_SIZE_SIMPLE = 24;

        public QuickListItem(string title, string description, ImageSource imgSrc)
        {
            m_title = title;
            m_description = description;
            m_iconImageSource = imgSrc;
            m_type = description == null ? ListType.Simple : ListType.Full;
        }

        public ListBoxItem CreateListBoxItemTo(QuickListBox list)
        {
            ListBoxItem item = new ListBoxItem();
            item.Focusable = false;
            item.SetResourceReference(ListBoxItem.StyleProperty, "quickListItemStyle");

            InitGrid();
            InitImage();
            InitTextGrid();
            InitTitle();
            InitDescription();

            item.Content = m_grid;
            list.Items.Add(item);

            item.MouseEnter += (sender, e) => { list.SelectedItem = item; };
            item.PreviewMouseDown += (sender, e) => { list.ActivateListItemClickedEvent(item); };
            item.Tag = this;

            return item;
        }

        private void InitGrid()
        {
            GridLength length ;
            if (m_type == ListType.Full)
                length = new GridLength(IMAGE_SIZE, GridUnitType.Pixel);
            else
                length = new GridLength(IMAGE_SIZE_SIMPLE, GridUnitType.Pixel);
            ColumnDefinition gcd1 = new ColumnDefinition();
            ColumnDefinition gcd2 = new ColumnDefinition();
            gcd1.Width = length;
            m_grid = new Grid();
            m_grid.ColumnDefinitions.Add(gcd1);
            m_grid.ColumnDefinitions.Add(gcd2);
        }

        private void InitImage()
        {
            m_img = new Image();
            if (m_iconImageSource != null)
                m_img.Source = m_iconImageSource;
            else
                m_img.Source = QuickUIResource.GetDefaultIcon();
            Grid.SetColumn(m_img, 0);
            m_grid.Children.Add(m_img);
        }

        private void InitTextGrid()
        {
            m_textGrid = new Grid();
            if (m_type == ListType.Full)
            {
                RowDefinition tgcd1 = new RowDefinition();
                RowDefinition tgcd2 = new RowDefinition();
                tgcd1.Height = new GridLength(4, GridUnitType.Star);
                tgcd2.Height = new GridLength(2, GridUnitType.Star);
                m_textGrid.RowDefinitions.Add(tgcd1);
                m_textGrid.RowDefinitions.Add(tgcd2);
            }
            m_grid.Children.Add(m_textGrid);
            Grid.SetColumn(m_textGrid, 1);
        }

        private void InitTitle()
        {
            m_lbTitle = new Label();
            m_lbTitle.Content = m_title.Replace("_", "__");
            m_lbTitle.FontFamily = DEFAULT_FONT_FAMILTY;
            m_lbTitle.FontWeight = FontWeights.Bold;
            m_lbTitle.FontSize = 16;
            m_textGrid.Children.Add(m_lbTitle);
            Grid.SetRow(m_lbTitle, 0);
        }

        private void InitDescription()
        {
            if (m_type == ListType.Full)
            {
                m_lbDescription = new Label();
                m_lbDescription.Content = m_description.Replace("_", "__");
                m_lbDescription.FontFamily = DEFAULT_FONT_FAMILTY;
                m_lbDescription.FontSize = 12;
                m_textGrid.Children.Add(m_lbDescription);
                Grid.SetRow(m_lbDescription, 1);
            }
        }

        public object Tag { get; set; }

        private readonly FontFamily DEFAULT_FONT_FAMILTY = new FontFamily("Consolas, Microsoft YaHei");
        private Grid m_grid;
        private Image m_img;
        private Grid m_textGrid;
        private Label m_lbTitle;
        private Label m_lbDescription;

        private ImageSource m_iconImageSource;
        private string m_title;
        private string m_description;
        private enum ListType {Simple, Full}
        private ListType m_type;
    }
}
