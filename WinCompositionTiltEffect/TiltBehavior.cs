using Microsoft.Xaml.Interactivity;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace WinCompositionTiltEffect
{
    public class TiltBehavior : Behavior
    {
        /// <summary>
        /// Windows Composition Visual element.
        /// </summary>
        private Visual elementVisual;

        /// <summary>
        /// Handle to control the behavior is attached to.
        /// </summary>
        private UIElement uiElement;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.uiElement = this.AssociatedObject as UIElement;
            if (this.uiElement == null)
            {
                throw new InvalidOperationException("TiltBehavior can only be attached to types inheriting UIElement");
            }

            this.elementVisual = ElementCompositionPreview.GetElementVisual(this.uiElement);

            this.uiElement.PointerPressed += UiElement_PointerPressed;
            this.uiElement.PointerMoved += UiElement_PointerMoved;
            this.uiElement.PointerReleased += UiElement_PointerReleased;
            this.uiElement.PointerCanceled += UiElement_PointerCanceled;
            this.uiElement.PointerExited += UiElement_PointerExited;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.elementVisual != null)
            {
                StopTiltEffect();

                this.elementVisual.Dispose();
                this.elementVisual = null;
            }

            this.uiElement.PointerPressed -= UiElement_PointerPressed;
            this.uiElement.PointerMoved -= UiElement_PointerMoved;
            this.uiElement.PointerReleased -= UiElement_PointerReleased;
            this.uiElement.PointerCanceled -= UiElement_PointerCanceled;
            this.uiElement.PointerExited -= UiElement_PointerExited;

            this.uiElement = null;
        }

        private void UiElement_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StartTiltEffect(e);
        }

        private void UiElement_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact)
            {
                StartTiltEffect(e);
            }
            else
            {
                StopTiltEffect();
            }
        }

        private void UiElement_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StopTiltEffect();
        }

        private void UiElement_PointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StopTiltEffect();
        }

        private void UiElement_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StopTiltEffect();
        }

        private void StartTiltEffect(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.elementVisual.CenterPoint = new Vector3((float)(this.uiElement.RenderSize.Width * 0.5f), (float)(this.uiElement.RenderSize.Height * 0.5f), -10.0f);

            var contactPoint = e.GetCurrentPoint(uiElement).Position;
            // var contactVector = Vector3.Normalize(this.elementVisual.CenterPoint - new Vector3((float)contactPoint.X, (float)contactPoint.Y, 0.0f));
            var contactVector = new Vector3((float)contactPoint.X, (float)contactPoint.Y, 0.0f) - this.elementVisual.CenterPoint;
            var magnitude = contactVector.Length();
            contactVector = Vector3.Normalize(contactVector);
                        
            this.elementVisual.RotationAxis = new Vector3(contactVector.Y, -contactVector.X, 0.0f);
            this.elementVisual.RotationAngleInDegrees = 20.0f;
        }

        private void StopTiltEffect()
        {
            this.elementVisual.RotationAngleInDegrees = 0.0f;
        }
    }
}
