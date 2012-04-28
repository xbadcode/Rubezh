﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class ToolboxView : UserControl
    {
        public ToolboxView()
        {
            InitializeComponent();
            Current = this;
            AcceptKeyboard = true;
        }

        static ToolboxView()
        {
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDownStatic), true);
        }

        public static ToolboxView Current { get; private set; }
        public bool AcceptKeyboard { get; set; }

        private Point? dragStartPoint = null;

        private void On_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.dragStartPoint = new Point?(e.GetPosition(this));
            e.Handled = true;
        }

        private void On_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                var elementBase = CreateNewElementBase((sender as Controls.ToolBarButton).Name);
                var dataObject = new DataObject("DESIGNER_ITEM", elementBase);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                e.Handled = true;
            }

            e.Handled = true;
        }

        ElementBase CreateNewElementBase(string itemType)
        {
            ElementBase elementBase = null;

            switch (itemType)
            {
                case "ElementPolyLine":
                    elementBase = new ElementPolyline();
                    break;

                case "ElementRectangle":
                    elementBase = new ElementRectangle();
                    break;

                case "ElementEllipse":
                    elementBase = new ElementEllipse();
                    break;

                case "ElementPolygon":
                    elementBase = new ElementPolygon();
                    break;

                case "ElementTextBlock":
                    elementBase = new ElementTextBlock();
                    break;

                case "ElementRectangleZone":
                    elementBase = new ElementRectangleZone();
                    break;

                case "ElementPolygonZone":
                    elementBase = new ElementPolygonZone();
                    break;

                case "ElementSubPlan":
                    elementBase = new ElementSubPlan();
                    break;
            }

            return elementBase;
        }

        static void OnKeyDownStatic(object sender, KeyEventArgs e)
        {
            Current.OnKeyDown(sender, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (AcceptKeyboard == false)
                return;

            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(copyButton);
            }
            if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(cutButton);
            }
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(pasteButton);
            }
            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(undoButton);
            }
            if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                PressButton(redoButton);
            }
            if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var designerCanvas = (DataContext as PlansViewModel).DesignerCanvas;
                if (designerCanvas != null)
                    designerCanvas.SelectAll();
            }
            if (e.Key == Key.Delete)
            {
                var designerCanvas = (DataContext as PlansViewModel).DesignerCanvas;
                if (designerCanvas != null)
                    designerCanvas.RemoveAllSelected();
            }
        }

        void PressButton(Button button)
        {
            if (button.Command.CanExecute(null))
                button.Command.Execute(null);
        }
    }
}