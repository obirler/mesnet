/*
========================================================================
    Copyright (C) 2016 Omer Birler.
    
    This file is part of Mesnet.

    Mesnet is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Mesnet is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Mesnet.  If not, see <http://www.gnu.org/licenses/>.
========================================================================
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Mesnet.Classes.Ui
{
	/// <summary>
	/// A Canvas which manages dragging of the UIElements it contains.  
	/// </summary>
	public class DragCanvas : Canvas
	{
		#region Data

		// Stores a reference to the UIElement currently being dragged by the user.
		private UIElement Element_being_dragged;

		// Keeps track of where the mouse cursor was when a drag operation began.		
		private Point Orig_cursor_location;
		
		// The offsets from the DragCanvas' edges when the drag operation began.
		private double Orig_horiz_offset, Orig_vert_offset;

		// Keeps track of which horizontal and vertical offset should be modified for the drag element.
		private bool Modify_left_offset, Modify_top_offset;

		// True if a drag operation is underway, else false.
		private bool Is_drag_in_progress;

		#endregion // Data

		#region Attached Properties

			#region CanBeDragged

		public static readonly DependencyProperty CanBeDraggedProperty;

		public static bool GetCanBeDragged( UIElement uiElement )
		{
			if( uiElement == null )
				return false;

			return (bool)uiElement.GetValue( CanBeDraggedProperty );
		}

		public static void SetCanBeDragged( UIElement uiElement, bool value )
		{
			if( uiElement != null )
				uiElement.SetValue( CanBeDraggedProperty, value );
		}

			#endregion // CanBeDragged

		#endregion // Attached Properties

		#region Dependency Properties

		public static readonly DependencyProperty AllowDraggingProperty;
		public static readonly DependencyProperty AllowDragOutOfViewProperty;

		#endregion // Dependency Properties

		#region Static Constructor

		static DragCanvas()
		{
			AllowDraggingProperty = DependencyProperty.Register(
				"AllowDragging",
				typeof( bool ),
				typeof( DragCanvas ),
				new PropertyMetadata( true ) );

			AllowDragOutOfViewProperty = DependencyProperty.Register(
				"AllowDragOutOfView",
				typeof( bool ),
				typeof( DragCanvas ),
				new UIPropertyMetadata( false ) );

			CanBeDraggedProperty = DependencyProperty.RegisterAttached(
				"CanBeDragged",
				typeof( bool ),
				typeof( DragCanvas ),
				new UIPropertyMetadata( true ) );
		}

		#endregion // Static Constructor

		#region Constructor

		/// <summary>
		/// Initializes a new instance of DragCanvas.  UIElements in
		/// the DragCanvas will immediately be draggable by the user.
		/// </summary>
		public DragCanvas()
		{
		}

		#endregion // Constructor

		#region Interface

			#region AllowDragging

		/// <summary>
		/// Gets/sets whether elements in the DragCanvas should be draggable by the user.
		/// The default value is true.  This is a dependency property.
		/// </summary>
		public bool AllowDragging
		{
			get { return (bool)base.GetValue( AllowDraggingProperty ); }
			set { base.SetValue( AllowDraggingProperty, value ); }
		}

			#endregion // AllowDragging

			#region AllowDragOutOfView

		/// <summary>
		/// Gets/sets whether the user should be able to drag elements in the DragCanvas out of
		/// the viewable area.  The default value is false.  This is a dependency property.
		/// </summary>
		public bool AllowDragOutOfView
		{
			get { return (bool)GetValue( AllowDragOutOfViewProperty ); }
			set { SetValue( AllowDragOutOfViewProperty, value ); }
		}

			#endregion // AllowDragOutOfView			

			#region BringToFront / SendToBack

		/// <summary>
		/// Assigns the element a z-index which will ensure that 
		/// it is in front of every other element in the Canvas.
		/// The z-index of every element whose z-index is between 
		/// the element's old and new z-index will have its z-index 
		/// decremented by one.
		/// </summary>
		/// <param name="targetElement">
		/// The element to be sent to the front of the z-order.
		/// </param>
		public void BringToFront( UIElement element )
		{
			this.UpdateZOrder( element, true );
		}

		/// <summary>
		/// Assigns the element a z-index which will ensure that 
		/// it is behind every other element in the Canvas.
		/// The z-index of every element whose z-index is between 
		/// the element's old and new z-index will have its z-index 
		/// incremented by one.
		/// </summary>
		/// <param name="targetElement">
		/// The element to be sent to the back of the z-order.
		/// </param>
		public void SendToBack( UIElement element )
		{
			this.UpdateZOrder( element, false );
		}

			#endregion // BringToFront / SendToBack

			#region ElementBeingDragged

		/// <summary>
		/// Returns the UIElement currently being dragged, or null.
		/// </summary>
		/// <remarks>
		/// Note to inheritors: This property exposes a protected 
		/// setter which should be used to modify the drag element.
		/// </remarks>
		public UIElement ElementBeingDragged
		{
			get
			{
				if( !this.AllowDragging )
					return null;
				else
					return this.Element_being_dragged;
			}
			protected set
			{
				if( this.Element_being_dragged != null )
					this.Element_being_dragged.ReleaseMouseCapture();

				if( !this.AllowDragging )
					this.Element_being_dragged = null;
				else
				{
					if( GetCanBeDragged( value ) )
					{
						this.Element_being_dragged = value;
						this.Element_being_dragged.CaptureMouse();
					}
					else
						this.Element_being_dragged = null;
				}
			}
}

			#endregion // ElementBeingDragged

			#region FindCanvasChild

		/// <summary>
		/// Walks up the visual tree starting with the specified DependencyObject, 
		/// looking for a UIElement which is a child of the Canvas.  If a suitable 
		/// element is not found, null is returned.  If the 'depObj' object is a 
		/// UIElement in the Canvas's Children collection, it will be returned.
		/// </summary>
		/// <param name="depObj">
		/// A DependencyObject from which the search begins.
		/// </param>
		public UIElement FindCanvasChild( DependencyObject depObj )
		{
			while( depObj != null )
			{
				// If the current object is a UIElement which is a child of the
				// Canvas, exit the loop and return it.
				UIElement elem = depObj as UIElement;
				if( elem != null && base.Children.Contains( elem ) )
					break;

				// VisualTreeHelper works with Objects of type Visual or Visual3D.
				// If the current object is not derived from Visual or Visual3D,
				// then use the LogicalTreeHelper to find the parent element.
				if( depObj is Visual || depObj is Visual3D )
					depObj = VisualTreeHelper.GetParent( depObj );
				else
					depObj = LogicalTreeHelper.GetParent( depObj );
			}
			return depObj as UIElement;
		}

			#endregion // FindCanvasChild

		#endregion // Interface

		#region Overrides

		#region OnPreviewMouseLeftButtonDown

		protected override void OnPreviewMouseLeftButtonDown( MouseButtonEventArgs e )
		{
			base.OnPreviewMouseLeftButtonDown( e );

			this.Is_drag_in_progress = false;

			// Cache the mouse cursor location.
			this.Orig_cursor_location = e.GetPosition( this );

			// Walk up the visual tree from the element that was clicked, 
			// looking for an element that is a direct child of the Canvas.
			this.ElementBeingDragged = this.FindCanvasChild( e.Source as DependencyObject );
			if( this.ElementBeingDragged == null )
				return;

			// Get the element's offsets from the four sides of the Canvas.
			double left = GetLeft( this.ElementBeingDragged );
			double right = GetRight( this.ElementBeingDragged );
			double top = GetTop( this.ElementBeingDragged );
			double bottom = GetBottom( this.ElementBeingDragged );

			// Calculate the offset deltas and determine for which sides
			// of the Canvas to adjust the offsets.
			this.Orig_horiz_offset = ResolveOffset( left, right, out this.Modify_left_offset );
			this.Orig_vert_offset = ResolveOffset( top, bottom, out this.Modify_top_offset );

			// Set the Handled flag so that a control being dragged 
			// does not react to the mouse input.
			e.Handled = true;

			this.Is_drag_in_progress = true;
		}

			#endregion // OnPreviewMouseLeftButtonDown

			#region OnPreviewMouseMove

		protected override void OnPreviewMouseMove( MouseEventArgs e )
		{
			base.OnPreviewMouseMove( e );

			// If no element is being dragged, there is nothing to do.
			if( this.ElementBeingDragged == null || !this.Is_drag_in_progress )
				return;

			// Get the position of the mouse cursor, relative to the Canvas.
			Point cursorLocation = e.GetPosition( this );

			// These values will store the new offsets of the drag element.
			double newHorizontalOffset, newVerticalOffset;

			#region Calculate Offsets

			// Determine the horizontal offset.
			if( this.Modify_left_offset )
				newHorizontalOffset = this.Orig_horiz_offset + (cursorLocation.X - this.Orig_cursor_location.X);
			else
				newHorizontalOffset = this.Orig_horiz_offset - (cursorLocation.X - this.Orig_cursor_location.X);

			// Determine the vertical offset.
			if( this.Modify_top_offset )
				newVerticalOffset = this.Orig_vert_offset + (cursorLocation.Y - this.Orig_cursor_location.Y);
			else
				newVerticalOffset = this.Orig_vert_offset - (cursorLocation.Y - this.Orig_cursor_location.Y);

			#endregion // Calculate Offsets

			if( ! this.AllowDragOutOfView )
			{
				#region Verify Drag Element Location

				// Get the bounding rect of the drag element.
				Rect elemRect = this.CalculateDragElementRect( newHorizontalOffset, newVerticalOffset );

				//
				// If the element is being dragged out of the viewable area, 
				// determine the ideal rect location, so that the element is 
				// within the edge(s) of the canvas.
				//
				bool leftAlign = elemRect.Left < 0;
				bool rightAlign = elemRect.Right > this.ActualWidth;

				if( leftAlign )
					newHorizontalOffset = Modify_left_offset ? 0 : this.ActualWidth - elemRect.Width;
				else if( rightAlign )
					newHorizontalOffset = Modify_left_offset ? this.ActualWidth - elemRect.Width : 0;

				bool topAlign = elemRect.Top < 0;
				bool bottomAlign = elemRect.Bottom > this.ActualHeight;

				if( topAlign )
					newVerticalOffset = Modify_top_offset ? 0 : this.ActualHeight - elemRect.Height;
				else if( bottomAlign )
					newVerticalOffset = Modify_top_offset ? this.ActualHeight - elemRect.Height : 0;

				#endregion // Verify Drag Element Location
			}

			#region Move Drag Element

			if( this.Modify_left_offset )
				SetLeft( this.ElementBeingDragged, newHorizontalOffset );
			else
				SetRight( this.ElementBeingDragged, newHorizontalOffset );

			if( this.Modify_top_offset )
				SetTop( this.ElementBeingDragged, newVerticalOffset );
			else
				SetBottom( this.ElementBeingDragged, newVerticalOffset );

			#endregion // Move Drag Element
		}

			#endregion // OnPreviewMouseMove

			#region OnHostPreviewMouseUp

		protected override void OnPreviewMouseUp( MouseButtonEventArgs e )
		{
			base.OnPreviewMouseUp( e );

			// Reset the field whether the left or right mouse button was 
			// released, in case a context menu was opened on the drag element.
			this.ElementBeingDragged = null;
		}

			#endregion // OnHostPreviewMouseUp

		#endregion // Host Event Handlers

		#region Private Helpers

			#region CalculateDragElementRect

		/// <summary>
		/// Returns a Rect which describes the bounds of the element being dragged.
		/// </summary>
		private Rect CalculateDragElementRect( double newHorizOffset, double newVertOffset )
		{
			if( this.ElementBeingDragged == null )
				throw new InvalidOperationException( "ElementBeingDragged is null." );

			Size elemSize = this.ElementBeingDragged.RenderSize;

			double x, y;

			if( this.Modify_left_offset )
				x = newHorizOffset;
			else
				x = this.ActualWidth - newHorizOffset - elemSize.Width;

			if( this.Modify_top_offset )
				y = newVertOffset;
			else
				y = this.ActualHeight - newVertOffset - elemSize.Height;

			Point elemLoc = new Point( x, y );

			return new Rect( elemLoc, elemSize );
		}

			#endregion // CalculateDragElementRect

			#region ResolveOffset

		/// <summary>
		/// Determines one component of a UIElement's location 
		/// within a Canvas (either the horizontal or vertical offset).
		/// </summary>
		/// <param name="side1">
		/// The value of an offset relative to a default side of the 
		/// Canvas (i.e. top or left).
		/// </param>
		/// <param name="side2">
		/// The value of the offset relative to the other side of the 
		/// Canvas (i.e. bottom or right).
		/// </param>
		/// <param name="useSide1">
		/// Will be set to true if the returned value should be used 
		/// for the offset from the side represented by the 'side1' 
		/// parameter.  Otherwise, it will be set to false.
		/// </param>
		private static double ResolveOffset( double side1, double side2, out bool useSide1 )
		{
			// If the Canvas.Left and Canvas.Right attached properties 
			// are specified for an element, the 'Left' value is honored.
			// The 'Top' value is honored if both Canvas.Top and 
			// Canvas.Bottom are set on the same element.  If one 
			// of those attached properties is not set on an element, 
			// the default value is Double.NaN.
			useSide1 = true;
			double result;
			if( Double.IsNaN( side1 ) )
			{
				if( Double.IsNaN( side2 ) )
				{
					// Both sides have no value, so set the
					// first side to a value of zero.
					result = 0;
				}
				else
				{
					result = side2;
					useSide1 = false;
				}
			}
			else
			{
				result = side1;
			}
			return result;
		}

			#endregion // ResolveOffset

			#region UpdateZOrder

		/// <summary>
		/// Helper method used by the BringToFront and SendToBack methods.
		/// </summary>
		/// <param name="element">
		/// The element to bring to the front or send to the back.
		/// </param>
		/// <param name="bringToFront">
		/// Pass true if calling from BringToFront, else false.
		/// </param>
		private void UpdateZOrder( UIElement element, bool bringToFront )
		{
			#region Safety Check

			if( element == null )
				throw new ArgumentNullException( "element" );

			if( !base.Children.Contains( element ) )
				throw new ArgumentException( "Must be a child element of the Canvas.", "element" );

			#endregion // Safety Check

			#region Calculate Z-Indici And Offset

			// Determine the Z-Index for the target UIElement.
			int elementNewZIndex = -1;
			if( bringToFront )
			{
				foreach( UIElement elem in base.Children )
					if( elem.Visibility != Visibility.Collapsed )
						++elementNewZIndex;
			}
			else
			{
				elementNewZIndex = 0;
			}

			// Determine if the other UIElements' Z-Index 
			// should be raised or lowered by one. 
			int offset = (elementNewZIndex == 0) ? +1 : -1;

			int elementCurrentZIndex = GetZIndex( element );

			#endregion // Calculate Z-Indici And Offset

			#region Update Z-Indici

			// Update the Z-Index of every UIElement in the Canvas.
			foreach( UIElement childElement in base.Children )
			{
				if( childElement == element )
					SetZIndex( element, elementNewZIndex );
				else
				{
					int zIndex = GetZIndex( childElement );

					// Only modify the z-index of an element if it is  
					// in between the target element's old and new z-index.
					if( bringToFront && elementCurrentZIndex < zIndex ||
						!bringToFront && zIndex < elementCurrentZIndex )
					{
						SetZIndex( childElement, zIndex + offset );
					}
				}
			}

			#endregion // Update Z-Indici
		}

			#endregion // UpdateZOrder

		#endregion // Private Helpers
	}
}