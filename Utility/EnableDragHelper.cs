using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultiStopwatch.Utility;

public class EnableDragHelper
{
    public static readonly DependencyProperty EnableDragProperty = DependencyProperty.RegisterAttached(
        "EnableDrag", typeof(bool), typeof(EnableDragHelper), new PropertyMetadata(default(bool), OnLoaded));

    private static void OnLoaded(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (dependencyObject is not UIElement uiElement || dependencyPropertyChangedEventArgs.NewValue is bool == false)
            return;

        if ((bool)dependencyPropertyChangedEventArgs.NewValue == true)
            uiElement.MouseMove += UIElementOnMouseMove;
        else
            uiElement.MouseMove -= UIElementOnMouseMove;
    }

    private static void UIElementOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
    {
        if (sender is not UIElement uiElement || mouseEventArgs.LeftButton != MouseButtonState.Pressed)
            return;

        DependencyObject parent = uiElement;
        var avoidInfiniteLoop = 0;

        // Search up the visual tree to find the first parent window.
        while (parent is Window == false)
        {
            parent = VisualTreeHelper.GetParent(parent);
            avoidInfiniteLoop++;
            if (avoidInfiniteLoop == 1000)
            {
                // Something is wrong - we could not find the parent window.
                return;
            }
        }

        var window = parent as Window;
        try
        {
            window.DragMove();
        }
        catch (InvalidOperationException)
        {
            // ignored
        }
    }

    public static void SetEnableDrag(DependencyObject element, bool value)
    {
        element.SetValue(EnableDragProperty, value);
    }

    public static bool GetEnableDrag(DependencyObject element)
    {
        return (bool)element.GetValue(EnableDragProperty);
    }
}