using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ZdyLovePlayer.Behavior
{
    public class ReachingScrollViewerBottomBehavior : Behavior<ScrollViewer>
    {
        public ReachingScrollViewerBottomBehavior()
        {
            // Insert code required on object creation below this point.
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            // Insert code that you would want run when the Behavior is attached to an object.
            var dpd = DependencyPropertyDescriptor.FromProperty(ScrollViewer.VerticalOffsetProperty, AssociatedType);
            dpd.AddValueChanged(AssociatedObject, (sender, args) =>
            {
                RaiseReachingBottomEvent();
            });
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        private void RaiseReachingBottomEvent()
        {
            bool isReachingBottom = AssociatedObject.VerticalOffset >= AssociatedObject.ScrollableHeight;

            if (isReachingBottom)
            {
                if (this.ReachingBottomEvent != null)
                {
                    this.ReachingBottomEvent();
                }
            }
        }

        public event Action ReachingBottomEvent;
    }
}
